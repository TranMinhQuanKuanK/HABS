
using DataAcessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using BusinessLayer.Services.Redis;
using BusinessLayer.Interfaces.User;
using BusinessLayer.ResponseModels.SearchModels.User;
using BusinessLayer.RequestModels.SearchModels.User;
using static DataAccessLayer.Models.CheckupRecord;
using BusinessLayer.Constants;
using static DataAccessLayer.Models.Doctor;
using DataAccessLayer.Models;

namespace BusinessLayer.Services.User
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
        //có thể những slot này thuộc những phòng khác nhau
        public List<CheckupSlotResponseModel> GetAvailableSlots(SlotSearchModel search)
        {
            if (search.Date.Date<DateTime.Now.Date)
            {
                throw new Exception("Invalid time");
            }
            var doctor = _unitOfWork.DoctorRepository.Get()
                .Where(x => x.Id == search.DoctorId)
                .Where(x => x.Type == DoctorType.BS_DA_KHOA)
                .FirstOrDefault();
            if (doctor == null)
            {
                throw new Exception("Doctor doesn't exist");
            }
            //lấy lịch làm việc ở các phòng của bác sĩ
            var schedule = _unitOfWork.ScheduleRepository.Get()
                .Where(x => x.DoctorId == search.DoctorId)
                .Where(x => x.Weekday == search.Date.DayOfWeek)
                .ToList();
            Room morningRoom = null, afternoonRoom = null, eveningRoom = null;
            foreach (var s in schedule)
            {
                if (s.Session == SessionType.SANG)
                {
                    morningRoom = s.Room;
                }
                else if (s.Session == SessionType.CHIEU)
                {
                    afternoonRoom = s.Room;
                }
                else if (s.Session == SessionType.TOI)
                {
                    eveningRoom = s.Room;
                }
            }
            //lấy các CR đã được đặt khám ông bác sĩ này
            var crList = _unitOfWork.CheckupRecordRepository.Get()
                .Where(x => x.DoctorId == search.DoctorId)
                .Where(x => ((DateTime)x.EstimatedDate).Date==search.Date.Date)
                .Where(x => x.Status != CheckupRecordStatus.CHO_TAI_KHAM
                && x.Status != CheckupRecordStatus.DA_HUY
                && x.Status != CheckupRecordStatus.DA_XOA)
                .ToList();
            List<CheckupSlotResponseModel> result = new List<CheckupSlotResponseModel>();

            #region Thêm lịch
            DateTime now = DateTime.Now.AddHours(7);

            DateTime start = new DateTime(now.Year, now.Month, now.Day,
                WorkingShiftConstant.BeginMorningShiftHour,
                WorkingShiftConstant.BeginAfternoonShiftMinute,
                0);
            DateTime end = new DateTime(now.Year, now.Month, now.Day,
                WorkingShiftConstant.EndMorningShiftHour,
                WorkingShiftConstant.EndMorningShiftMinute,
                0);
            DateTime temp = start;
            int _curSlot = 1;

            //thêm lịch buổi sáng
            if (morningRoom != null)
            {
                while (temp <= end)
                {
                    var slot = new CheckupSlotResponseModel()
                    {
                        IsAvailable = true,
                        NumericalOrder = _curSlot,
                        EstimatedStartTime = temp,
                        RoomId = morningRoom.Id,
                        RoomNumber = morningRoom.RoomNumber,
                        Floor = morningRoom.Floor,
                    };
                    result.Add(slot);
                    _curSlot++;
                    temp = temp = temp.AddMinutes((int)doctor.AverageCheckupDuration);
                }
            }

            //thêm lịch buổi chiều
            if (afternoonRoom != null)
            {
                _curSlot = 1;
                start = new DateTime(now.Year, now.Month, now.Day,
                    WorkingShiftConstant.BeginAfternoonShiftHour,
                    WorkingShiftConstant.BeginAfternoonShiftMinute,
                    0);
                end = new DateTime(now.Year, now.Month, now.Day,
                    WorkingShiftConstant.EndAfternoonShiftHour,
                    WorkingShiftConstant.EndAfternoonShiftMinute,
                    0);
                temp = start;
                while (temp <= end)
                {
                    var slot = new CheckupSlotResponseModel()
                    {
                        IsAvailable = true,
                        NumericalOrder = _curSlot,
                        EstimatedStartTime = temp,
                        RoomId = afternoonRoom.Id,
                        RoomNumber = afternoonRoom.RoomNumber,
                        Floor = afternoonRoom.Floor,
                    };
                    result.Add(slot);
                    _curSlot++;
                    temp = temp.AddMinutes((int)doctor.AverageCheckupDuration);
                }
            }
            //thêm lịch buổi tối
            if (eveningRoom != null)
            {
                _curSlot = 1;
                start = new DateTime(now.Year, now.Month, now.Day,
                    WorkingShiftConstant.BeginEveningShiftHour,
                    WorkingShiftConstant.BeginEveningShiftMinute,
                    0);
                end = new DateTime(now.Year, now.Month, now.Day,
                    WorkingShiftConstant.EndEveningShiftHour,
                    WorkingShiftConstant.EndEveningShiftMinute,
                    0);
                temp = start;
                while (temp <= end)
                {
                    var slot = new CheckupSlotResponseModel()
                    {
                        IsAvailable = true,
                        NumericalOrder = _curSlot,
                        EstimatedStartTime = temp,
                        RoomId = eveningRoom.Id,
                        RoomNumber = eveningRoom.RoomNumber,
                        Floor = eveningRoom.Floor,
                    };
                    result.Add(slot);
                    _curSlot++;
                    temp = temp.AddMinutes((int)doctor.AverageCheckupDuration);
                }
            }

            #endregion Thêm lịch
            //duyệt tick "false" vào các lịch khám đã được đặt trước
            foreach (var slot in result)
            {
                foreach (var cr in crList)
                {
                    if (cr.NumericalOrder==slot.NumericalOrder
                        && (getSession((DateTime)cr.EstimatedStartTime) 
                        == getSession((DateTime)slot.EstimatedStartTime)))
                    {
                        slot.IsAvailable = false;
                    }
                }
            }
            return result;
        }
        //trùng với loginService của doctor => chuyển sang util
        private SessionType? getSession(DateTime time)
        {
            SessionType? session = null;
            var beginMorningShift = new DateTime(time.Year, time.Month, time.Day,
                WorkingShiftConstant.BeginMorningShiftHour, WorkingShiftConstant.BeginMorningShiftMinute, 0);
            var endMorningShift = new DateTime(time.Year, time.Month, time.Day,
               WorkingShiftConstant.EndMorningShiftHour, WorkingShiftConstant.EndMorningShiftMinute, 0);

            var beginEveningShift = new DateTime(time.Year, time.Month, time.Day,
                WorkingShiftConstant.BeginEveningShiftHour, WorkingShiftConstant.BeginEveningShiftMinute, 0);
            var endEveningShift = new DateTime(time.Year, time.Month, time.Day,
               WorkingShiftConstant.EndEveningShiftHour, WorkingShiftConstant.EndAfternoonShiftMinute, 0);

            var beginAfternoonShift = new DateTime(time.Year, time.Month, time.Day,
               WorkingShiftConstant.BeginAfternoonShiftHour, WorkingShiftConstant.BeginAfternoonShiftMinute, 0);
            var endAfternoonShift = new DateTime(time.Year, time.Month, time.Day,
               WorkingShiftConstant.EndAfternoonShiftHour, WorkingShiftConstant.EndAfternoonShiftMinute, 0);

            if (time >= beginMorningShift && time <= endMorningShift)
            {
                session = SessionType.SANG;
            }
            else if (time >= beginEveningShift && time <= endEveningShift)
            {
                session = SessionType.TOI;
            }
            else if (time >= beginAfternoonShift && time <= endAfternoonShift)
            {
                session = SessionType.CHIEU;
            }
            return session;
        }
    }
}
