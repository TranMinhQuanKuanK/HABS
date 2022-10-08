
using BusinessLayer.Constants;
using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.SearchModels.User;
using BusinessLayer.Services.Redis;
using DataAccessLayer.Models;
using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using static DataAccessLayer.Models.CheckupRecord;
using static DataAccessLayer.Models.Doctor;

namespace BusinessLayer.Services.User
{
    public class ScheduleService : BaseService, IScheduleService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        //config
        private readonly BaseConfig _baseConfig;

        public ScheduleService(IUnitOfWork unitOfWork, IDistributedCache distributedCache,
            BaseConfig baseConfig) : base(unitOfWork)
        {
            _baseConfig = baseConfig;
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public List<CheckupAppointmentResponseModel> GetCheckupAppointment(CheckupAppointmentSearchModel searchModel,
        long accountId)
        {
            var result = _unitOfWork.CheckupRecordRepository.Get()
                            .Include(x => x.Room).ThenInclude(x => x.RoomType)
                            .Where(x => searchModel.FromTime == null || ((DateTime)x.EstimatedDate).Date >= ((DateTime)searchModel.FromTime).Date)
                            .Where(x => searchModel.ToTime == null || ((DateTime)x.EstimatedDate).Date <= ((DateTime)searchModel.ToTime).Date)
                            .Where(x => searchModel.DepartmentId == null || x.DepartmentId == searchModel.DepartmentId)
                            .Where(x => searchModel.PatientId == null || x.PatientId == searchModel.PatientId)
                            .Where(x => x.Patient.AccountId == accountId)
                            .Where(x => searchModel.IsFutureReExam ? x.Status == CheckupRecordStatus.CHO_TAI_KHAM
                                : (x.Status != CheckupRecordStatus.CHUYEN_KHOA
                                && x.Status != CheckupRecordStatus.DA_HUY
                                && x.Status != CheckupRecordStatus.DA_XOA
                                && x.Status != CheckupRecordStatus.NHAP_VIEN
                                && x.Status != CheckupRecordStatus.KET_THUC
                                && x.Status != CheckupRecordStatus.CHO_TAI_KHAM)
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
                                RoomNumber = x.RoomNumber,
                                RoomId = x.RoomId,
                                Floor = x.Floor,
                                RoomType = x.Room.RoomType.Name,
                                QrCode = x.QrCode
                            }).ToList();
            return result;
        }
        //có thể những slot này thuộc những phòng khác nhau
        public List<CheckupSlotResponseModel> GetAvailableSlots(SlotSearchModel search)
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
            DateTime searchDate = search.Date.Date;
            //thêm lịch buổi sáng
            if (morningRoom != null)
            {
                int _curSlot = 1;
                var start = new DateTime(searchDate.Year, searchDate.Month, searchDate.Day,
               _baseConfig.WorkingShiftConfig.BeginMorningShiftHour,
               _baseConfig.WorkingShiftConfig.BeginAfternoonShiftMinute,
               0);
                var end = new DateTime(searchDate.Year, searchDate.Month, searchDate.Day,
                    _baseConfig.WorkingShiftConfig.EndMorningShiftHour,
                    _baseConfig.WorkingShiftConfig.EndMorningShiftMinute,
                    0);
                var temp = start;

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
                        Session = (int)SessionType.SANG
                    };
                    result.Add(slot);
                    _curSlot++;
                    temp = temp = temp.AddMinutes((int)doctor.AverageCheckupDuration);
                }
            }

            //thêm lịch buổi chiều
            if (afternoonRoom != null)
            {
                int _curSlot = 1;
                var start = new DateTime(searchDate.Year, searchDate.Month, searchDate.Day,
                    _baseConfig.WorkingShiftConfig.BeginAfternoonShiftHour,
                    _baseConfig.WorkingShiftConfig.BeginAfternoonShiftMinute,
                    0);
                var end = new DateTime(searchDate.Year, searchDate.Month, searchDate.Day,
                    _baseConfig.WorkingShiftConfig.EndAfternoonShiftHour,
                    _baseConfig.WorkingShiftConfig.EndAfternoonShiftMinute,
                    0);
                var temp = start;
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
                        Session = (int)SessionType.CHIEU
                    };
                    result.Add(slot);
                    _curSlot++;
                    temp = temp.AddMinutes((int)doctor.AverageCheckupDuration);
                }
            }
            //thêm lịch buổi tối
            if (eveningRoom != null)
            {
                int _curSlot = 1;
                var start = new DateTime(searchDate.Year, searchDate.Month, searchDate.Day,
                    _baseConfig.WorkingShiftConfig.BeginEveningShiftHour,
                    _baseConfig.WorkingShiftConfig.BeginEveningShiftMinute,
                    0);
                var end = new DateTime(searchDate.Year, searchDate.Month, searchDate.Day,
                    _baseConfig.WorkingShiftConfig.EndEveningShiftHour,
                    _baseConfig.WorkingShiftConfig.EndEveningShiftMinute,
                    0);
                var temp = start;
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
                        Session = (int)SessionType.TOI
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
        //trùng với loginService của doctor => chuyển sang util
        private SessionType? getSession(DateTime time)
        {
            SessionType? session = null;
            var beginMorningShift = new DateTime(time.Year, time.Month, time.Day,
                _baseConfig.WorkingShiftConfig.BeginMorningShiftHour, _baseConfig.WorkingShiftConfig.BeginMorningShiftMinute, 0);
            var endMorningShift = new DateTime(time.Year, time.Month, time.Day,
               _baseConfig.WorkingShiftConfig.EndMorningShiftHour, _baseConfig.WorkingShiftConfig.EndMorningShiftMinute, 0);

            var beginEveningShift = new DateTime(time.Year, time.Month, time.Day,
                _baseConfig.WorkingShiftConfig.BeginEveningShiftHour, _baseConfig.WorkingShiftConfig.BeginEveningShiftMinute, 0);
            var endEveningShift = new DateTime(time.Year, time.Month, time.Day,
               _baseConfig.WorkingShiftConfig.EndEveningShiftHour, _baseConfig.WorkingShiftConfig.EndAfternoonShiftMinute, 0);

            var beginAfternoonShift = new DateTime(time.Year, time.Month, time.Day,
               _baseConfig.WorkingShiftConfig.BeginAfternoonShiftHour, _baseConfig.WorkingShiftConfig.BeginAfternoonShiftMinute, 0);
            var endAfternoonShift = new DateTime(time.Year, time.Month, time.Day,
               _baseConfig.WorkingShiftConfig.EndAfternoonShiftHour, _baseConfig.WorkingShiftConfig.EndAfternoonShiftMinute, 0);

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
