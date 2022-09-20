using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Common;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.ResponseModels.ViewModels.User;
using BusinessLayer.Services.Redis;
using DataAccessLayer.Models;
using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Utilities;
using static DataAccessLayer.Models.CheckupRecord;
using static DataAccessLayer.Models.Doctor;
using static DataAccessLayer.Models.Operation;

namespace BusinessLayer.Services.User
{
    public class CheckupRecordService : BaseService, Interfaces.User.ICheckupRecordService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly Interfaces.User.IScheduleService _scheduleService;
        private readonly Interfaces.Doctor.IScheduleService _scheduleServiceDoctor;
        private readonly IDepartmentService _departmentService;
        private readonly IOperationService _operationService;
        private readonly INumercialOrderService _numService;
        //config 
        private readonly BaseConfig _baseConfig;

        private readonly RedisService _redisService;
        public CheckupRecordService(IUnitOfWork unitOfWork, IDistributedCache distributedCache,
            Interfaces.User.IScheduleService scheduleService,
            Interfaces.Doctor.IScheduleService scheduleServiceDoctor,
             IDepartmentService departmentService,
             IOperationService operationService,
            INumercialOrderService numService,
            BaseConfig workingShiftConfig
            ) : base(unitOfWork)
        {
            _baseConfig = workingShiftConfig;
            _scheduleServiceDoctor = scheduleServiceDoctor;
            _numService = numService;
            _operationService = operationService;
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
            _scheduleService = scheduleService;
            _departmentService = departmentService;
            
        }
        public List<PatientRecordMetadataResponseModel> GetCheckupRecordMetadata(long? patientId, DateTime? fromTime, DateTime? toTime,
            long? departmentId, long accountId)
        {
            List<PatientRecordMetadataResponseModel> data = new List<PatientRecordMetadataResponseModel>();
            var dbSetData = _unitOfWork.CheckupRecordRepository.Get().Include(x => x.Patient);
            IQueryable<CheckupRecord> queryableData = dbSetData;
            queryableData = queryableData.Where(x => x.Patient.AccountId == accountId);
            queryableData = queryableData.Where(x => x.Status == CheckupRecordStatus.KET_THUC
           || x.Status == CheckupRecordStatus.NHAP_VIEN
           || x.Status == CheckupRecordStatus.CHUYEN_KHOA
           );
            if (patientId != null)
            {
                queryableData = queryableData.Where(x => x.PatientId == patientId);
            }
            if (fromTime != null)
            {
                queryableData = queryableData.Where(x => x.Date >= ((DateTime)fromTime).Date);
            }
            if (toTime != null)
            {
                queryableData = queryableData.Where(x => x.Date <= ((DateTime)toTime).Date);
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
                   PatientName = x.PatientName,
                   IsReExam = (bool)x.IsReExam,
                   Floor = x.Floor,
                   RoomId = (long)x.RoomId,
                   QrCode = x.QrCode,
                   RoomNumber = x.RoomNumber,
               }
               ).OrderByDescending(x=>x.Date).ToList();

            return data;
        }
        public PatientRecordFullDataResponseModel GetCheckupRecordFullData(long recordId, long accountId, bool includeBills)
        {
           
            PatientRecordFullDataResponseModel data = _unitOfWork.CheckupRecordRepository.Get()
                .Include(x => x.Patient)
                .ThenInclude(x=>x.Account)
                .Include(x => x.Prescriptions)
                .ThenInclude(x => x.PrescriptionDetails)
                .Include(x => x.TestRecords)
                .Include(x=>x.Room).ThenInclude(x=>x.RoomType)
                .Where(x => x.Id == recordId).AsEnumerable().Select
                (x =>
                {
                    var _prescription = (x.Prescriptions.ToArray().Length > 0) ? x.Prescriptions.ToArray()[0] : null;
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
                        IsReExam = (bool)x.IsReExam,
                        IcdDiseaseId = x.IcdDiseaseId,
                        IcdDiseaseName = x.IcdDiseaseName,
                        PatientData = new PatientResponseModel()
                        {
                            Id = x.Patient.Id,
                            Address = x.Patient.Address,
                            Bhyt = x.Patient.Bhyt,
                            DateOfBirth = x.Patient.DateOfBirth,
                            Gender = (int)x.Patient.Gender,
                            PhoneNumber = x.Patient.PhoneNumber,
                            AccountPhoneNo = x.Patient.Account.PhoneNumber,
                            Name = x.Patient.Name,
                        },
                        PatientId = x.PatientId,
                        Prescription = (_prescription != null) ? new PrescriptionResponseModel()
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
                            }).ToList(),
                        }
                        : null,
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
                            DoctorName = tr.DoctorName,
                            QrCode = tr.QrCode
                        }).ToList(),
                        RoomId = (long)x.RoomId,
                        Floor = x.Floor,
                        RoomNumber = x.RoomNumber,
                        RoomType = x.Room.RoomType.Name,
                        QrCode = x.QrCode
                    };
                }).FirstOrDefault();

            if (includeBills && data!=null)
            {
                var billIds= _unitOfWork.BillDetailRepository
                    .Get()
                    .Where(x => (x.CheckupRecordId==null? true : x.CheckupRecordId == data.Id) &&
                    (x.TestRecordId == null ? true : 
                    data.TestRecords.Select(x => x.Id).ToList().Contains((long)x.TestRecordId)
                    ))
                    .Select(x => x.BillId)
                    .Distinct()
                    .ToList();
                var bills = _unitOfWork.BillRepository.Get()
                    .Include(x=>x.BillDetails)
                    .Include(x=>x.Patient)
                    .Where(x => billIds.Contains(x.Id)).Select(x=>new BillViewModel()
                    {
                        Id = x.Id,
                        AccountPhoneNo = x.AccountPhoneNo,
                        Details = x.BillDetails.Select(detail=>new BillDetailViewModel()
                        {
                            Id = detail.Id,
                            InsuranceStatus = (int)detail.InsuranceStatus,
                            OperationName = detail.OperationName,
                            OperationId = detail.OperationId,
                            Price = detail.Price,
                            SubTotal = detail.SubTotal,
                            Quantity = detail.Quantity,
                        }).ToList(),
                        Content = x.Content,
                        Gender = (int)x.Patient.Gender,
                        PatientName = x.PatientName,
                        Status = (int)x.Status,
                        PhoneNo = x.PhoneNo,
                        TotalInWord = x.TotalInWord,
                        Total = x.Total,
                        PatientId = (long)x.PatientId,
                        DateOfBirth =x.Patient.DateOfBirth,
                        TimeCreated = x.TimeCreated,
                        CashierName = x.CashierName,
                        CashierId   =x.CashierId,
                        BankCode = x.BankCode,
                        BankName = x.BankName,
                        BankLogoLink = x.BankLogoLink,
                        BankTranNo = x.BankTranNo,
                        CardType = x.CardType,
                        VnPayTranNo = x.VnPayTransactionNo,
                        PaymentMethod = x.PaymentMethod==null?0:(int)x.PaymentMethod,
                        TransactionStatus = x.TransactionStatus 
                    }).ToList();
                data.Bill = bills;
            }
            return data;
        }
        public async Task<AppointmenAfterBookingResponseModel> CreatReExamAppointment(long patientId, long previousCrId, DateTime date,
            long doctorId, int? numericalOrder, string clinicalSymptom,long accountId)
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
             .Where(x=>x.AccountId==accountId)
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
                            prevCr.RoomId = slot.RoomId;
                            prevCr.RoomNumber = slot.RoomNumber;
                            prevCr.Floor = slot.Floor;
                            prevCr.Date = slot.EstimatedStartTime;
                            prevCr.NumericalOrder = slot.NumericalOrder;
                            prevCr.EstimatedStartTime = slot.EstimatedStartTime;
                            prevCr.DoctorId = doctor.Id;
                            prevCr.DoctorName = doctor.Name;
                            prevCr.EstimatedDate = slot.EstimatedStartTime;
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
                var room = _numService.GetAppropriateRoomForOperation(_tr.Operation,true);
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
                _tr.Date = date.Date;
            }
            await _unitOfWork.SaveChangesAsync();
            return new AppointmenAfterBookingResponseModel()
            {
                DepartmentName = prevCr.RoomNumber,
                DoctorName = doctor.Name,
                Date = (DateTime)prevCr.Date,
                Floor = prevCr.Floor,
                NumericalOrder = (int)prevCr.NumericalOrder,
                RoomNumber = prevCr.RoomNumber,
            };
        }
        public async Task<AppointmenAfterBookingResponseModel> CreatNewAppointment(long patientId, DateTime date, long doctorId, int? numericalOrder, string clinicalSymptom, long accountId)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                //do something in here
                Console.WriteLine("Hello");
                scope.Complete();
            }
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
                .Include(x=>x.Account)
             .Where(x => x.Status == Patient.PatientStatus.HOAT_DONG)
             .Where(x => x.Id == patientId)
             .Where(x => x.AccountId == accountId)
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
            var dakhoaDep = _departmentService.GetDepartmentById(IdConfig.ID_DEPARTMENT_DA_KHOA);
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
                Date = estimatedStartTime,
                DepartmentId = IdConfig.ID_DEPARTMENT_DA_KHOA,
                DepartmentName = dakhoaDep.Name,
                DoctorId = doctorId,
                DoctorName = doctor.Name,
                ClinicalSymptom = clinicalSymptom,
                QrCode = Guid.NewGuid().ToString()
            };
            await _unitOfWork.CheckupRecordRepository.Add(cr);
            await _unitOfWork.SaveChangesAsync();
            //tạo một bill detail và 1 bill tương ứng
            Bill bill = new Bill()
            {
                Content = "Thanh toán viện phí khám tổng quát đa khoa cho bệnh nhân "+patient.Name,
                Total = dakhoaOp.Price,
                Status = Bill.BillStatus.CHUA_TT,
                TimeCreated = DateTime.Now.AddHours(7),
                PatientName = patient.Name,
                TotalInWord = MoneyHelper.NumberToText(dakhoaOp.Price),
                PatientId = patient.Id,
                PhoneNo = patient.PhoneNumber,
                AccountPhoneNo = patient.Account.PhoneNumber,
                PaymentMethod = Bill.PaymentMethodEnum.TIEN_MAT
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
            if (((DateTime)cr.EstimatedDate).Date==DateTime.Now.AddHours(7).Date)
            {
                _scheduleServiceDoctor.UpdateRedis_CheckupQueue((long)cr.RoomId);
            }
            return new AppointmenAfterBookingResponseModel()
            {
                Floor = cr.Floor,
                DoctorName = doctor.Name,
                DepartmentName = cr.DepartmentName,
                NumericalOrder = (int)cr.NumericalOrder,
                Date = (DateTime)cr.EstimatedStartTime,
                RoomNumber = cr.RoomNumber,
            };
        }
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
