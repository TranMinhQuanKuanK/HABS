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
        public async Task Checkin(string qrCode, long roomId)
        {
            var cr = _unitOfWork.CheckupRecordRepository.Get()
                .Include(x => x.Patient)
                .Include(x=>x.Room)
                .Where(x => x.QrCode == qrCode)
                .Where(x => x.RoomId == roomId)
                .FirstOrDefault();
            if (cr == null)
            {
                throw new Exception("QR doesn't exist");
            }
            if ((long)cr.Room.RoomTypeId== IdConfig.ID_ROOMTYPE_PHONG_KHAM)
            {
                if (cr.Status == CheckupRecordStatus.DA_THANH_TOAN)
                {
                    cr.Status = CheckupRecordStatus.CHECKED_IN;

                }
                else if (cr.Status == CheckupRecordStatus.DA_CO_KQXN)
                {
                    cr.Status = CheckupRecordStatus.CHECKED_IN_SAU_XN;
                }
                else
                {
                    return;
                };
                //cập nhật queue
                if (cr.RoomId != null)
                {
                    _scheduleService.UpdateRedis_CheckupQueue((long)cr.RoomId);
                }
                //}
                //if (doUpdateTestQueue)
                //{
                //    foreach(var id in tempRoomForTest)
                //    {
                //        _scheduleService.UpdateRedis_TestQueue(id, false);
                //    }
                //}
                //remind mobile
                await _notiService.SendUpdateCheckupInfoReminder(cr.Id, cr.Patient.AccountId);
            } else
            {
                //đổi status
                //cập nhật queue
                //remind mobile
            }

        }
    }
}
