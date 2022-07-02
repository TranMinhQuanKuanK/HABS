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
using static DataAccessLayer.Models.CheckupRecord;
using BusinessLayer.Interfaces.User;
using static DataAccessLayer.Models.Bill;
using BusinessLayer.ResponseModels.ViewModels.User;
using BusinessLayer.Interfaces.Common;
using Utilities;
using BusinessLayer.Constants;

namespace BusinessLayer.Services.Doctor
{
    public class CheckupRecordService : BaseService, Interfaces.Doctor.ICheckupRecordService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IPatientService _patientService;
        private readonly Interfaces.Doctor.IScheduleService _scheduleService;

        private readonly IDepartmentService _departmentService;
        private readonly INumercialOrderService _numService;
        private readonly RedisService _redisService;
        public CheckupRecordService(IUnitOfWork unitOfWork,
            IDistributedCache distributedCache,
            IPatientService patientService,
             IDepartmentService departmentService,
             Interfaces.Doctor.IScheduleService scheduleService,
            INumercialOrderService numService) : base(unitOfWork)

        { 
            _scheduleService = scheduleService;
            _numService = numService;
            _patientService = patientService;
            _distributedCache = distributedCache;
            _departmentService = departmentService;
            _redisService = new RedisService(_distributedCache);
        }
        public List<PatientRecordMetadataViewModel> GetCheckupRecordMetadata(long? patientId, DateTime? fromTime, 
            DateTime? toTime, long? departmentId)
        {
            List<PatientRecordMetadataViewModel> data = new List<PatientRecordMetadataViewModel>();
            var dbSetData = _unitOfWork.CheckupRecordRepository.Get();
            IQueryable<CheckupRecord> queryableData = dbSetData;
            queryableData = queryableData.Where(x => x.Status == CheckupRecordStatus.KET_THUC
            || x.Status == CheckupRecordStatus.NHAP_VIEN
            );
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
               (x => new PatientRecordMetadataViewModel()
               {
                   Id = x.Id,
                   Status = (int)x.Status,
                   Date = x.Date,
                   DepartmentName = x.DepartmentName,
                   DoctorName = x.DoctorName,
                   NumericalOrder = x.NumericalOrder,
                   PatientName = x.PatientName,
                   IsReExam = (bool)x.IsReExam
               }
               ).ToList();
            return data;
        }
        public PatientRecordFullDataViewModel GetCheckupRecordFullData(long recordId)
        {
            PatientRecordFullDataViewModel data = new PatientRecordFullDataViewModel();
            data = _unitOfWork.CheckupRecordRepository.Get()
                .Include(x => x.Patient)
                .Include(x => x.Prescriptions)
                .ThenInclude(x => x.PrescriptionDetails)
                .Include(x => x.TestRecords)
                .Where(x => x.Id == recordId).AsEnumerable()
                .Select
                (x =>
                {
                    var _prescription = (x.Prescriptions.ToArray().Length>0)? x.Prescriptions.ToArray()[0]:null;
                    return new PatientRecordFullDataViewModel()
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
                        PatientData = new PatientViewModel()
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
                        Prescription =(_prescription!=null)? new PrescriptionViewModel()
                        {
                            CheckupRecordId = _prescription.CheckupRecordId,
                            Id = _prescription.Id,
                            TimeCreated = _prescription.TimeCreated,
                            Note = _prescription.Note,
                            Details = _prescription.PrescriptionDetails.Select(dt => new PrescriptionDetailViewModel()
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
                        }
                        :null,
                        Pulse = x.Pulse,
                        EstimatedDate = x.EstimatedDate,
                        Temperature = x.Temperature,
                        TestRecords = x.TestRecords.Select(tr => new TestRecordViewModel()
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
        public async Task CreatePrescription(long recordId, PrescriptionCreateModel model)
        {
            //kiểm tra CR
            var cr = _unitOfWork.CheckupRecordRepository.Get()
           .Where(x => x.Id == recordId).FirstOrDefault();
            if (cr==null)
            {
                throw new Exception("Record id invalid");
            }
            //query xem presc có chưa
            Prescription presc = null;
            presc = _unitOfWork.PrescriptionRepository
               .Get()
               .Include(x=>x.PrescriptionDetails)
               .Where(x => x.CheckupRecordId == recordId)
               .FirstOrDefault();
            //chưa có thì tạo một cái mới
            if (presc == null)
            {
                presc = new Prescription()
                {
                    Note = "",
                    CheckupRecordId = recordId,
                    TimeCreated = DateTime.Now.AddHours(7),
                };
                await _unitOfWork.PrescriptionRepository.Add(presc);
            } else
            {
                //clear hết presc detail
                foreach (var presDetail in presc.PrescriptionDetails)
                {
                    await _unitOfWork.PrescriptionDetailRepository.Delete(presDetail.Id);
                }
                await _unitOfWork.SaveChangesAsync();
                //tạo từng cái presc detail add vào presc
                foreach (var detail in model.Details)
                {
                    var med = _unitOfWork.MedicineRepository.Get()
                        .Where(x => x.Id == detail.MedicineId).FirstOrDefault();
                    if (med == null)
                    {
                        throw new Exception("Invalid medicine with id" + detail.MedicineId);
                    }
                    var presDetail = new PrescriptionDetail()
                    {
                        MedicineId = detail.MedicineId,
                        MedicineName = med.Name,
                        EveningDose = detail.EveningDose,
                        MiddayDose = detail.MiddayDose,
                        MorningDose = detail.MorningDose,
                        NightDose = detail.NightDose,
                        Quantity = detail.Quantity,
                        Unit = med.Unit,
                        Usage = detail.Usage,
                        PrescriptionId = presc.Id,
                    };
                    await _unitOfWork.PrescriptionDetailRepository.Add(presDetail);
                }
                presc.Note = model.Note;
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<List<RedirectViewModel>> RedirectPatient(RedirectCreateModel model, long recordId)
        {
            List<RedirectViewModel> result = new List<RedirectViewModel>();
            //kiểm tra patient
            var cr = _unitOfWork.CheckupRecordRepository.Get()
              .Where(x => x.Id == recordId).FirstOrDefault();
            var patient = _patientService.GetPatientById((int)cr.PatientId);
            if (patient == null)
            {
                throw new Exception("Patient invalid");
            }
            //lấy id trong model
            var depIdList = new List<long>();
            foreach (var item in model.Details)
            {
                depIdList.Add(item.DepartmentId);
            }
            //kiểm tra khoa có mở khám không 
            var validDeps = _unitOfWork.DepartmentRepository.Get()
                .Where(x => depIdList.Contains(x.Id))
                .Where(x => x.Status == Department.DepartmentStatus.CO_MO_KHAM)
                .ToList();
            if (validDeps.Count < depIdList.Count)
            {
                throw new Exception("Invalid department id list");
            }

            //lấy các operation khám tương ứng và tạo thành một bill
            var checkupOpList = _unitOfWork.OperationRepository.Get()
                .Where(x=>x.DepartmentId!=null)
                .Where(x => depIdList.Contains((long)x.DepartmentId))
                .ToList();
            Bill bill = new Bill()
            {
                Content = "Thu tiền cho các khoảng khám chuyển khoa",
                Status = BillStatus.CHUA_TT,
                TimeCreated = DateTime.Now.AddHours(7),
                PatientName = patient.Name,
                Total = 0,
                PatientId = patient.Id
            };
            await _unitOfWork.BillRepository.Add(bill);
            await _unitOfWork.SaveChangesAsync();
            //tạo các checkup record tương ứng
            foreach (var checkup in checkupOpList)
            {
                string symptom = "";
                foreach (var item in model.Details)
                {
                    if (item.DepartmentId == checkup.DepartmentId)
                    {
                        symptom = item.ClinicalSymptom;
                    }
                }
                //lấy department tương úng của checkup thay vì include
                var dep = _departmentService.GetDepartmentById((long)checkup.DepartmentId);
                //lấy lịch và phòng
                var room = _numService.GetAppropriateRoomForOperation(checkup);
                var numOrd = _numService.GetNumOrderForAutoIncreaseRoom(room,DateTime.Now.AddHours(7));
                //lấy bác sĩ làm việc tương ứng trong khung giờ + phòng

                //tạo record
                CheckupRecord _cr = new CheckupRecord()
                {
                    DepartmentId = checkup.DepartmentId,
                    Date = DateTime.Now.AddHours(7),
                    Status = CheckupRecordStatus.DA_DAT_LICH,
                    IsReExam = false,
                    Floor = room.Floor,
                    RoomNumber = room.RoomNumber,
                    RoomId = room.Id,
                    PatientId = cr.PatientId,
                    EstimatedDate = DateTime.Now.AddHours(7),
                    PatientName = patient.Name,
                    NumericalOrder = numOrd,
                    ClinicalSymptom = "Chuyển khoa từ đa khoa sang. Triệu chứng báo cáo từ bác sĩ đa khoa: \""+ symptom + "\".",
                    DepartmentName = dep.Name,
                    //NHỚ BỔ SUNG
                    //DoctorId = null,
                    //DoctorName = "Nhớ nhắc Quân bổ sung DoctorName trong trường hợp tụi mày vẫn thấy cái này",
                };
                await _unitOfWork.CheckupRecordRepository.Add(_cr);
                await _unitOfWork.SaveChangesAsync();

                //tạo bill detail tương ứng
                BillDetail detail = new BillDetail()
                {
                    BillId = bill.Id,
                    InsuranceStatus = checkup.InsuranceStatus,
                    OperationId = checkup.Id,
                    Price = checkup.Price,
                    OperationName = checkup.Name,
                    Quantity = 1,
                    SubTotal = checkup.Price,
                    CheckupRecordId = _cr.Id,
                };
                bill.Total = bill.Total + checkup.Price;
                await _unitOfWork.BillDetailRepository.Add(detail);

                //tạo response model tương ứng
                result.Add(new RedirectViewModel()
                {
                    NumericalOrder = numOrd,
                    DepartmentName = dep.Name,
                    RoomId = room.Id,
                    Floor = room.Floor,
                    RoomNumber = room.RoomNumber,
                });
            }
            cr.Status = CheckupRecordStatus.CHUYEN_KHOA;
            bill.TotalInWord = MoneyHelper.NumberToText((double)bill.Total, false);
            await _unitOfWork.SaveChangesAsync();
            return result;
        }
        public async Task RequestExamination(long recordId, TestRequestCreateModel testReqModel)
        {
            var cr = _unitOfWork.CheckupRecordRepository.Get()
               .Where(x => x.Id == recordId).FirstOrDefault();
            var opList = _unitOfWork.OperationRepository
                .Get()
                .Where(x => testReqModel.ExamOperationIds.Contains(x.Id))
                .ToList();
            var patient = _patientService.GetPatientById((int)cr.PatientId);
            //tạo bill
            Bill bill = new Bill()
            {
                Status = BillStatus.CHUA_TT,
                Content = "Hóa đơn thanh toán viện phí cho bệnh nhân " + patient.Name + " cho " + testReqModel.ExamOperationIds.Count + " mục.",
                TimeCreated = DateTime.Now.AddHours(7),
                PatientName = patient.Name,
                PatientId = patient.Id
            };
            await _unitOfWork.BillRepository.Add(bill);
            await _unitOfWork.SaveChangesAsync();
            foreach (var opId in testReqModel.ExamOperationIds)
            {
                Operation _op = null;
                foreach (var op in opList)
                {
                    if (op.Id == opId)
                    {
                        _op = op;
                    }
                }
                //Tìm phòng tương ứng cho bệnh nhân
                var room = _numService.GetAppropriateRoomForOperation(_op);
                if (room==null)
                {
                    throw new Exception("Rooms for this operation haven't been configured");
                }
                var numOrd = _numService.GetNumOrderForAutoIncreaseRoom(room, DateTime.Now.AddHours(7));
                //tạo record mới 
                TestRecord tc = new TestRecord()
                {
                    OperationId = opId,
                    OperationName = _op.Name,
                    PatientId = patient.Id,
                    PatientName = patient.Name,
                    CheckupRecordId = recordId,
                    EstimatedDate = null,
                    ResultFileLink = null,
                    Status = TestRecord.TestRecordStatus.DA_DAT_LICH,
                    Date = DateTime.Now.AddHours(7),
                    NumericalOrder = numOrd,
                    RoomId = room.Id,
                    RoomNumber = room.RoomNumber,
                    Floor = room.Floor
                };
                await _unitOfWork.TestRecordRepository.Add(tc);
                await _unitOfWork.SaveChangesAsync();
                //tạo bill detail
                BillDetail bd = new BillDetail()
                {
                    InsuranceStatus = _op.InsuranceStatus,
                    OperationId = opId,
                    OperationName = _op.Name,
                    Price = _op.Price,
                    Quantity = 1,
                    SubTotal = _op.Price,
                    TestRecordId = tc.Id,
                    BillId = bill.Id,
                };
                bill.Total = bill.Total += _op.Price;
                await _unitOfWork.BillDetailRepository.Add(bd);
                await _unitOfWork.SaveChangesAsync();
            }
            cr.Status = CheckupRecordStatus.CHO_KQXN;
            bill.TotalInWord = MoneyHelper.NumberToText((double)bill.Total, false);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task EditCheckupRecord(CheckupRecordEditModel model)
        {
            var cr = _unitOfWork.CheckupRecordRepository.Get()
                 .Where(x => x.Id == model.Id).FirstOrDefault();
            var patient = _patientService.GetPatientById(model.PatientId);
            if (cr == null
                || cr.Status == CheckupRecordStatus.CHO_TAI_KHAM
                || cr.Status == CheckupRecordStatus.DA_DAT_LICH
                || cr.Status == CheckupRecordStatus.DA_XOA
                || cr.Status == CheckupRecordStatus.CHO_TAI_KHAM
                || cr.Status == CheckupRecordStatus.KET_THUC
                || cr.Status == CheckupRecordStatus.CHO_KQXN
                || cr.Status == CheckupRecordStatus.CHUYEN_KHOA
                || cr.Status == CheckupRecordStatus.DA_HUY
                )
            {
                throw new Exception("Invalid checkup record id");
            }

            if (model.Pulse != null)
            {
                cr.Pulse = model.Pulse;
            }
            if (model.Status != null)
            {
                if (model.Status== (int)CheckupRecordStatus.KET_THUC 
                    || model.Status == (int)CheckupRecordStatus.NHAP_VIEN
                    )
                    cr.Status = (CheckupRecordStatus)model.Status;
            }
            if (model.Temperature != null)
            {
                cr.Temperature = model.Temperature;
            }
            if (model.BloodPressure != null)
            {
                cr.BloodPressure = model.BloodPressure;
            }
            if (model.DoctorAdvice != null)
            {
                cr.DoctorAdvice = model.DoctorAdvice;
            }
            if (model.Diagnosis != null)
            {
                cr.Diagnosis = model.Diagnosis;
            }
            if (model.IcdDiseaseId != null)
            {
                var icd = _unitOfWork.IcdDiseaseRepository.Get().Where(x => x.Id == model.IcdDiseaseId).FirstOrDefault();
                if (icd == null)
                {
                    throw new Exception("Invalid ICD with id" + model.IcdDiseaseId);
                }
                cr.IcdDiseaseId = model.IcdDiseaseId;
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task ConfirmCheckup(long crId, long? doctorId)
        {
            var cr = _unitOfWork.CheckupRecordRepository.Get().Where(x => x.Id == crId).FirstOrDefault();
            
            if (cr==null)
            {
                throw new Exception("Invalid checkup record id");
            }
            //lấy hàng đợi của ngày hôm đó
            var queue = _scheduleService.GetCheckupQueue((long)cr.RoomId);
            //kiểm tra xem có thật sự trong hàng đợi không
            var crInQueue = queue.SingleOrDefault(x => x.Id == cr.Id);
            if (crInQueue==null)
            {
                throw new Exception("Invalid checkup record id");
            }
            if (queue[0].Status==(int)CheckupRecordStatus.DANG_KHAM)
            {
                throw new Exception("A patient is currently in the checkup room");
            }
            crInQueue.Status = (int)CheckupRecordStatus.DANG_KHAM;
            cr.Status = CheckupRecordStatus.DANG_KHAM;
            cr.Date = DateTime.Now.AddHours(7);
            queue.Remove(crInQueue);
            queue.Insert(0, crInQueue);

            //nếu có doctor id, tức là đây là confirm của 
            if (cr.DepartmentId!=IdConstant.ID_DEPARTMENT_DA_KHOA)
            {
                var doctor = _unitOfWork.DepartmentRepository.Get().Where(x => x.Id == (long)doctorId).FirstOrDefault();
                if (doctor==null)
                {
                    throw new Exception("Invalid doctor");
                }
                cr.DoctorId = (long)doctorId;
                cr.DoctorName = doctor.Name;
            }

            await _unitOfWork.SaveChangesAsync();

            //Cập nhật lại cache hàng đợi tương ứng của phòng trong cr
        }
        public async Task CreateReExamCheckupRecord(long previousCrId, long doctorId, ReExamCreateModel model)
        {
            //không cần kiểm tra id bác sĩ
            var doc = _unitOfWork.DoctorRepository.Get().Where(x => x.Id == doctorId).FirstOrDefault();
            //kiểm tra bệnh nhân
            var patient = _unitOfWork.PatientRepository.Get().Where(x => x.Id == model.PatientId).FirstOrDefault();
            if (patient==null)
            {
                throw new Exception("Patient doesn't exist");
            }
            //kiểm tra previous CR
            var preCr = _unitOfWork.CheckupRecordRepository.Get()
                .Where(x => x.Id == previousCrId)
                .FirstOrDefault();
            if (preCr == null)
            {
                throw new Exception("Previous Checkup record doesn't exist");
            }
            //kiểm tra các operation
            var listOp = _unitOfWork.OperationRepository.Get()
                .Where(x => model.RequiredTest.ExamOperationIds.Contains(x.Id)).ToList();
            if (listOp.Count != model.RequiredTest.ExamOperationIds.Count)
            {
                throw new Exception("Invalid test request");
            }
            //kiểm tra department 
            var department = _unitOfWork.DepartmentRepository.Get().Where(x => x.Id == model.DepartmentId).FirstOrDefault();
            if (department == null)
            {
                throw new Exception("Department doesn't exist");
            }
            //Thêm note vào CR cũ
            preCr.DoctorAdvice = preCr.DoctorAdvice + $"\\n Hẹn tái khám vào ngày {model.ReExamDate.Date}, các xét nghiệm cần thực hiện trước khi tái khám";
            for (int i = 0; i < listOp.Count; i++)
            {
                preCr.DoctorAdvice = preCr.DoctorAdvice + listOp[i].Name;
                if (i== listOp.Count-1)
                {
                    preCr.DoctorAdvice = preCr.DoctorAdvice + ", ";
                } else
                {
                    preCr.DoctorAdvice = preCr.DoctorAdvice + ". ";
                }
            }
            preCr.DoctorAdvice = preCr.DoctorAdvice + "Gặp bác sĩ " + doc.Name + " , SĐT: " + doc.PhoneNo + ".";

            //tạo mới bill
            var bill = new Bill()
            {
                Content = listOp.Count == 0 ? "Thanh toán viện phí tái khám" : "Thanh toán viện phí tái khám và các xét nghiệm",
                PatientId = patient.Id,
                PhoneNo = patient.PhoneNumber,
                Status = BillStatus.CHUA_TT,
                TimeCreated = DateTime.Now.AddHours(7),
            };
            await _unitOfWork.BillRepository.Add(bill);
            //tạo mới CR
            var cr = new CheckupRecord()
            {
                DoctorId = doctorId,
                EstimatedDate = model.ReExamDate,
                DepartmentId = model.DepartmentId,
                Status = CheckupRecordStatus.CHO_TAI_KHAM,
                IsReExam = true,
                PatientId = model.PatientId,
                PatientName = patient.Name,
                TestRecords = new List<TestRecord>()
            };
            await _unitOfWork.CheckupRecordRepository.Add(cr);
            await _unitOfWork.SaveChangesAsync();
            //add bill detail and test record for TR
            foreach (var op in listOp)
            {
                var testRecor = new TestRecord()
                {
                    EstimatedDate = model.ReExamDate,
                    CheckupRecordId = cr.Id,
                    OperationId = op.Id,
                    PatientId = model.PatientId,
                    PatientName = patient.Name,
                    Status = TestRecord.TestRecordStatus.CHUA_DAT_LICH,
                };
                await _unitOfWork.TestRecordRepository.Add(testRecor);
                await _unitOfWork.SaveChangesAsync();
                var bdTR = new BillDetail()
                {
                    BillId = bill.Id,
                    InsuranceStatus = op.InsuranceStatus,
                    TestRecordId = testRecor.Id,
                    OperationId = op.Id,
                    Price = op.Price,
                    Quantity = 1,
                    SubTotal = op.Price,
                    OperationName = op.Name,
                };
                bill.Total += op.Price;
                await _unitOfWork.BillDetailRepository.Add(bdTR);
            }
            await _unitOfWork.SaveChangesAsync();
            //bill detail for CR
            var checkupOp = _unitOfWork.OperationRepository.Get()
                .Where(x => x.DepartmentId != null)
                .Where(x => x.DepartmentId == model.DepartmentId)
                .FirstOrDefault();
            if (checkupOp==null)
            {
                throw new Exception("Checkup operation corressponding with department doesn't exist");
            }
            var bdCR = new BillDetail()
            {
                BillId = bill.Id,
                InsuranceStatus = checkupOp.InsuranceStatus,
                CheckupRecordId = cr.Id,
                OperationId = checkupOp.Id,
                Price = checkupOp.Price,
                Quantity = 1,
                SubTotal = checkupOp.Price,
                OperationName = checkupOp.Name,
            };
            bill.Total += checkupOp.Price;
            await _unitOfWork.BillDetailRepository.Add(bdCR);
            bill.TotalInWord = MoneyHelper.NumberToText(bill.Total);
            await _unitOfWork.SaveChangesAsync();
        }
    }

}
