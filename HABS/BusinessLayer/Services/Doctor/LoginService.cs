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
        //config
        private readonly BaseConfig _baseConfig;
        public LoginService(IUnitOfWork unitOfWork, IDistributedCache distributedCache, BaseConfig baseConfig) : base(unitOfWork)
        {
            _baseConfig = baseConfig;
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }

        private SessionType? getCurrentSession()
        {
            SessionType? session = null;
            var now = DateTime.Now.AddHours(7);
            var beginMorningShift = new DateTime(now.Year, now.Month, now.Day,
                _baseConfig.WorkingShiftConfig.BeginMorningShiftHour, _baseConfig.WorkingShiftConfig.BeginMorningShiftMinute, 0);
            var endMorningShift = new DateTime(now.Year, now.Month, now.Day,
               _baseConfig.WorkingShiftConfig.EndMorningShiftHour, _baseConfig.WorkingShiftConfig.EndMorningShiftMinute, 0);

            var beginEveningShift = new DateTime(now.Year, now.Month, now.Day,
                _baseConfig.WorkingShiftConfig.BeginEveningShiftHour, _baseConfig.WorkingShiftConfig.BeginEveningShiftMinute, 0);
            var endEveningShift = new DateTime(now.Year, now.Month, now.Day,
               _baseConfig.WorkingShiftConfig.EndEveningShiftHour, _baseConfig.WorkingShiftConfig.EndEveningShiftMinute, 0);

            var beginAfternoonShift = new DateTime(now.Year, now.Month, now.Day,
               _baseConfig.WorkingShiftConfig.BeginAfternoonShiftHour, _baseConfig.WorkingShiftConfig.BeginAfternoonShiftMinute, 0);
            var endAfternoonShift = new DateTime(now.Year, now.Month, now.Day,
               _baseConfig.WorkingShiftConfig.EndAfternoonShiftHour, _baseConfig.WorkingShiftConfig.EndAfternoonShiftMinute, 0);

            if (now >= beginMorningShift
                .AddMinutes((-1) * _baseConfig.WorkingShiftConfig.LoginTimeBeforeWorkingShift) && now <= endMorningShift)
            {
                session = SessionType.SANG;
            }
            else if (now >= beginEveningShift
                .AddMinutes((-1) * _baseConfig.WorkingShiftConfig.LoginTimeBeforeWorkingShift) && now <= endEveningShift)
            {
                session = SessionType.TOI;
            }
            else if (now >= beginAfternoonShift
                .AddMinutes((-1) * _baseConfig.WorkingShiftConfig.LoginTimeBeforeWorkingShift) && now <= endAfternoonShift)
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
                    Type = (int)_doc.Type,
                })
                .FirstOrDefault();
            if (doctor == null) return null;

            var room = _unitOfWork.RoomRepository.Get()
                .Include(x => x.Department)
                .Include(x => x.RoomType)
                .Where(x => x.Id == login.RoomId).FirstOrDefault();
            if (room == null)
            {
                throw new Exception("Room invalid");
            }

            doctor.Room = new RoomViewModel()
            {
                Id = room.Id,
                DepartmentId = room.DepartmentId == null ? null : room.DepartmentId,
                DepartmentName = room.Department == null ? null : room.Department.Name,
                Floor = room.Floor,
                IsGeneralRoom = room.DepartmentId == null ? false : room.DepartmentId == IdConfig.ID_DEPARTMENT_DA_KHOA,
                RoomTypeId = (long)room.RoomTypeId,
                RoomNumber = room.RoomNumber,
                Note = room.Note,
                RoomTypeName = room.RoomType.Name,
            };
            #region AccountFor
            if (login.Username == "doctor" && login.Password == "123")
            {
                if (room != null)
                {
                    doctor.Room = new RoomViewModel()
                    {
                        Id = room.Id,
                        DepartmentId = room.DepartmentId == null ? null : room.DepartmentId,
                        DepartmentName = room.Department == null ? null : room.Department.Name,
                        Floor = room.Floor,
                        IsGeneralRoom = room.DepartmentId == null ? false : room.DepartmentId == IdConfig.ID_DEPARTMENT_DA_KHOA,
                        RoomTypeId = (long)room.RoomTypeId,
                        RoomNumber = room.RoomNumber,
                        Note = room.Note,
                        RoomTypeName = room.RoomType.Name,
                    };
                }
                return doctor;
            }
            #endregion AccountForDevelopingPurpose
            //bác sĩ xét nghiệm hoặc bác sĩ chuyên khoa thì bỏ qua check session
            bool isTestDoctor = room.RoomTypeId != IdConfig.ID_ROOMTYPE_PHONG_KHAM &&
               doctor.Type == (int)DoctorType.BS_XET_NGHIEM;
            bool isSpecificDoctor = room.RoomTypeId == IdConfig.ID_ROOMTYPE_PHONG_KHAM &&
                room.DepartmentId != null &&
                doctor.Type == (int)DoctorType.BS_CHUYEN_KHOA;
            if (isTestDoctor|| isSpecificDoctor)
            {
                return doctor;
            }
           
            SessionType? currSess = null;
            currSess = getCurrentSession();
            if (currSess == null)
            {
                return null;
            }
            var now = DateTime.Now.AddHours(7);
            var schedule = _unitOfWork.ScheduleRepository.Get().
                Where(x => x.DoctorId == doctor.Id && x.RoomId == login.RoomId && x.Weekday == now.DayOfWeek
                && x.Session == (SessionType)currSess).FirstOrDefault();
            if (schedule == null) { return null; }
            return doctor;
        }
    }

}
