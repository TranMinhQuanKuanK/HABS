
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
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        public List<CheckupAppointmentResponseModel> UpdateRedis_CheckupAppointment(CheckupAppointmentSearchModel searchModel)
        {
            string redisKey = $"checkup-appointment-from{searchModel.FromTime}-to{searchModel.ToTime}-department-{searchModel.DepartmentId}"
      + $"patient{searchModel.PatientId}";

            var result = _unitOfWork.CheckupRecordRepository.Get()
                 .Where(x => searchModel.FromTime == null || ((DateTime)x.EstimatedDate).Date >= ((DateTime)searchModel.FromTime).Date)
                 .Where(x => searchModel.ToTime == null || ((DateTime)x.EstimatedDate).Date <= ((DateTime)searchModel.ToTime).Date)
                 .Where(x => searchModel.DepartmentId == null || x.DepartmentId == searchModel.DepartmentId)
                 .Where(x => searchModel.PatientId == null || x.PatientId == searchModel.PatientId)
                 .Where(x => x.Status != CheckupRecordStatus.CHUYEN_KHOA
                 && x.Status != CheckupRecordStatus.DA_HUY
                 && x.Status != CheckupRecordStatus.DA_XOA
                 && x.Status != CheckupRecordStatus.NHAP_VIEN
                 && x.Status != CheckupRecordStatus.KET_THUC
                 )
                 .Select(x => new CheckupAppointmentResponseModel()
                 {
                     Id = x.Id,
                     PatientId = x.PatientId,
                     DoctorId = x.DoctorId,
                     DepartmentId = x.DepartmentId,
                     DepartmentName = x.DepartmentName,
                     EstimatedDate = x.EstimatedDate,
                     EstimatedStartTime = x.EstimatedStartTime,
                     DoctorName = x.DoctorName,
                     IsReExam = (bool)x.IsReExam,
                     NumericalOrder = x.NumericalOrder,
                     PatientName = x.PatientName,
                     Status = (int)x.Status,
                 }).ToList();

            _redisService.SetValueToKey(redisKey, JsonConvert.SerializeObject(result));
            return result;
        }
        public List<CheckupAppointmentResponseModel> GetCheckupAppointment(CheckupAppointmentSearchModel searchModel)
        {
            List<CheckupAppointmentResponseModel> result = null;

            string redisKey = $"checkup-appointment-from{searchModel.FromTime}-to{searchModel.ToTime}-department-{searchModel.DepartmentId}"
                + $"patient{searchModel.PatientId}";
            string dataFromRedis = _redisService.GetValueFromKey(redisKey);
            if (!String.IsNullOrEmpty(dataFromRedis))
            {
                result = JsonConvert.DeserializeObject<List<CheckupAppointmentResponseModel>>(dataFromRedis);
            }
            else
            {
                result = UpdateRedis_CheckupAppointment(searchModel);
            }

            return result;
        }
        public List<CheckupSlotResponseModel> UpdateRedis_AvailableSlots(SlotSearchModel search)
        {
            List<CheckupSlotResponseModel> result = new List<CheckupSlotResponseModel>();

            string redisKey = $"available-slots-for-doctor{search.DoctorId}-date{search.Date.Date}";

            if (search.Date.Date < DateTime.Now.Date)
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
                .Include(x => x.Room)
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
                .Where(x => ((DateTime)x.EstimatedDate).Date == search.Date.Date)
                .Where(x => x.Status != CheckupRecordStatus.CHO_TAI_KHAM
                && x.Status != CheckupRecordStatus.DA_HUY
                && x.Status != CheckupRecordStatus.DA_XOA)
                .ToList();

            #region Thêm lịch
            DateTime now = DateTime.Now.AddHours(7);

            DateTime start = new DateTime(now.Year, now.Month, now.Day,
                WorkingShiftConfig.BeginMorningShiftHour,
                WorkingShiftConfig.BeginAfternoonShiftMinute,
                0);
            DateTime end = new DateTime(now.Year, now.Month, now.Day,
                WorkingShiftConfig.EndMorningShiftHour,
                WorkingShiftConfig.EndMorningShiftMinute,
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
                    WorkingShiftConfig.BeginAfternoonShiftHour,
                    WorkingShiftConfig.BeginAfternoonShiftMinute,
                    0);
                end = new DateTime(now.Year, now.Month, now.Day,
                    WorkingShiftConfig.EndAfternoonShiftHour,
                    WorkingShiftConfig.EndAfternoonShiftMinute,
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
                    WorkingShiftConfig.BeginEveningShiftHour,
                    WorkingShiftConfig.BeginEveningShiftMinute,
                    0);
                end = new DateTime(now.Year, now.Month, now.Day,
                    WorkingShiftConfig.EndEveningShiftHour,
                    WorkingShiftConfig.EndEveningShiftMinute,
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
                    if (cr.NumericalOrder == slot.NumericalOrder
                        && (getSession((DateTime)cr.EstimatedStartTime)
                        == getSession((DateTime)slot.EstimatedStartTime)))
                    {
                        slot.IsAvailable = false;
                    }
                }
            }
            return result;
        }
        //có thể những slot này thuộc những phòng khác nhau
        public List<CheckupSlotResponseModel> GetAvailableSlots(SlotSearchModel search)
        {
            List<CheckupSlotResponseModel> result = new List<CheckupSlotResponseModel>();

            string redisKey = $"available-slots-for-doctor{search.DoctorId}-date{search.Date.Date}";
            string dataFromRedis = _redisService.GetValueFromKey(redisKey);
            if (!String.IsNullOrEmpty(dataFromRedis))
            {
                result = JsonConvert.DeserializeObject<List<CheckupSlotResponseModel>>(dataFromRedis);
            }
            else
            {
                result = UpdateRedis_AvailableSlots(search);
            }

            return result;
        }
        //trùng với loginService của doctor => chuyển sang util
        private SessionType? getSession(DateTime time)
        {
            SessionType? session = null;
            var beginMorningShift = new DateTime(time.Year, time.Month, time.Day,
                WorkingShiftConfig.BeginMorningShiftHour, WorkingShiftConfig.BeginMorningShiftMinute, 0);
            var endMorningShift = new DateTime(time.Year, time.Month, time.Day,
               WorkingShiftConfig.EndMorningShiftHour, WorkingShiftConfig.EndMorningShiftMinute, 0);

            var beginEveningShift = new DateTime(time.Year, time.Month, time.Day,
                WorkingShiftConfig.BeginEveningShiftHour, WorkingShiftConfig.BeginEveningShiftMinute, 0);
            var endEveningShift = new DateTime(time.Year, time.Month, time.Day,
               WorkingShiftConfig.EndEveningShiftHour, WorkingShiftConfig.EndAfternoonShiftMinute, 0);

            var beginAfternoonShift = new DateTime(time.Year, time.Month, time.Day,
               WorkingShiftConfig.BeginAfternoonShiftHour, WorkingShiftConfig.BeginAfternoonShiftMinute, 0);
            var endAfternoonShift = new DateTime(time.Year, time.Month, time.Day,
               WorkingShiftConfig.EndAfternoonShiftHour, WorkingShiftConfig.EndAfternoonShiftMinute, 0);

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
