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
                WorkingShiftConstant.BeginMorningShiftHour, WorkingShiftConstant.BeginMorningShiftMinute, 0);
            var endMorningShift = new DateTime(now.Year, now.Month, now.Day,
               WorkingShiftConstant.EndMorningShiftHour, WorkingShiftConstant.EndMorningShiftMinute, 0);

            var beginEveningShift = new DateTime(now.Year, now.Month, now.Day,
                WorkingShiftConstant.BeginEveningShiftHour, WorkingShiftConstant.BeginEveningShiftMinute, 0);
            var endEveningShift = new DateTime(now.Year, now.Month, now.Day,
               WorkingShiftConstant.EndEveningShiftHour, WorkingShiftConstant.EndAfternoonShiftMinute, 0);

            var beginAfternoonShift = new DateTime(now.Year, now.Month, now.Day,
               WorkingShiftConstant.BeginAfternoonShiftHour, WorkingShiftConstant.BeginAfternoonShiftMinute, 0);
            var endAfternoonShift = new DateTime(now.Year, now.Month, now.Day,
               WorkingShiftConstant.EndAfternoonShiftHour, WorkingShiftConstant.EndAfternoonShiftMinute, 0);

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
                .Where(x => x.Username == login.Username && x.Password == login.Password)
                .Select(x=>new DoctorLoginViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    PhoneNo = x.PhoneNo,
                    Username = x.Username,
                    Type = (int) x.Type
                })
                .FirstOrDefault();
            if (doctor == null) return null;
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
