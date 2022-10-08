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
using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Notification;
using BusinessLayer.Interfaces.Screen;
using BusinessLayer.ResponseModels.ViewModels.Screen;

namespace BusinessLayer.Services.Screen
{
    public class LoginService : BaseService, ILoginService
    {
        private readonly Interfaces.Doctor.IScheduleService _scheduleService;
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        private readonly INotificationService _notiService;

        public LoginService(IUnitOfWork unitOfWork, IDistributedCache distributedCache,
             Interfaces.Doctor.IScheduleService scheduleService,
              INotificationService notiService
            ) : base(unitOfWork)
        {
            _notiService = notiService;
            _scheduleService = scheduleService;
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public ScreenLoginViewModel LoginRoom(long roomId, string password)
        {
            if (password!="ABC123")
            {
                throw new Exception("Password isn't correct");
            }
            var room = _unitOfWork.RoomRepository.Get()
                .Include(x=>x.Department)
                .Include(x=>x.RoomType)
                .Where(x => x.Id == roomId).Select(x=>new ScreenLoginViewModel() { 
                    Id = x.Id,
                    RoomNumber = x.RoomNumber,
                    RoomType = x.RoomType.Name,
                    Department = x.Department.Name,
                    DepartmentId = x.DepartmentId,
                    Floor =x.Floor,
                    Note= x.Note,
                    RoomTypeId = x.RoomTypeId,
                    IsCheckupRoom = x.RoomTypeId == IdConfig.ID_ROOMTYPE_PHONG_KHAM
                })
                .FirstOrDefault();
            if (room == null)
            {
                throw new Exception("Room doesn't exist");
            }
            return room;
        }
    }
}
