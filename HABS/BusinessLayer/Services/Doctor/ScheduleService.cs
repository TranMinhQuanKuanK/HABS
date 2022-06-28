using DataAcessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using BusinessLayer.Services.Redis;
using Newtonsoft.Json;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using static DataAccessLayer.Models.CheckupRecord;
using static DataAccessLayer.Models.TestRecord;

namespace BusinessLayer.Services.Doctor
{
    public class ScheduleService : BaseService, IScheduleService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public ScheduleService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public List<CheckupAppointmentViewModel> GetCheckupQueue(long RoomId)
        {
            var queue = _unitOfWork.CheckupRecordRepository.Get()
                .Where(x => x.RoomId == RoomId)
                .Where(x => x.Status == CheckupRecordStatus.DANG_KHAM
                || x.Status == CheckupRecordStatus.DA_THANH_TOAN
                || x.Status == CheckupRecordStatus.DA_CO_KQXN
                )
                .Where(x => ((DateTime)x.EstimatedDate).Date == DateTime.Now.AddHours(7).Date)
                .OrderBy(x => ((DateTime)x.EstimatedStartTime).TimeOfDay)
                .Select(x => new CheckupAppointmentViewModel()
                {
                    Id = x.Id,
                    EstimatedStartTime = x.EstimatedStartTime,
                    PatientId = x.PatientId,
                    PatientName = x.PatientName,
                    NumericalOrder = x.NumericalOrder,
                    Status = (int)x.Status,
                    IsReExam = (bool)x.IsReExam

                }).ToList();
            var checkingUpPatient = queue.SingleOrDefault(x => x.Status == (int)CheckupRecordStatus.DANG_KHAM);
            if (checkingUpPatient != null)
            {
                queue.Remove(checkingUpPatient);
                queue.Insert(0, checkingUpPatient);
            }
            return queue;
        }
        public List<TestAppointmentViewModel> GetTestQueue(long RoomId, bool isWaitingForResult)
        {
            var queue = _unitOfWork.TestRecordRepository.Get()
                .Where(x => x.RoomId == RoomId)
                .Where(x => (isWaitingForResult)?
                 x.Status == TestRecordStatus.CHO_KET_QUA
                 :
                x.Status == TestRecordStatus.DA_THANH_TOAN
                || x.Status == TestRecordStatus.DANG_TIEN_HANH
                )
                .Where(x => ((DateTime)x.Date).Date == DateTime.Now.AddHours(7).Date)
                .OrderBy(x => x.NumericalOrder)
                .Select(x => new TestAppointmentViewModel()
                {
                    Id = x.Id,
                    PatientId = x.PatientId,
                    PatientName = x.PatientName,
                    NumericalOrder = x.NumericalOrder,
                    Status = (int)x.Status,
                    OperationId = (long)x.OperationId,
                    OperationName = x.OperationName,
                    Date = x.Date
                }).ToList();
            var testingPatient = queue.SingleOrDefault(x => x.Status == (int)TestRecordStatus.DANG_TIEN_HANH);
            if (testingPatient != null)
            {
                queue.Remove(testingPatient);
                queue.Insert(0, testingPatient);
            }
            return queue;
        }
    }
}
