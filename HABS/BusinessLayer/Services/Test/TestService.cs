using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.Services.Redis;
using DataAccessLayer.Models;
using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using static DataAccessLayer.Models.CheckupRecord;
using static DataAccessLayer.Models.Doctor;
using static DataAccessLayer.Models.Operation;

namespace BusinessLayer.Services.Test
{
    public class TestService : BaseService
    {
        private readonly Interfaces.User.IScheduleService _scheduleService;
        private readonly Interfaces.Doctor.IScheduleService _scheduleServiceDoctor;

        private readonly IDepartmentService _departmentService;
        private readonly IOperationService _operationService;
        private readonly ILogger<TestService> _logger;
        public TestService(IUnitOfWork unitOfWork, IDistributedCache distributedCache,
            Interfaces.User.IScheduleService scheduleService,
             IDepartmentService departmentService,
            Interfaces.Doctor.IScheduleService scheduleServiceDoctor,
             IOperationService operationService,
              ILogger<TestService> logger
            ) : base(unitOfWork)
        {
            _logger = logger;
            _scheduleServiceDoctor = scheduleServiceDoctor;
            _operationService = operationService;
            _scheduleService = scheduleService;
            _departmentService = departmentService;
        }
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
        public async Task<string> CreatNewAppointment(long patientId, DateTime date,
            long doctorId, int? numericalOrder, string clinicalSymptom)
        {
            _logger.LogDebug($"Toi debug from create an appointment for patient {patientId}");
            var reqSession = SessionType.SANG;
            //kiểm tra bác sĩ
            var doctor = _unitOfWork.DoctorRepository.Get()
               .Where(x => x.Id == doctorId)
               .Where(x => x.Type == DoctorType.BS_DA_KHOA)
               .FirstOrDefault();
            if (doctor == null)
            {
                throw new Exception("Doctor doesn't exist");
            }
            //kiểm tra xem thời điểm đó bs có làm việc ko
            var schedule = _unitOfWork.ScheduleRepository.Get()
                .Where(x => x.RoomId == 10001)
                .Where(x => x.Session == reqSession)
                .Where(x => x.Weekday == date.DayOfWeek)
                .Include(x => x.Room)
                .Include(x => x.Doctor)
                .FirstOrDefault();

            //kiểm tra bệnh nhân, nào bổ sung status check sau khi đã update db bổ sung status vào bảng patient
            var patient = _unitOfWork.PatientRepository.Get()
             .Where(x => x.Id == patientId)
             .FirstOrDefault();
            if (patient == null)
            {
                throw new Exception("Patient doesn't exist");
            }
            //nếu số thứ tự null thì lấy số nhỏ nhất có thể trong ngày hôm đó
            var avaiSlot = _scheduleService.GetAvailableSlots(new RequestModels.SearchModels.User.SlotSearchModel()
            {
                DoctorId = schedule.DoctorId,
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
                //không đặt trước trong hàm test
            }
            //nếu chưa có CR thì đăng kí 1 checkup record mới
            var dakhoaDep = _departmentService.GetDepartmentById(IdConfig.ID_DEPARTMENT_DA_KHOA);
            var dakhoaOp = _operationService.GetOperationForDepartment(dakhoaDep.Id);
            string newUUid = Guid.NewGuid().ToString();
            var cr = new CheckupRecord()
            {
                IsReExam = false,
                PatientId = patientId,
                PatientName = patient.Name,
                //mặc định phòng 10001 test
                RoomId = 10001,
                RoomNumber = "002",
                Floor = "1",
                //đã thanh toán luôn
                Status = CheckupRecordStatus.CHECKED_IN,
                NumericalOrder = numericalOrder,
                EstimatedDate = date,
                EstimatedStartTime = estimatedStartTime,
                DepartmentId = IdConfig.ID_DEPARTMENT_DA_KHOA,
                DepartmentName = dakhoaDep.Name,
                DoctorId = schedule.DoctorId,
                ClinicalSymptom = clinicalSymptom,
                DoctorName = doctor.Name,
                QrCode = newUUid,
            };
            await _unitOfWork.CheckupRecordRepository.Add(cr);
            await _unitOfWork.SaveChangesAsync();
            //tạo một bill detail và 1 bill tương ứng
            Bill bill = new Bill()
            {
                Content = "Thanh toán viện phí khám tổng quát đa khoa",
                //tính luôn
                Total = dakhoaOp.Price,
                Status = Bill.BillStatus.TT_TIEN_MAT,
                TimeCreated = DateTime.Now.AddHours(7),
                PatientName = patient.Name,
                TotalInWord = MoneyHelper.NumberToText(dakhoaOp.Price),
                CashierId = 10001,
                CashierName = "Nhân viên test",
                PatientId = patient.Id,
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
            _scheduleServiceDoctor.UpdateRedis_CheckupQueue((long)cr.RoomId);
            return newUUid;
        }
        public async Task<string> CreatNewAppointmentWithoutCheckin(long patientId, DateTime date,
           long doctorId, int? numericalOrder, string clinicalSymptom)
        {
            var reqSession = SessionType.SANG;
            //kiểm tra bác sĩ
            var doctor = _unitOfWork.DoctorRepository.Get()
               .Where(x => x.Id == doctorId)
               .Where(x => x.Type == DoctorType.BS_DA_KHOA)
               .FirstOrDefault();
            if (doctor == null)
            {
                throw new Exception("Doctor doesn't exist");
            }
            //kiểm tra xem thời điểm đó bs có làm việc ko
            var schedule = _unitOfWork.ScheduleRepository.Get()
                .Where(x => x.RoomId == 10001)
                .Where(x => x.Session == reqSession)
                .Where(x => x.Weekday == date.DayOfWeek)
                .Include(x => x.Room)
                .Include(x => x.Doctor)
                .FirstOrDefault();

            //kiểm tra bệnh nhân, nào bổ sung status check sau khi đã update db bổ sung status vào bảng patient
            var patient = _unitOfWork.PatientRepository.Get()
             .Where(x => x.Id == patientId)
             .FirstOrDefault();
            if (patient == null)
            {
                throw new Exception("Patient doesn't exist");
            }
            //nếu số thứ tự null thì lấy số nhỏ nhất có thể trong ngày hôm đó
            var avaiSlot = _scheduleService.GetAvailableSlots(new RequestModels.SearchModels.User.SlotSearchModel()
            {
                DoctorId = schedule.DoctorId,
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
                //không đặt trước trong hàm test
            }
            //nếu chưa có CR thì đăng kí 1 checkup record mới
            var dakhoaDep = _departmentService.GetDepartmentById(IdConfig.ID_DEPARTMENT_DA_KHOA);
            var dakhoaOp = _operationService.GetOperationForDepartment(dakhoaDep.Id);
            string newUUid = Guid.NewGuid().ToString();
            var cr = new CheckupRecord()
            {
                IsReExam = false,
                PatientId = patientId,
                PatientName = patient.Name,
                //mặc định phòng 10001 test
                RoomId = 10001,
                RoomNumber = "002",
                Floor = "1",
                //đã thanh toán luôn
                Status = CheckupRecordStatus.DA_THANH_TOAN,
                NumericalOrder = numericalOrder,
                EstimatedDate = date,
                EstimatedStartTime = estimatedStartTime,
                DepartmentId = IdConfig.ID_DEPARTMENT_DA_KHOA,
                DepartmentName = dakhoaDep.Name,
                DoctorId = schedule.DoctorId,
                ClinicalSymptom = clinicalSymptom,
                DoctorName = doctor.Name,
                QrCode = newUUid,
            };
            await _unitOfWork.CheckupRecordRepository.Add(cr);
            await _unitOfWork.SaveChangesAsync();
            //tạo một bill detail và 1 bill tương ứng
            Bill bill = new Bill()
            {
                Content = "Thanh toán viện phí khám tổng quát đa khoa",
                //tính luôn
                Total = dakhoaOp.Price,
                Status = Bill.BillStatus.TT_TIEN_MAT,
                TimeCreated = DateTime.Now.AddHours(7),
                PatientName = patient.Name,
                TotalInWord = MoneyHelper.NumberToText(dakhoaOp.Price),
                CashierId = 10001,
                CashierName = "Nhân viên test",
                PatientId = patient.Id,
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
            _scheduleServiceDoctor.UpdateRedis_CheckupQueue((long)cr.RoomId);
            return newUUid;
        }
        public async Task<string> CreatNewAppointmentWithPreviousTest(long patientId, DateTime date,
             long doctorId, int? numericalOrder, string clinicalSymptom)
        {
            string newUUid = Guid.NewGuid().ToString();
            var reqSession = SessionType.SANG;
            //kiểm tra bác sĩ
            var doctor = _unitOfWork.DoctorRepository.Get()
               .Where(x => x.Id == doctorId)
               .Where(x => x.Type == DoctorType.BS_DA_KHOA)
               .FirstOrDefault();
            if (doctor == null)
            {
                throw new Exception("Doctor doesn't exist");
            }
            //kiểm tra xem thời điểm đó bs có làm việc ko
            var schedule = _unitOfWork.ScheduleRepository.Get()
                .Where(x => x.RoomId == 10001)
                .Where(x => x.Session == reqSession)
                .Where(x => x.Weekday == date.DayOfWeek)
                .Include(x => x.Room)
                .Include(x => x.Doctor)
                .FirstOrDefault();

            //kiểm tra bệnh nhân, nào bổ sung status check sau khi đã update db bổ sung status vào bảng patient
            var patient = _unitOfWork.PatientRepository.Get()
             .Where(x => x.Id == patientId)
             .FirstOrDefault();
            if (patient == null)
            {
                throw new Exception("Patient doesn't exist");
            }
            //nếu số thứ tự null thì lấy số nhỏ nhất có thể trong ngày hôm đó
            var avaiSlot = _scheduleService.GetAvailableSlots(new RequestModels.SearchModels.User.SlotSearchModel()
            {
                DoctorId = schedule.DoctorId,
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
                //không đặt trước trong hàm test
            }
            //nếu chưa có CR thì đăng kí 1 checkup record mới
            var dakhoaDep = _departmentService.GetDepartmentById(IdConfig.ID_DEPARTMENT_DA_KHOA);
            var dakhoaOp = _operationService.GetOperationForDepartment(dakhoaDep.Id);
            var cr = new CheckupRecord()
            {
                QrCode = newUUid,
                IsReExam = false,
                PatientId = patientId,
                PatientName = patient.Name,
                //mặc định phòng 10001 test
                RoomId = 10001,
                RoomNumber = "002",
                Floor = "1",
                //đã thanh toán luôn
                Status = CheckupRecordStatus.CHO_KQXN,
                NumericalOrder = numericalOrder,
                EstimatedDate = date,
                EstimatedStartTime = estimatedStartTime,
                DepartmentId = IdConfig.ID_DEPARTMENT_DA_KHOA,
                DepartmentName = dakhoaDep.Name,
                DoctorId = schedule.DoctorId,
                ClinicalSymptom = clinicalSymptom,
                DoctorName = doctor.Name,
            };
            await _unitOfWork.CheckupRecordRepository.Add(cr);
            await _unitOfWork.SaveChangesAsync();
            //tạo một bill detail và 1 bill tương ứng
            //Bill bill = new Bill()
            //{
            //    Content = "Thanh toán viện phí khám tổng quát đa khoa",
            //    //tính luôn
            //    Total = dakhoaOp.Price,
            //    Status = Bill.BillStatus.CHUA_TT,
            //    TimeCreated = DateTime.Now.AddHours(7),
            //    PatientName = patient.Name,
            //    TotalInWord = MoneyHelper.NumberToText(dakhoaOp.Price),
            //    CashierId = 10001,
            //    CashierName = "Nhân viên test",
            //    PatientId = patient.Id,
            //};
            //await _unitOfWork.BillRepository.Add(bill);
            //await _unitOfWork.SaveChangesAsync();
            //BillDetail bd = new BillDetail()
            //{
            //    Price = dakhoaOp.Price,
            //    OperationId = dakhoaOp.Id,
            //    InsuranceStatus = (InsuranceSupportStatus)dakhoaOp.InsuranceStatus,
            //    OperationName = dakhoaOp.Name,
            //    Quantity = 1,
            //    SubTotal = dakhoaOp.Price,
            //    CheckupRecordId = cr.Id,
            //    BillId = bill.Id
            //};
            //await _unitOfWork.BillDetailRepository.Add(bd);
            TestRecord tr = new TestRecord()
            {
                QrCode = Guid.NewGuid().ToString(),
                RoomId = 10008,
                EstimatedDate = DateTime.Now.AddYears(-2),
                Date = DateTime.Now.AddYears(-2),
                Floor = "23",
                NumericalOrder = 9999,
                OperationId = 10010,
                OperationName = "Xét nghiệm máu",
                PatientId = patient.Id,
                PatientName = patient.Name,
                CheckupRecordId = cr.Id,
                Status = TestRecord.TestRecordStatus.DA_THANH_TOAN,
            };
            TestRecord tr2 = new TestRecord()
            {
                QrCode = Guid.NewGuid().ToString(),
                RoomId = 10008,
                EstimatedDate = DateTime.Now.AddYears(-2),
                Date = DateTime.Now.AddYears(-2),
                Floor = "23",
                NumericalOrder = 9999,
                OperationId = 10011,
                OperationName = "Xét nghiệm mỡ trong máu",
                PatientId = patient.Id,
                PatientName = patient.Name,
                CheckupRecordId = cr.Id,
                Status = TestRecord.TestRecordStatus.DA_THANH_TOAN,
            };
            await _unitOfWork.TestRecordRepository.Add(tr2);
            await _unitOfWork.TestRecordRepository.Add(tr);
            await _unitOfWork.SaveChangesAsync();
            _scheduleServiceDoctor.UpdateRedis_CheckupQueue((long)cr.RoomId);
            _scheduleServiceDoctor.UpdateRedis_TestQueue(10007, false);
            return newUUid;
        }
        public async Task<string> CreatNewAppointmentWithPreviousTestFinished(long patientId, DateTime date,
            long doctorId, int? numericalOrder, string clinicalSymptom)
        {
            string newUUid = Guid.NewGuid().ToString();
            var reqSession = SessionType.SANG;
            //kiểm tra bác sĩ
            var doctor = _unitOfWork.DoctorRepository.Get()
               .Where(x => x.Id == doctorId)
               .Where(x => x.Type == DoctorType.BS_DA_KHOA)
               .FirstOrDefault();
            if (doctor == null)
            {
                throw new Exception("Doctor doesn't exist");
            }
            //kiểm tra xem thời điểm đó bs có làm việc ko
            var schedule = _unitOfWork.ScheduleRepository.Get()
                .Where(x => x.RoomId == 10001)
                .Where(x => x.Session == reqSession)
                .Where(x => x.Weekday == date.DayOfWeek)
                .Include(x => x.Room)
                .Include(x => x.Doctor)
                .FirstOrDefault();

            //kiểm tra bệnh nhân, nào bổ sung status check sau khi đã update db bổ sung status vào bảng patient
            var patient = _unitOfWork.PatientRepository.Get()
             .Where(x => x.Id == patientId)
             .FirstOrDefault();
            if (patient == null)
            {
                throw new Exception("Patient doesn't exist");
            }
            //nếu số thứ tự null thì lấy số nhỏ nhất có thể trong ngày hôm đó
            var avaiSlot = _scheduleService.GetAvailableSlots(new RequestModels.SearchModels.User.SlotSearchModel()
            {
                DoctorId = schedule.DoctorId,
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
                //không đặt trước trong hàm test
            }
            //nếu chưa có CR thì đăng kí 1 checkup record mới
            var dakhoaDep = _departmentService.GetDepartmentById(IdConfig.ID_DEPARTMENT_DA_KHOA);
            var dakhoaOp = _operationService.GetOperationForDepartment(dakhoaDep.Id);
            var cr = new CheckupRecord()
            {
                QrCode = newUUid,
                IsReExam = false,
                PatientId = patientId,
                PatientName = patient.Name,
                //mặc định phòng 10001 test
                RoomId = 10001,
                RoomNumber = "002",
                Floor = "1",
                //đã thanh toán luôn
                Status = CheckupRecordStatus.DA_CO_KQXN,
                NumericalOrder = numericalOrder,
                EstimatedDate = date,
                EstimatedStartTime = estimatedStartTime,
                DepartmentId = IdConfig.ID_DEPARTMENT_DA_KHOA,
                DepartmentName = dakhoaDep.Name,
                DoctorId = schedule.DoctorId,
                ClinicalSymptom = clinicalSymptom,
                DoctorName = doctor.Name,
            };
            await _unitOfWork.CheckupRecordRepository.Add(cr);
            await _unitOfWork.SaveChangesAsync();
            //tạo một bill detail và 1 bill tương ứng
            //Bill bill = new Bill()
            //{
            //    Content = "Thanh toán viện phí khám tổng quát đa khoa",
            //    //tính luôn
            //    Total = dakhoaOp.Price,
            //    Status = Bill.BillStatus.CHUA_TT,
            //    TimeCreated = DateTime.Now.AddHours(7),
            //    PatientName = patient.Name,
            //    TotalInWord = MoneyHelper.NumberToText(dakhoaOp.Price),
            //    CashierId = 10001,
            //    CashierName = "Nhân viên test",
            //    PatientId = patient.Id,
            //};
            //await _unitOfWork.BillRepository.Add(bill);
            //await _unitOfWork.SaveChangesAsync();
            //BillDetail bd = new BillDetail()
            //{
            //    Price = dakhoaOp.Price,
            //    OperationId = dakhoaOp.Id,
            //    InsuranceStatus = (InsuranceSupportStatus)dakhoaOp.InsuranceStatus,
            //    OperationName = dakhoaOp.Name,
            //    Quantity = 1,
            //    SubTotal = dakhoaOp.Price,
            //    CheckupRecordId = cr.Id,
            //    BillId = bill.Id
            //};
            //await _unitOfWork.BillDetailRepository.Add(bd);
            TestRecord tr = new TestRecord()
            {
                QrCode = Guid.NewGuid().ToString(),
                RoomId = 10008,
                EstimatedDate = DateTime.Now.AddYears(-2),
                Date = DateTime.Now.AddYears(-2),
                Floor = "23",
                NumericalOrder = 9999,
                OperationId = 10010,
                OperationName = "Xét nghiệm máu",
                PatientId = patient.Id,
                PatientName = patient.Name,
                CheckupRecordId = cr.Id,
                Status = TestRecord.TestRecordStatus.HOAN_THANH,
            };
            TestRecord tr2 = new TestRecord()
            {
                QrCode = Guid.NewGuid().ToString(),
                RoomId = 10008,
                EstimatedDate = DateTime.Now.AddYears(-2),
                Date = DateTime.Now.AddYears(-2),
                Floor = "23",
                NumericalOrder = 9999,
                OperationId = 10011,
                OperationName = "Xét nghiệm mỡ trong máu",
                PatientId = patient.Id,
                PatientName = patient.Name,
                CheckupRecordId = cr.Id,
                Status = TestRecord.TestRecordStatus.HOAN_THANH,
            };
            await _unitOfWork.TestRecordRepository.Add(tr2);
            await _unitOfWork.TestRecordRepository.Add(tr);
            await _unitOfWork.SaveChangesAsync();
            _scheduleServiceDoctor.UpdateRedis_CheckupQueue((long)cr.RoomId);
            _scheduleServiceDoctor.UpdateRedis_TestQueue(10007, false);
            return newUUid;
        }
        public async Task RemoveAllPatientThatDay(long roomId)
        {
            var crList = _unitOfWork.CheckupRecordRepository.Get()
                .Where(x => ((DateTime)x.Date).Date == DateTime.Now.Date || ((DateTime)x.EstimatedDate).Date == DateTime.Now.Date
               )
                .ToList();
            foreach (var item in crList)
            {
                item.Status = CheckupRecordStatus.DA_XOA;
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task RemoveAllBill()
        {
            var billList = _unitOfWork.BillRepository.Get()
                .ToList();
            foreach (var item in billList)
            {
                item.Status = Bill.BillStatus.HUY;
            }
            var crList = _unitOfWork.CheckupRecordRepository.Get()
                .ToList();
            foreach (var item in crList)
            {
                item.Status = CheckupRecordStatus.DA_XOA;
            }
            var trList = _unitOfWork.TestRecordRepository.Get()
               .ToList();
            foreach (var item in trList)
            {
                item.Status = TestRecord.TestRecordStatus.DA_XOA;
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task CreateAHistory(long patientId, DateTime date,
          long doctorId, int? numericalOrder, string clinicalSymptom, long departmentId)
        {
            var reqSession = SessionType.SANG;
            //kiểm tra bác sĩ
            var doctor = _unitOfWork.DoctorRepository.Get()
               .Where(x => x.Id == doctorId)
               .Where(x => x.Type == DoctorType.BS_DA_KHOA)
               .FirstOrDefault();
            if (doctor == null)
            {
                throw new Exception("Doctor doesn't exist");
            }
            //kiểm tra xem thời điểm đó bs có làm việc ko
            var schedule = _unitOfWork.ScheduleRepository.Get()
                .Where(x => x.RoomId == 10001)
                .Where(x => x.Session == reqSession)
                .Where(x => x.Weekday == date.DayOfWeek)
                .Include(x => x.Room)
                .Include(x => x.Doctor)
                .FirstOrDefault();

            //kiểm tra bệnh nhân, nào bổ sung status check sau khi đã update db bổ sung status vào bảng patient
            var patient = _unitOfWork.PatientRepository.Get()
             .Where(x => x.Id == patientId)
             .FirstOrDefault();
            if (patient == null)
            {
                throw new Exception("Patient doesn't exist");
            }
            //nếu số thứ tự null thì lấy số nhỏ nhất có thể trong ngày hôm đó
            //nếu chưa có CR thì đăng kí 1 checkup record mới
            var dep = _departmentService.GetDepartmentById(departmentId);
            var dakhoaOp = _operationService.GetOperationForDepartment(dep.Id);
            var cr = new CheckupRecord()
            {
                QrCode = Guid.NewGuid().ToString(),
                Date = date,
                Diagnosis = "Bệnh nhân này có triệu chứng bất thường.",
                BloodPressure = 42,
                DoctorAdvice = "Về ngâm nước muối trong 23h",
                IcdDiseaseId = 10004,
                IcdDiseaseCode = "A09",
                IcdDiseaseName = "Tiêu chảy, viêm dạ dày - ruột",
                //nhớ cho đơn thuốc
                Temperature = 23,
                Pulse=231,
                IsReExam = false,
                PatientId = patientId,
                PatientName = patient.Name,
                //mặc định phòng 10001 test
                RoomId = 10001,
                RoomNumber = "002",
                Floor = "1",
                //đã thanh toán luôn
                Status = CheckupRecordStatus.KET_THUC,
                NumericalOrder = numericalOrder,
                EstimatedDate = date,
                EstimatedStartTime = DateTime.Now,
                DepartmentId = departmentId,
                DepartmentName = dep.Name,
                DoctorId =10005,
                ClinicalSymptom = clinicalSymptom,
                DoctorName = "Ngô Trần Thanh Long",
            };
            
            await _unitOfWork.CheckupRecordRepository.Add(cr);
            await _unitOfWork.SaveChangesAsync();
            var pr = new Prescription()
            {
                CheckupRecordId = cr.Id,
                Note = "Về không được ăn chiều ăn xế gì hết",
                TimeCreated = DateTime.Now,
            };
            await _unitOfWork.PrescriptionRepository.Add(pr);
            await _unitOfWork.SaveChangesAsync();
            var prdt1 = new PrescriptionDetail()
            {
                EveningDose = 2,
                MiddayDose = 2,
                MorningDose = 4,
                MedicineId = 10003,
                MedicineName = "Aminoglycoside",
                NightDose = 3,
                PrescriptionId = pr.Id,
                Quantity = 23,
                Unit = "Lọ",
                Usage = "Bỏ vô miệng nhai nhai",
            };
            var prdt2 = new PrescriptionDetail()
            {
                EveningDose = 2,
                MiddayDose = 2,
                MorningDose = 4,
                MedicineId = 10004,
                MedicineName = "Promethazine",
                NightDose = 3,
                PrescriptionId = pr.Id,
                Quantity = 23,
                Unit = "Lọ",
                Usage = "Uống không mở nắp",
            };
            await _unitOfWork.PrescriptionDetailRepository.Add(prdt1);
            await _unitOfWork.PrescriptionDetailRepository.Add(prdt2);
            await _unitOfWork.SaveChangesAsync();
            Bill bill = new Bill()
            {
                Content = "Thanh toán viện phí khám tổng quát đa khoa",
                Total = dakhoaOp.Price,
                Status = Bill.BillStatus.TT_TIEN_MAT,
                TimeCreated = DateTime.Now.AddHours(7),
                PatientName = patient.Name,
                TotalInWord = MoneyHelper.NumberToText(dakhoaOp.Price),
                CashierId = 10001,
                CashierName = "Nhân viên test",
                PatientId = patient.Id,
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
            TestRecord tr = new TestRecord()
            {
                QrCode = Guid.NewGuid().ToString(),
                RoomId = 10007,
                EstimatedDate = DateTime.Now.AddHours(7),
                Date = DateTime.Now.AddHours(7),
                Floor = "23",
                RoomNumber = "0012",
                NumericalOrder = 9999,
                OperationId = 10010,
                OperationName = "Xét nghiệm máu",
                PatientId = patient.Id,
                PatientName = patient.Name,
                DoctorId = 10014,
                DoctorName = "Thành Phước Tâm",
                CheckupRecordId = cr.Id,
                Status = TestRecord.TestRecordStatus.HOAN_THANH,
                ResultFileLink = "https://firebasestorage.googleapis.com/v0/b/hospitalmanagement-42da9.appspot.com/o/test-result%2Fpatient-10000%2Fresult-10034-1656324975418.pdf?alt=media&token=ac9092ba-174f-40e0-b3f1-3bbeaa5eb723"
            };
            TestRecord tr2 = new TestRecord()
            {
                QrCode = Guid.NewGuid().ToString(),
                DoctorId = 10014,
                RoomNumber = "0012",
                DoctorName = "Thành Phước Tâm",
                RoomId = 10007,
                EstimatedDate = DateTime.Now.AddHours(7),
                Date = DateTime.Now.AddHours(7),
                Floor = "23",
                NumericalOrder = 9999,
                OperationId = 10011,
                OperationName = "Xét nghiệm mỡ trong máu",
                PatientId = patient.Id,
                PatientName = patient.Name,
                CheckupRecordId = cr.Id,
                Status = TestRecord.TestRecordStatus.HOAN_THANH,
                ResultFileLink = "https://firebasestorage.googleapis.com/v0/b/hospitalmanagement-42da9.appspot.com/o/test-result%2Fpatient-10000%2Fresult-10034-1656324975418.pdf?alt=media&token=ac9092ba-174f-40e0-b3f1-3bbeaa5eb723"
            };
            await _unitOfWork.TestRecordRepository.Add(tr2);
            await _unitOfWork.TestRecordRepository.Add(tr);
            await _unitOfWork.SaveChangesAsync();
            _scheduleServiceDoctor.UpdateRedis_CheckupQueue((long)cr.RoomId);
            _scheduleServiceDoctor.UpdateRedis_TestQueue(10007, false);

        }
    }
}
