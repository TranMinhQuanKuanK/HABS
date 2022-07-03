using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.Services.Redis;
using DataAccessLayer.Models;
using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
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
    public class TestService :  BaseService
    {
        private readonly Interfaces.User.IScheduleService _scheduleService;
        private readonly IDepartmentService _departmentService;
        private readonly IOperationService _operationService;
        public TestService(IUnitOfWork unitOfWork, IDistributedCache distributedCache,
            Interfaces.User.IScheduleService scheduleService,
             IDepartmentService departmentService,
             IOperationService operationService
            ) : base(unitOfWork)
        {
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
        public async Task CreatNewAppointment(long patientId, DateTime date, 
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
                .Where(x => x.RoomId==10001)
                .Where(x => x.Session == reqSession)
                .Where(x => x.Weekday == date.DayOfWeek)
                .Include(x => x.Room)
                .Include(x=>x.Doctor)
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
                CashierId = 10001,
                CashierName = "Nhân viên test"
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
    }
}
