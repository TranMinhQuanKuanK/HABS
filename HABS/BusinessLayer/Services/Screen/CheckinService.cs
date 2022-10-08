using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using BusinessLayer.Services.Redis;
using BusinessLayer.Interfaces.Notification;
using BusinessLayer.Interfaces.Screen;
using static DataAccessLayer.Models.CheckupRecord;
using BusinessLayer.Constants;
using static DataAccessLayer.Models.TestRecord;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System.Collections.Generic;

namespace BusinessLayer.Services.Screen
{
    public class CheckinService : BaseService, ICheckinService
    {
        private readonly Interfaces.Doctor.IScheduleService _scheduleService;
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        private readonly INotificationService _notiService;

        public CheckinService(IUnitOfWork unitOfWork, IDistributedCache distributedCache,
             Interfaces.Doctor.IScheduleService scheduleService,
              INotificationService notiService
            ) : base(unitOfWork)
        {
            _notiService = notiService;
            _scheduleService = scheduleService;
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public async Task<List<TestAppointmentViewModel>> CheckinForTestRecord(string qrCode, long roomId)
        {
            //Checkin phòng xét nghiệm
            var tr = _unitOfWork.TestRecordRepository.Get()
            .Include(x => x.Room)
              .Include(x => x.Patient)
            .Where(x => x.QrCode == qrCode)
            .Where(x => x.RoomId == roomId)
            .FirstOrDefault();
            if (tr == null)
            {
                throw new Exception("QR doesn't exist");
            }
            //đổi status
            if (tr.Status == TestRecordStatus.DA_THANH_TOAN)
            {
                tr.Status = TestRecordStatus.CHECKED_IN;
            }
            else
            {
                throw new Exception("Invalid QR");
            }
            //cập nhật queue
            await _unitOfWork.SaveChangesAsync();
            List<TestAppointmentViewModel> result = new List<TestAppointmentViewModel>();
            if (tr.RoomId != null)
            {
                result = _scheduleService.UpdateRedis_TestQueue((long)tr.RoomId, false);
            }
            //remind mobile
            if (tr.CheckupRecordId != null)
            {
                await _notiService.SendUpdateCheckupInfoReminder((long)tr.CheckupRecordId,
                    tr.Patient.AccountId);
            }
            return result;
        }
        public async Task<List<CheckupAppointmentViewModel>> CheckinForCheckupRecord(string qrCode, long roomId)
        {
            //Checkin phòng khám
            var cr = _unitOfWork.CheckupRecordRepository.Get()
              .Include(x => x.Room)
              .Include(x => x.Patient)
              .Where(x => x.QrCode == qrCode)
              .Where(x => x.RoomId == roomId)
              .FirstOrDefault();
            bool isCheckinFromTesting = false;
            if (cr == null)
            {
                throw new Exception("QR doesn't exist");
            }
            //đổi status
            if (cr.Status == CheckupRecordStatus.DA_THANH_TOAN)
            {
                cr.Status = CheckupRecordStatus.CHECKED_IN;
            }
            else if (cr.Status == CheckupRecordStatus.DA_CO_KQXN)
            {
                isCheckinFromTesting = true;
                cr.Status = CheckupRecordStatus.CHECKED_IN_SAU_XN;
            }
            else
            {
                throw new Exception("Invalid QR");
            };
            await _unitOfWork.SaveChangesAsync();
            //cập nhật queue
            List<CheckupAppointmentViewModel> result = new List<CheckupAppointmentViewModel>();
            if (cr.RoomId != null)
            {
                if (isCheckinFromTesting)
                {
                    _scheduleService.UpdateRedis_TestingCheckupQueue((long)cr.RoomId);
                    result = _scheduleService.UpdateRedis_CheckupQueue((long)cr.RoomId);
                }
                else
                {
                    result = _scheduleService.UpdateRedis_CheckupQueue((long)cr.RoomId);
                }
            }
            //remind mobile
            await _notiService.SendUpdateCheckupInfoReminder(cr.Id, cr.Patient.AccountId);
            return result;
        }
    }
}
