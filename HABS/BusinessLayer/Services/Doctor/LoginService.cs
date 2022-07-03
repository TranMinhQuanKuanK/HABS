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
using BusinessLayer.Constants;
using static DataAccessLayer.Models.Doctor;

namespace BusinessLayer.Services.Doctor
{
    public class LoginService : BaseService, ILoginService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public LoginService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        private SessionType? getCurrentSession()
        {
            SessionType? session = null;
            var now = DateTime.Now.AddHours(7);
            var beginMorningShift = new DateTime(now.Year, now.Month, now.Day,
                WorkingShiftConfig.BeginMorningShiftHour, WorkingShiftConfig.BeginMorningShiftMinute, 0);
            var endMorningShift = new DateTime(now.Year, now.Month, now.Day,
               WorkingShiftConfig.EndMorningShiftHour, WorkingShiftConfig.EndMorningShiftMinute, 0);

            var beginEveningShift = new DateTime(now.Year, now.Month, now.Day,
                WorkingShiftConfig.BeginEveningShiftHour, WorkingShiftConfig.BeginEveningShiftMinute, 0);
            var endEveningShift = new DateTime(now.Year, now.Month, now.Day,
               WorkingShiftConfig.EndEveningShiftHour, WorkingShiftConfig.EndAfternoonShiftMinute, 0);

            var beginAfternoonShift = new DateTime(now.Year, now.Month, now.Day,
               WorkingShiftConfig.BeginAfternoonShiftHour, WorkingShiftConfig.BeginAfternoonShiftMinute, 0);
            var endAfternoonShift = new DateTime(now.Year, now.Month, now.Day,
               WorkingShiftConfig.EndAfternoonShiftHour, WorkingShiftConfig.EndAfternoonShiftMinute, 0);

            if (now >= beginMorningShift && now <= endMorningShift)
            {
                session = SessionType.SANG;
            }
            else if (now >= beginEveningShift && now <= endEveningShift)
            {
                session = SessionType.TOI;
            }
            else if (now >= beginAfternoonShift && now <= endAfternoonShift)
            {
                session = SessionType.CHIEU;
            }
            return session;
        }
        public DoctorLoginViewModel Login(LoginModel login)
        {
           
            var doctor = _unitOfWork.DoctorRepository
                .Get()
                .Where(_doc => _doc.Username == login.Username && _doc.Password == login.Password)
                .Select(_doc => new DoctorLoginViewModel()
                {
                    Id = _doc.Id,
                    Name = _doc.Name,
                    PhoneNo = _doc.PhoneNo,
                    Username = _doc.Username,
                    Type = (int) _doc.Type
                })
                .FirstOrDefault();
            if (doctor == null) return null;

            var room = _unitOfWork.RoomRepository.Get().Where(x => x.Id == login.RoomId).FirstOrDefault();
            if (room==null)
            {
                throw new Exception("Room invalid");
            }
            if (room.RoomTypeId!= IdConfig.ID_ROOMTYPE_PHONG_KHAM)
            {
                if (doctor.Type == (int)DoctorType.BS_XET_NGHIEM)
                {
                    return doctor;
                } 
            }

            #region Sau này xóa
            if (login.Username == "doctor" && login.Password == "123123")
            {
                return doctor;
            }
            #endregion Sau này xóa

            var currSess = getCurrentSession();
            if (currSess == null)
            {
                return null;
            }
            var schedule = _unitOfWork.ScheduleRepository.Get().
                Where(x => x.DoctorId == doctor.Id && x.RoomId == login.RoomId
                && x.Session == (SessionType)currSess).FirstOrDefault();
            if (schedule == null) { return null; }
            return doctor;
        }
    }

}
