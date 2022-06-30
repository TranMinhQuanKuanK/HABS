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
using BusinessLayer.Interfaces.User;
using BusinessLayer.ResponseModels.ViewModels.User;
using DataAccessLayer.Models;
using static DataAccessLayer.Models.Doctor;
using static DataAccessLayer.Models.CheckupRecord;
using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Doctor;
using Utilities;
using static DataAccessLayer.Models.Operation;
using BusinessLayer.Interfaces.Common;

namespace BusinessLayer.Services.User
{
    public class CheckupRecordService : BaseService, Interfaces.User.ICheckupRecordService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly Interfaces.User.IScheduleService _scheduleService;
        private readonly IDepartmentService _departmentService;
        private readonly IOperationService _operationService;
        private readonly INumercialOrderService _numService;


        private readonly RedisService _redisService;
        public CheckupRecordService(IUnitOfWork unitOfWork, IDistributedCache distributedCache,
            Interfaces.User.IScheduleService scheduleService,
             IDepartmentService departmentService,
             IOperationService operationService,
            INumercialOrderService numService
            ) : base(unitOfWork)
        {
            _numService = numService;
            _operationService = operationService;
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
            _scheduleService = scheduleService;
            _departmentService = departmentService;
        }
        public List<PatientRecordMetadataResponseModel> GetCheckupRecordMetadata(long? patientId, DateTime? fromTime, DateTime? toTime, long? departmentId)
        {
            List<PatientRecordMetadataResponseModel> data = new List<PatientRecordMetadataResponseModel>();
            var dbSetData = _unitOfWork.CheckupRecordRepository.Get();
            IQueryable<CheckupRecord> queryableData = dbSetData;
            if (patientId != null)
            {
                queryableData = queryableData.Where(x => x.PatientId == patientId);
            }
            if (fromTime != null)
            {
                queryableData = queryableData.Where(x => x.Date >= fromTime);
            }
            if (toTime != null)
            {
                queryableData = queryableData.Where(x => x.Date <= toTime);
            }
            if (departmentId != null)
            {
                queryableData = dbSetData.Where(x => x.DepartmentId == departmentId);
            }
            data = queryableData.Select
               (x => new PatientRecordMetadataResponseModel()
               {
                   Id = x.Id,
                   Status = (int)x.Status,
                   Date = x.Date,
                   DepartmentName = x.DepartmentName,
                   DoctorName = x.DoctorName,
                   NumericalOrder = x.NumericalOrder,
                   PatientName = x.PatientName
               }
               ).ToList();
            return data;
        }
        public PatientRecordFullDataResponseModel GetCheckupRecordFullData(long recordId)
        {
            PatientRecordFullDataResponseModel data = new PatientRecordFullDataResponseModel();
            data = _unitOfWork.CheckupRecordRepository.Get()
                .Include(x => x.Patient)
                .Include(x => x.Prescriptions)
                    .ThenInclude(x => x.PrescriptionDetails)
                .Include(x => x.TestRecords)
                .Where(x => x.Id == recordId).AsEnumerable().Select
                (x =>
                {
                    var _prescription = x.Prescriptions.ToArray()[0];
                    return new PatientRecordFullDataResponseModel()
                    {
                        Id = x.Id,
                        Status = (int)x.Status,
                        Date = x.Date,
                        DepartmentName = x.DepartmentName,
                        DoctorName = x.DoctorName,
                        NumericalOrder = x.NumericalOrder,
                        PatientName = x.PatientName,
                        BloodPressure = x.BloodPressure,
                        ClinicalSymptom = x.ClinicalSymptom,
                        DepartmentId = x.DepartmentId,
                        Diagnosis = x.Diagnosis,
                        DoctorAdvice = x.DoctorAdvice,
                        DoctorId = x.DoctorId,
                        EstimatedStartTime = x.EstimatedStartTime,
                        IcdCode = x.IcdDiseaseCode,
                        IcdDiseaseId = x.IcdDiseaseId,
                        IcdDiseaseName = x.IcdDiseaseName,
                        PatientData = new PatientResponseModel()
                        {
                            Id = x.Patient.Id,
                            Address = x.Patient.Address,
                            Bhyt = x.Patient.Bhyt,
                            DateOfBirth = x.Patient.DateOfBirth,
                            Gender = x.Patient.Gender,
                            PhoneNumber = x.Patient.PhoneNumber,
                            Name = x.Patient.Name,
                        },
                        PatientId = x.PatientId,
                        Prescription = new PrescriptionResponseModel()
                        {
                            CheckupRecordId = _prescription.CheckupRecordId,
                            Id = _prescription.Id,
                            TimeCreated = _prescription.TimeCreated,
                            Note = _prescription.Note,
                            Details = _prescription.PrescriptionDetails.Select(dt => new PrescriptionDetailResponseModel()
                            {
                                Id = dt.Id,
                                EveningDose = dt.EveningDose,
                                MedicineId = dt.MedicineId,
                                MedicineName = dt.MedicineName,
                                MiddayDose = dt.MiddayDose,
                                MorningDose = dt.MorningDose,
                                NightDose = dt.NightDose,
                                PrescriptionId = dt.PrescriptionId,
                                Quantity = dt.Quantity,
                                Unit = dt.Unit,
                                Usage = dt.Usage,
                            }).ToList()
                        },
                        Pulse = x.Pulse,
                        EstimatedDate = x.EstimatedDate,
                        Temperature = x.Temperature,
                        TestRecords = x.TestRecords.Select(tr => new TestRecordResponseModel()
                        {
                            Id = tr.Id,
                            CheckupRecordId = tr.CheckupRecordId,
                            Date = tr.Date,
                            Floor = tr.Floor,
                            NumericalOrder = tr.NumericalOrder,
                            OperationId = (long)tr.OperationId,
                            OperationName = tr.OperationName,
                            PatientId = tr.PatientId,
                            PatientName = tr.PatientName,
                            ResultFileLink = tr.ResultFileLink,
                            RoomId = tr.RoomId,
                            RoomNumber = tr.RoomNumber,
                            Status = (int)tr.Status,
                            DoctorId = tr.DoctorId,
                            DoctorName = tr.DoctorName
                        }).ToList(),
                    };
                }).FirstOrDefault();
            return data;
        }
        public async Task CreatReExamAppointment(long patientId, long previousCrId, DateTime date, long doctorId, int? numericalOrder, string clinicalSymptom)
        {
            //kiểm tra ngày có hợp lệ, có thuộc phiên làm việc chính thức ko
            var reqSession = getSession(date);
            if (reqSession == null)
            {
                throw new Exception("Invalid time");
            }
            //kiểm tra bác sĩ
            var doctor = _unitOfWork.DoctorRepository.Get()
               .Where(x => x.Id == doctorId)
               .Where(x => x.Type == DoctorType.BS_DA_KHOA)
               .FirstOrDefault();
            if (doctor == null)
            {
                throw new Exception("Doctor doesn't exist");
            }
            //kiểm tra bệnh nhân
            var patient = _unitOfWork.PatientRepository.Get()
             .Where(x => x.Id == patientId)
             .Where(x=>x.Status==Patient.PatientStatus.HOAT_DONG)
             .FirstOrDefault();
            if (patient == null)
            {
                throw new Exception("Patient doesn't exist");
            }
            //kiểm tra cr cũ
            var prevCr = _unitOfWork.CheckupRecordRepository.Get()
                .Include(x=>x.TestRecords)
                .ThenInclude(x=>x.Operation)
                .Where(x => x.Id == previousCrId).FirstOrDefault();
            if (prevCr == null)
            {
                throw new Exception("Previous checkup record doesn't exist");

            }
            //kiểm tra xem thời điểm đó bs có làm việc ko
            var schedule = _unitOfWork.ScheduleRepository.Get()
                .Where(x => x.DoctorId == doctorId)
                .Where(x => x.Session == reqSession)
                .Where(x => x.Weekday == date.DayOfWeek)
                .Include(x => x.Room)
                .FirstOrDefault();

            if (schedule == null)
            {
                throw new Exception("No working schedule for doctor");
            }
            //nếu số thứ tự null thì lấy số nhỏ nhất có thể trong ngày hôm đó
            var avaiSlot = _scheduleService.GetAvailableSlots(new RequestModels.SearchModels.User.SlotSearchModel()
            {
                DoctorId = doctorId,
                Date = date
            });
            DateTime estimatedStartTime = new DateTime();
            if (numericalOrder == null)
            {
                for (int i = 0; i < avaiSlot.Count; i++)
                {
                    if (avaiSlot[i].IsAvailable)
                    {
                        numericalOrder = avaiSlot[i].NumericalOrder;
                        estimatedStartTime = (DateTime)avaiSlot[i].EstimatedStartTime;
                        break;
                    }
                }
            }
            else
            {
                //kiểm đã có checkup record nào chưa
                bool slotExisted = false;
                foreach (var slot in avaiSlot)
                {
                    if ((int)slot.NumericalOrder == (int)numericalOrder
                        && getSession((DateTime)slot.EstimatedStartTime) == reqSession)
                    {
                        if (slot.IsAvailable)
                        {
                            estimatedStartTime = (DateTime)slot.EstimatedStartTime;
                            break;
                        }
                        else
                        {
                            slotExisted = true;

                        }
                    }
                }
                if (slotExisted)
                {
                    throw new Exception("Slot's already been booked");
                }
                //gán thời gian bắt đầu cho hợp lí (không lấy date của client)
            }
            //chuyển status về đã đặt lịch thay vì tạo mới
            prevCr.Status = CheckupRecordStatus.DA_DAT_LICH;
            //đặt lịch cho các TR
            var prevCrTRList = _unitOfWork.TestRecordRepository
                .Get()
                .Where(x => x.CheckupRecordId == prevCr.Id)
                .ToList();
            foreach (var _tr in prevCrTRList)
            {
                var room = _numService.GetAppropriateRoomForOperation(_tr.Operation);
                if (room == null)
                {
                    throw new Exception("Rooms for this operation haven't been configured");
                }
                var numOrd = _numService.GetNumOrderForAutoIncreaseRoom(room, date);
                _tr.RoomId = room.Id;
                _tr.RoomNumber = room.RoomNumber;
                _tr.Floor = room.Floor;
                _tr.NumericalOrder = numOrd;
                _tr.Status = TestRecord.TestRecordStatus.DA_DAT_LICH;
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task CreatNewAppointment(long patientId, DateTime date, long doctorId, int? numericalOrder, string clinicalSymptom)
        {
            //kiểm tra ngày có hợp lệ, có thuộc phiên làm việc chính thức ko
            var reqSession = getSession(date);
            if (reqSession == null)
            {
                throw new Exception("Invalid time");
            }
            //kiểm tra bác sĩ
            var doctor = _unitOfWork.DoctorRepository.Get()
               .Where(x => x.Id == doctorId)
               .Where(x => x.Type == DoctorType.BS_DA_KHOA)
               .FirstOrDefault();
            if (doctor == null)
            {
                throw new Exception("Doctor doesn't exist");
            }
            //kiểm tra bệnh nhân
            var patient = _unitOfWork.PatientRepository.Get()
             .Where(x => x.Status == Patient.PatientStatus.HOAT_DONG)
             .Where(x => x.Id == patientId)
             .FirstOrDefault();
            if (patient == null)
            {
                throw new Exception("Patient doesn't exist");
            }
            //kiểm tra xem thời điểm đó bs có làm việc ko
            var schedule = _unitOfWork.ScheduleRepository.Get()
                .Where(x => x.DoctorId == doctorId)
                .Where(x => x.Session == reqSession)
                .Where(x => x.Weekday == date.DayOfWeek)
                .Include(x => x.Room)
                .FirstOrDefault();

            if (schedule == null)
            {
                throw new Exception("No working schedule for doctor");
            }
            //nếu số thứ tự null thì lấy số nhỏ nhất có thể trong ngày hôm đó
            var avaiSlot = _scheduleService.GetAvailableSlots(new RequestModels.SearchModels.User.SlotSearchModel()
            {
                DoctorId = doctorId,
                Date = date
            });
            DateTime estimatedStartTime = new DateTime();
            if (numericalOrder == null)
            {
                for (int i = 0; i < avaiSlot.Count; i++)
                {
                    if (avaiSlot[i].IsAvailable)
                    {
                        numericalOrder = avaiSlot[i].NumericalOrder;
                        estimatedStartTime = (DateTime)avaiSlot[i].EstimatedStartTime;
                        break;
                    }
                }
            }
            else
            {
                //kiểm đã có checkup record nào chưa
                bool slotExisted = false;
                foreach (var slot in avaiSlot)
                {
                    if ((int)slot.NumericalOrder == (int)numericalOrder
                        && getSession((DateTime)slot.EstimatedStartTime) == reqSession)
                    {
                        if (slot.IsAvailable)
                        {
                            estimatedStartTime = (DateTime)slot.EstimatedStartTime;
                            break;
                        }
                        else
                        {
                            slotExisted = true;

                        }
                    }
                }
                if (slotExisted)
                {
                    throw new Exception("Slot's already been booked");
                }
                //gán thời gian bắt đầu cho hợp lí (không lấy date của client)
            }
            //nếu lịch available thì 1 checkup record mới
            var dakhoaDep = _departmentService.GetDepartmentById(IdConstant.ID_DEPARTMENT_DA_KHOA);
            var dakhoaOp = _operationService.GetOperationForDepartment(dakhoaDep.Id);
            var cr = new CheckupRecord()
            {
                IsReExam = false,
                PatientId = patientId,
                PatientName = patient.Name,
                RoomId = schedule.RoomId,
                RoomNumber = schedule.Room.RoomNumber,
                Floor = schedule.Room.Floor,
                Status = CheckupRecordStatus.DA_DAT_LICH,
                NumericalOrder = numericalOrder,
                EstimatedDate = date,
                EstimatedStartTime = estimatedStartTime,
                DepartmentId = IdConstant.ID_DEPARTMENT_DA_KHOA,
                DepartmentName = dakhoaDep.Name,
                DoctorId = doctorId,
                ClinicalSymptom = clinicalSymptom,
            };
            await _unitOfWork.CheckupRecordRepository.Add(cr);
            await _unitOfWork.SaveChangesAsync();
            //tạo một bill detail và 1 bill tương ứng
            Bill bill = new Bill()
            {
                Content = "Thanh toán viện phí khám tổng quát đa khoa",
                //tính luôn
                Total = dakhoaOp.Price,
                Status = Bill.BillStatus.CHUA_TT,
                TimeCreated = DateTime.Now.AddHours(7),
                PatientName = patient.Name,
                TotalInWord = MoneyHelper.NumberToText(dakhoaOp.Price),
                PatientId = patient.Id
            };
            await _unitOfWork.BillRepository.Add(bill);
            await _unitOfWork.SaveChangesAsync();
            BillDetail bd = new BillDetail()
            {
                Price = dakhoaOp.Price,
                OperationId = dakhoaOp.Id,
                InsuranceStatus = (InsuranceSupportStatus)dakhoaOp.InsuranceStatus,
                OperationName = dakhoaOp.Name,
                Quantity = 1,
                SubTotal = dakhoaOp.Price,
                CheckupRecordId = cr.Id,
                BillId = bill.Id
            };
            await _unitOfWork.BillDetailRepository.Add(bd);
            await _unitOfWork.SaveChangesAsync();
        }
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
