using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels;
using BusinessLayer.RequestModels.SearchModels;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.Services;
using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using BusinessLayer.Services.Redis;
using Newtonsoft.Json;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using DataAccessLayer.Models;
using BusinessLayer.RequestModels.CreateModels.Doctor;
using static DataAccessLayer.Models.CheckupRecord;
using BusinessLayer.Interfaces.User;
using static DataAccessLayer.Models.Bill;
using BusinessLayer.ResponseModels.ViewModels.User;
using BusinessLayer.Interfaces.Common;
using Utilities;
using static DataAccessLayer.Models.TestRecord;
using static DataAccessLayer.Models.Doctor;
using BusinessLayer.Interfaces.Notification;

namespace BusinessLayer.Services.Doctor
{
    public class TestRecordService : BaseService, Interfaces.Doctor.ITestRecordService
    {
        private readonly IFileService _fileService;
        private readonly INotificationService _notiService;

        private readonly Interfaces.Doctor.IScheduleService _scheduleService;
        public TestRecordService(IUnitOfWork unitOfWork,
             Interfaces.Doctor.IScheduleService scheduleService,
             IFileService fileService,
              INotificationService notiService
            ) : base(unitOfWork)

        {
            _notiService = notiService;
            _fileService = fileService;
            _scheduleService = scheduleService;
        }

        public async Task UpdateTestRecordResult(TestRecordEditModel model, long doctorId)
        {
            bool doneAll = false;
            var tr = _unitOfWork.TestRecordRepository.Get()
              .Where(x => x.Id == model.Id)
              .FirstOrDefault();
            var cr = _unitOfWork.CheckupRecordRepository.Get()
                .Include(x => x.TestRecords)
                .Include(x => x.Patient)
                .Where(x => x.Id == tr.CheckupRecordId)
                .FirstOrDefault();

            if (tr == null)
            {
                throw new Exception("Test record doesn't exist");
            }

            if (model.Status != null)
            {
                if (model.Status == (int)TestRecordStatus.CHO_KET_QUA)
                {
                    tr.Status = TestRecordStatus.CHO_KET_QUA;
                }
                else if (model.Status == (int)TestRecordStatus.HOAN_THANH)
                {
                    tr.Status = TestRecordStatus.HOAN_THANH;
                    //kiếm tra nếu là xét nghiệm cuối cùng thì hoàn thành phiếu 
                    doneAll = true;
                    if (cr.TestRecords.Count > 1)
                    {
                        foreach (var _tr in cr.TestRecords)
                        {
                            if (_tr.Id != tr.Id && _tr.Status != TestRecordStatus.HOAN_THANH)
                            {
                                doneAll = false;
                            }
                        }
                    }
                    if (doneAll)
                    {
                        cr.Status = CheckupRecordStatus.DA_CO_KQXN;
                        //bắn noti cho mobile là đã có đủ kết quả
                    }
                }
            }
            if (model.ResultFile != null)
            {
                // upload file lên firebase và retrieve link
                if (tr.ResultFileLink != null)
                {
                    //xóa file đã có trên firebase
                }
                var url = await _fileService.UploadToFirebase(model.ResultFile, (long)tr.PatientId, tr.Id);
                // gán link vào test record
                tr.ResultFileLink = url;
            }
            var doctor = _unitOfWork.DoctorRepository.Get().Where(x => x.Id == doctorId).FirstOrDefault();
            if (doctor == null)
            {
                throw new Exception("Doctor doesn't exist");
            }
            tr.DoctorId = doctorId;
            tr.DoctorName = doctor.Name;
            await _unitOfWork.SaveChangesAsync();
            if (model.Status != null)
            {
                if (model.Status == (int)TestRecordStatus.HOAN_THANH ||
                model.Status == (int)TestRecordStatus.CHO_KET_QUA)
                {
                    await _notiService.SendUpdateCheckupInfoReminder(cr.Id, cr.Patient.AccountId);
                }
                //cập nhật cache
                _scheduleService.UpdateRedis_TestQueue((long)tr.RoomId, false);
                _scheduleService.UpdateRedis_TestQueue((long)tr.RoomId, true);
                if (model.Status == (int)TestRecordStatus.HOAN_THANH)
                {
                    _scheduleService.UpdateRedis_FinishedTestQueue((long)tr.RoomId);
                }
            }
        }
        public async Task ConfirmTest(long doctorId, long tId)
        {
            var doc = _unitOfWork.DoctorRepository.Get()
                .Where(x => x.Id == doctorId)
                .Where(x => x.Type == DoctorType.BS_XET_NGHIEM)
                .FirstOrDefault();
            if (doc == null)
            {
                throw new Exception("Doctor doesn't exist");
            }
            var tr = _unitOfWork.TestRecordRepository.Get().Where(x => x.Id == tId).FirstOrDefault();
            if (tr == null)
            {
                throw new Exception("Invalid test record id");
            }
            //lấy hàng đợi của ngày hôm đó
            var queue = _scheduleService.GetTestQueue((long)tr.RoomId, false);
            //kiểm tra xem có thật sự trong hàng đợi không
            var trInQueue = queue.SingleOrDefault(x => x.Id == tId);
            if (trInQueue == null)
            {
                throw new Exception("Invalid test record id");
            }
            if (queue[0].Status == (int)TestRecordStatus.DANG_TIEN_HANH)
            {
                throw new Exception("A patient is currently in the test room");
            }
            trInQueue.Status = (int)TestRecordStatus.DANG_TIEN_HANH;
            tr.Status = TestRecordStatus.DANG_TIEN_HANH;
            tr.Date = DateTime.Now.AddHours(7);
            tr.DoctorId = doc.Id;
            tr.DoctorName = doc.Name;
            queue.Remove(trInQueue);
            queue.Insert(0, trInQueue);
            await _unitOfWork.SaveChangesAsync();

            _scheduleService.UpdateRedis_TestQueue((long)tr.RoomId, false);
            //Cập nhật lại cache hàng đợi tương ứng của phòng trong tr
        }
    }

}
