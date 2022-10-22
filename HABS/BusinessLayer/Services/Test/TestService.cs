using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Common;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.ResponseModels.SearchModels.User;
using BusinessLayer.Services.Redis;
using DataAccessLayer.Models;
using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly INumercialOrderService _numService;
        private readonly ILogger<TestService> _logger;
        private readonly RedisService _redisService;
        //config
        private readonly BaseConfig _baseConfig;
        public TestService(IUnitOfWork unitOfWork, IDistributedCache distributedCache,
            Interfaces.User.IScheduleService scheduleService,
             IDepartmentService departmentService,
            Interfaces.Doctor.IScheduleService scheduleServiceDoctor,
             IOperationService operationService,
            INumercialOrderService numService,
             BaseConfig workingShiftConfig,
              ILogger<TestService> logger
            ) : base(unitOfWork)
        {
            _baseConfig = workingShiftConfig;
            _redisService = new RedisService(distributedCache);
            _logger = logger;
            _numService = numService;
            _scheduleServiceDoctor = scheduleServiceDoctor;
            _operationService = operationService;
            _scheduleService = scheduleService;
            _departmentService = departmentService;

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
        public async Task CreatALotOfNewAppointments(int excludeAtTop, int examPending, int checkupFinished)
        {
            var roomDaKhoaList = new List<long> { 10001, 10002 };
            var scheduleForRoomList = new List<Schedule>();
            foreach (var room in roomDaKhoaList)
            {
                var now = DateTime.Now.Date;
                //tối ưu thành một câu query
                var scheduleForRoom = _unitOfWork.ScheduleRepository
                    .Get()
                     .Where(x => x.RoomId == room)
                     .Where(x => x.Session == SessionType.SANG || x.Session == SessionType.CHIEU || x.Session == SessionType.TOI)
                     .Where(x => x.Weekday == now.DayOfWeek)
                     .Include(x => x.Room)
                     .Include(x => x.Doctor)
                     .FirstOrDefault();
                scheduleForRoomList.Add(scheduleForRoom);
            }
            //Slot trống mỗi phòng
            var avaiSlotsList = new List<CheckupSlotResponseModel>();
            var slotsForExamPendingList = new List<CheckupSlotResponseModel>();
            var slotsForExamFinishedList = new List<CheckupSlotResponseModel>();

            foreach (var scheduleForRoom in scheduleForRoomList)
            {
                var avaiSlots = _scheduleService.GetAvailableSlots(new RequestModels.SearchModels.User.SlotSearchModel()
                {
                    DoctorId = scheduleForRoom.DoctorId,
                    Date = DateTime.Now
                });
                //skip bệnh nhân ở đầu để chừa ra khám
                avaiSlots = avaiSlots.Skip(excludeAtTop).ToList();
                //thêm doctor (script purpose only)
                foreach (var slot in avaiSlots)
                {
                    slot.Doctor = scheduleForRoom.Doctor.Name;
                    slot.DoctorId = scheduleForRoom.Doctor.Id;
                }

                avaiSlotsList.AddRange(avaiSlots.SkipLast(examPending + checkupFinished));
                slotsForExamPendingList.AddRange(avaiSlots.TakeLast(examPending + checkupFinished).Take(examPending));
                slotsForExamFinishedList.AddRange(avaiSlots.TakeLast(checkupFinished));
            }

            //Lấy hết bệnh nhân test
            var patienList = _unitOfWork.PatientRepository.Get().Where(x => x.IsTestPatient == true).ToList();
            var currentPatientIndex = 0;
            foreach (var slot in avaiSlotsList)
            {
                //tạo CR tương ứng
                var patient = patienList[currentPatientIndex++];
                var cr = new CheckupRecord()
                {
                    IsReExam = false,
                    PatientId = patient.Id,
                    PatientName = patient.Name,
                    RoomId = slot.RoomId,
                    RoomNumber = slot.RoomNumber,
                    Floor = slot.Floor,
                    //đã thanh toán và checked in
                    Status = CheckupRecordStatus.CHECKED_IN,
                    NumericalOrder = slot.NumericalOrder,
                    EstimatedDate = slot.EstimatedStartTime?.Date,
                    EstimatedStartTime = slot.EstimatedStartTime,
                    DepartmentId = IdConfig.ID_DEPARTMENT_DA_KHOA,
                    DepartmentName = "Đa khoa",
                    DoctorId = slot.DoctorId,
                    ClinicalSymptom = "Triệu chứng được tạo tự động cho bệnh nhân " + patient.Name,
                    DoctorName = slot.Doctor,
                    QrCode = Guid.NewGuid().ToString(),
                };
                await _unitOfWork.CheckupRecordRepository.Add(cr);
                var examPendingCR = new List<CheckupRecord>();
            }
            var examPendingCRs = new List<CheckupRecord>();
            foreach (var slot in slotsForExamPendingList)
            {
                var patient = patienList[currentPatientIndex++];
                var cr = new CheckupRecord()
                {
                    IsReExam = false,
                    PatientId = patient.Id,
                    PatientName = patient.Name,
                    RoomId = slot.RoomId,
                    RoomNumber = slot.RoomNumber,
                    Floor = slot.Floor,
                    //đã thanh toán và checked in
                    Status = CheckupRecordStatus.CHO_KQXN,
                    NumericalOrder = slot.NumericalOrder,
                    EstimatedDate = slot.EstimatedStartTime?.Date,
                    EstimatedStartTime = slot.EstimatedStartTime,
                    DepartmentId = IdConfig.ID_DEPARTMENT_DA_KHOA,
                    DepartmentName = "Đa khoa",
                    DoctorId = slot.DoctorId,
                    ClinicalSymptom = "Triệu chứng được tạo tự động cho bệnh nhân " + patient.Name,
                    DoctorName = slot.Doctor,
                    QrCode = Guid.NewGuid().ToString(),
                };

                await _unitOfWork.CheckupRecordRepository.Add(cr);
                examPendingCRs.Add(cr);
            }
            var examFinishedCR = new List<CheckupRecord>();
            foreach (var slot in slotsForExamFinishedList)
            {
                var patient = patienList[currentPatientIndex++];
                var cr = new CheckupRecord()
                {
                    IsReExam = false,
                    PatientId = patient.Id,
                    PatientName = patient.Name,
                    RoomId = slot.RoomId,
                    RoomNumber = slot.RoomNumber,
                    Floor = slot.Floor,
                    Status = CheckupRecordStatus.KET_THUC,
                    NumericalOrder = slot.NumericalOrder,
                    EstimatedDate = slot.EstimatedStartTime?.Date,
                    EstimatedStartTime = slot.EstimatedStartTime,
                    DepartmentId = IdConfig.ID_DEPARTMENT_DA_KHOA,
                    DepartmentName = "Đa khoa",
                    DoctorId = slot.DoctorId,
                    ClinicalSymptom = "Triệu chứng được tạo tự động cho bệnh nhân " + patient.Name,
                    DoctorName = slot.Doctor,
                    QrCode = Guid.NewGuid().ToString(),
                    Pulse = 133,
                    Diagnosis = "Chẩn đoán từ bác sĩ (tạo tự động)",
                    BloodPressure = 45,
                    Temperature = 35,
                    DoctorAdvice = "Lời khuyên từ bác sĩ (tạo tự động)",
                    IcdDiseaseCode = "M06",
                    IcdDiseaseId = 10034,
                    IcdDiseaseName = "Viêm khớp dạng thấp khác",
                };
                await _unitOfWork.CheckupRecordRepository.Add(cr);
                examFinishedCR.Add(cr);
            }
            await _unitOfWork.SaveChangesAsync();
            //tạo test record cho nhóm exam pending

            //Khám sơ sinh sanh non
            //Xét nghiệm máu
            //Mổ ruột thừa
            //Chụp X - quang phổi
            var operationIdList = new List<long>() { 10010, 10011, 10016, 10013 };
            var operationList = _unitOfWork.OperationRepository.Get().Where(x => operationIdList.Contains(x.Id)).ToList();
            var roomTypeIdList = new List<long>();
            foreach (var op in operationList)
            {
                if (!roomTypeIdList.Contains((long)op.RoomTypeId))
                {
                    roomTypeIdList.Add((long)op.RoomTypeId);
                }
            }
            var roomList = _unitOfWork.RoomRepository
                   .Get()
                   .Include(x => x.TestRecords)
                   .Where(x => x.RoomTypeId != null && roomTypeIdList.Contains((long)x.RoomTypeId))
                   .ToList();
            var roomDictionary = new Dictionary<long, List<Room>>();//map roomTypeId and roomList
            foreach (var room in roomList)
            {
                List<Room> listRoom;
                var key = (long)room.RoomTypeId;
                if (!roomDictionary.TryGetValue(key, out listRoom))
                {
                    listRoom = new List<Room>() { room };
                    roomDictionary.Add(key, listRoom);
                }
                else
                {
                    listRoom.Add(room);
                }
            }
            foreach (var cr in examPendingCRs)
            {
                foreach (var op in operationList)
                {
                    roomDictionary.TryGetValue((long)op.RoomTypeId, out var roomListForThisOp);
                    var roomWithLeastRecord = roomListForThisOp.
                    //    .Select(x => new
                    //{
                    //    Room = x,
                    //    Count = x.TestRecords
                    //    .Where(x => x.Status == TestRecord.TestRecordStatus.DA_DAT_LICH ||
                    //    x.Status == TestRecord.TestRecordStatus.DANG_TIEN_HANH ||
                    //    x.Status == TestRecord.TestRecordStatus.CHECKED_IN
                    //    ).Where(x => x.EstimatedDate?.Date == DateTime.Now.Date).Count()
                    //}).OrderBy(x => x.Count).Select(x=> x.Room).First();
                        OrderBy<Room, int>(x =>
                        {
                            return x.TestRecords
                             .Where(tr => tr.Status == TestRecord.TestRecordStatus.DA_DAT_LICH ||
                                         tr.Status == TestRecord.TestRecordStatus.DANG_TIEN_HANH ||
                                         tr.Status == TestRecord.TestRecordStatus.CHECKED_IN
                             ).Where(tr => tr.EstimatedDate?.Date == DateTime.Now.Date).Count();
                        }).First();
                    var tr = new TestRecord()
                    {
                        Date = DateTime.Now.AddHours(7),
                        EstimatedDate = DateTime.Now.AddHours(7),
                        Floor = roomWithLeastRecord.Floor,
                        RoomId = roomWithLeastRecord.Id,
                        RoomNumber = roomWithLeastRecord.RoomNumber,
                        OperationId = op.Id,
                        OperationName = op.Name,
                        PatientId = cr.PatientId,
                        PatientName = cr.PatientName,
                        QrCode = Guid.NewGuid().ToString(),
                        Status = TestRecord.TestRecordStatus.CHECKED_IN,
                        CheckupRecordId = cr.Id,
                        NumericalOrder = roomWithLeastRecord.TestRecords
                             .Where(x => x.Status == TestRecord.TestRecordStatus.DA_DAT_LICH ||
                                         x.Status == TestRecord.TestRecordStatus.DANG_TIEN_HANH ||
                                         x.Status == TestRecord.TestRecordStatus.CHECKED_IN
                             ).Where(x => x.EstimatedDate?.Date == DateTime.Now.Date).Count() + 1,
                    };
                    roomWithLeastRecord.TestRecords.Add(tr);//just for on-memory calculation
                    await _unitOfWork.TestRecordRepository.Add(tr);
                }
            }
            //tạo testrecord đã hoàn thành cho nhóm finished
            foreach (var cr in examFinishedCR)
            {
                var tr = new TestRecord()
                {
                    DoctorId = 10020,
                    DoctorName = "Hảo Nguyên Dương",
                    QrCode = Guid.NewGuid().ToString(),
                    CheckupRecordId = cr.Id,
                    Date = DateTime.Now.AddHours(7),
                    EstimatedDate = DateTime.Now.AddHours(7),
                    NumericalOrder = 99,
                    OperationId = 10013,
                    OperationName = "Mổ ruột thừa",
                    PatientId = cr.PatientId,
                    PatientName = cr.PatientName,
                    ResultFileLink = "https://firebasestorage.googleapis.com/v0/b/hospitalmanagement-42da9.appspot.com/o/test-result%2Fpatient-10000%2Fresult-10034-1656324975418.pdf?alt=media&token=ac9092ba-174f-40e0-b3f1-3bbeaa5eb723",
                    ResultDescription = "Kết quả tổng quát (được tạo tự động)",
                    RoomId = 10006,
                    RoomNumber = "113",
                    Floor = "3",
                };
                await _unitOfWork.TestRecordRepository.Add(tr);
            }
            //tạo prescription cho nhóm finished
            var prescriptionDictionary = new Dictionary<long, Prescription>();
            foreach (var cr in examFinishedCR)
            {
                var pr = new Prescription()
                {
                    CheckupRecordId = cr.Id,
                    Note = "Ghi chú cho đon thuốc (được tạo tự động)",
                    TimeCreated = DateTime.Now.AddHours(7),
                };
                prescriptionDictionary.Add(cr.Id, pr);
                await _unitOfWork.PrescriptionRepository.Add(pr);
            }
            await _unitOfWork.SaveChangesAsync();
            foreach (var keyvalue in prescriptionDictionary)
            {
                //tạo thuốc
                var pd = new PrescriptionDetail()
                {
                    PrescriptionId = keyvalue.Value?.Id,
                    MedicineId = 10004,
                    MedicineName = "Promethazine",
                    MiddayDose = 5,
                    MorningDose = 5,
                    EveningDose = 6,
                    NightDose = 4,
                    Quantity = 12,
                    Unit = "Chai",
                    Usage = "Uống không cần mở nắp",
                };
                var pd2 = new PrescriptionDetail()
                {
                    PrescriptionId = keyvalue.Value?.Id,
                    MedicineId = 10005,
                    MedicineName = "Loratadine",
                    MiddayDose = 5,
                    MorningDose = 5,
                    EveningDose = 6,
                    NightDose = 4,
                    Quantity = 12,
                    Unit = "Hộp",
                    Usage = "Uống không cần mở nắp",
                };
                var pd3 = new PrescriptionDetail()
                {
                    PrescriptionId = keyvalue.Value?.Id,
                    MedicineId = 10009,
                    MedicineName = "Paracetamol",
                    MiddayDose = 5,
                    MorningDose = 5,
                    EveningDose = 6,
                    NightDose = 4,
                    Quantity = 12,
                    Unit = "Lọ",
                    Usage = "Uống phải mở nắp 3 lần",
                };
                await _unitOfWork.PrescriptionDetailRepository.Add(pd);
                await _unitOfWork.PrescriptionDetailRepository.Add(pd2); await _unitOfWork.PrescriptionDetailRepository.Add(pd);
                await _unitOfWork.PrescriptionDetailRepository.Add(pd3);
            }
            await _unitOfWork.SaveChangesAsync();
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
                RoomNumber = "001",
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
                Status = Bill.BillStatus.DA_TT_TIEN_MAT,
                TimeCreated = DateTime.Now.AddHours(7),
                PatientName = patient.Name,
                TotalInWord = MoneyHelper.NumberToText(dakhoaOp.Price),
                CashierId = 10001,
                CashierName = "Nhân viên test",
                PatientId = patient.Id,
                QrCode = Guid.NewGuid().ToString()
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
                Status = Bill.BillStatus.DA_TT_TIEN_MAT,
                TimeCreated = DateTime.Now.AddHours(7),
                PatientName = patient.Name,
                TotalInWord = MoneyHelper.NumberToText(dakhoaOp.Price),
                CashierId = 10001,
                CashierName = "Nhân viên test",
                PatientId = patient.Id,
                QrCode = Guid.NewGuid().ToString()
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
                RoomNumber = "001",
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
            var operation = _unitOfWork.OperationRepository
                .Get()
                .Where(x => x.Id == 10010)
                .FirstOrDefault();
            var room = _unitOfWork.RoomRepository
               .Get()
               .Where(x => x.Id == 10008)
               .FirstOrDefault();
            var numOrd = _numService.GetNumOrderForAutoIncreaseRoom(room, DateTime.Now.AddHours(7));
            TestRecord tr = new TestRecord()
            {
                QrCode = Guid.NewGuid().ToString(),
                RoomId = 10008,
                EstimatedDate = DateTime.Now,
                Date = DateTime.Now,
                Floor = "23",
                NumericalOrder = numOrd,
                OperationId = 10010,
                OperationName = "Xét nghiệm máu",
                PatientId = patient.Id,
                PatientName = patient.Name,
                CheckupRecordId = cr.Id,
                Status = TestRecord.TestRecordStatus.CHECKED_IN,
            };
            TestRecord tr2 = new TestRecord()
            {
                QrCode = Guid.NewGuid().ToString(),
                RoomId = 10008,
                EstimatedDate = DateTime.Now,
                Date = DateTime.Now,
                Floor = "23",
                NumericalOrder = numOrd + 1,
                OperationId = 10011,
                OperationName = "Xét nghiệm mỡ trong máu",
                PatientId = patient.Id,
                PatientName = patient.Name,
                CheckupRecordId = cr.Id,
                Status = TestRecord.TestRecordStatus.CHECKED_IN,
            };
            await _unitOfWork.TestRecordRepository.Add(tr2);
            await _unitOfWork.TestRecordRepository.Add(tr);
            await _unitOfWork.SaveChangesAsync();
            _scheduleServiceDoctor.UpdateRedis_CheckupQueue((long)cr.RoomId);
            _scheduleServiceDoctor.UpdateRedis_TestQueue(10007, false);
            _scheduleServiceDoctor.UpdateRedis_TestQueue(10008, false);
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
            _scheduleServiceDoctor.UpdateRedis_TestQueue(10008, false);
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
        public void ClearAllCache(long? RoomId)
        {
            var roomList = _unitOfWork.RoomRepository.Get().Include(x => x.RoomType)
            .Where(x => (RoomId == null || RoomId == 0) ? true : x.Id == RoomId)
            .ToList();
            foreach (var room in roomList)
            {
                if (room.RoomType.Id == IdConfig.ID_ROOMTYPE_PHONG_KHAM)
                {
                    //_scheduleServiceDoctor.UpdateRedis_FinishedCheckupQueue(room.Id);
                    _redisService.RemoveValueToKey($"finished-checkup-queue-for-room-{room.Id}");
                    //_scheduleServiceDoctor.UpdateRedis_TestingCheckupQueue(room.Id);
                    _redisService.RemoveValueToKey($"testing-checkup-queue-for-room-{room.Id}");
                    //_scheduleServiceDoctor.UpdateRedis_CheckupQueue(room.Id);
                    _redisService.RemoveValueToKey($"checkup-queue-for-room-{room.Id}");
                }
                else
                {
                    //_scheduleServiceDoctor.UpdateRedis_FinishedCheckupQueue(room.Id);
                    _redisService.RemoveValueToKey($"finished-test-queue-for-room-{room.Id}");
                    //_scheduleServiceDoctor.UpdateRedis_TestQueue(room.Id, true);
                    _redisService.RemoveValueToKey($"test-queue-for-room-{room.Id}-{true}");
                    //_scheduleServiceDoctor.UpdateRedis_TestQueue(room.Id, false);
                    _redisService.RemoveValueToKey($"test-queue-for-room-{room.Id}-{false}");
                }
            }
        }
        public async Task RemoveEverything()
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
            ClearAllCache(null);
            //var roomList = _unitOfWork.RoomRepository.Get().Include(x => x.RoomType).ToList();
            //foreach (var room in roomList)
            //{
            //    if (room.RoomType.Id == IdConfig.ID_ROOMTYPE_PHONG_KHAM)
            //    {
            //        _scheduleServiceDoctor.UpdateRedis_FinishedCheckupQueue(room.Id);
            //        _scheduleServiceDoctor.UpdateRedis_TestingCheckupQueue(room.Id);
            //        _scheduleServiceDoctor.UpdateRedis_CheckupQueue(room.Id);
            //    }
            //    else
            //    {
            //        _scheduleServiceDoctor.UpdateRedis_FinishedCheckupQueue(room.Id);
            //        _scheduleServiceDoctor.UpdateRedis_TestQueue(room.Id, true);
            //        _scheduleServiceDoctor.UpdateRedis_TestQueue(room.Id, false);
            //    }
            //}
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
                Pulse = 231,
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
                DoctorId = 10005,
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
                Status = Bill.BillStatus.DA_TT_TIEN_MAT,
                TimeCreated = DateTime.Now.AddHours(7),
                PatientName = patient.Name,
                TotalInWord = MoneyHelper.NumberToText(dakhoaOp.Price),
                CashierId = 10001,
                CashierName = "Nhân viên test",
                PatientId = patient.Id,
                QrCode = Guid.NewGuid().ToString()
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
