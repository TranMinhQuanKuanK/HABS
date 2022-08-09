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
                .Include(x => x.Room)
                .Include(x => x.Patient)
                .Where(x => x.QrCode == qrCode)
                .Where(x => x.RoomId == roomId)
                .FirstOrDefault();
            var tr = _unitOfWork.TestRecordRepository.Get()
              .Include(x => x.Room)
                .Include(x => x.Patient)
              .Where(x => x.QrCode == qrCode)
              .Where(x => x.RoomId == roomId)
              .FirstOrDefault();
            if (cr == null && tr == null)
            {
                throw new Exception("QR doesn't exist");
            }
            if (cr != null)
            {
                //Checkin phòng khám
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
                await _unitOfWork.SaveChangesAsync();
                //cập nhật queue
                if (cr.RoomId != null)
                {
                    _scheduleService.UpdateRedis_CheckupQueue((long)cr.RoomId);
                }
                //remind mobile
                await _notiService.SendUpdateCheckupInfoReminder(cr.Id, cr.Patient.AccountId);
            }
            else if (tr != null)
            {
                //Checkin phòng xét nghiệm
                //đổi status
                tr.Status = TestRecordStatus.CHECKED_IN;
                //cập nhật queue
                if (tr.RoomId != null)
                {
                    _scheduleService.UpdateRedis_TestQueue((long)tr.RoomId, false);
                }
                await _unitOfWork.SaveChangesAsync();
                //remind mobile
                if (tr.CheckupRecordId != null)
                {
                    await _notiService.SendUpdateCheckupInfoReminder((long)tr.CheckupRecordId,
                        tr.Patient.AccountId);
                }
            }

        }
    }
}
