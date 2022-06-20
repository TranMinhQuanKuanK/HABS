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

namespace BusinessLayer.Services.Doctor
{
    public class CheckupRecordService : BaseService, Interfaces.Doctor.ICheckupRecordService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IPatientService _patientService;
        private readonly INumercialOrderService _numService;
        private readonly RedisService _redisService;
        public CheckupRecordService(IUnitOfWork unitOfWork, 
            IDistributedCache distributedCache, 
            IPatientService patientService,
            INumercialOrderService numService) : base(unitOfWork)
        {
            _numService = numService;
            _patientService = patientService;
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public List<PatientRecordMetadataViewModel> GetCheckupRecordMetadata(long? patientId, DateTime? fromTime, DateTime? toTime, long? departmentId)
        {
            List<PatientRecordMetadataViewModel> data = new List<PatientRecordMetadataViewModel>();
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
               (x => new PatientRecordMetadataViewModel()
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
        public PatientRecordFullDataViewModel GetCheckupRecordFullData(long patientId)
        {
            PatientRecordFullDataViewModel data = new PatientRecordFullDataViewModel();
            data = _unitOfWork.CheckupRecordRepository.Get()
                .Include(x => x.Patient)
                .Include(x => x.Prescriptions)
                    .ThenInclude(x => x.PrescriptionDetails)
                .Include(x => x.TestRecords)
                .Where(x => x.PatientId == patientId).AsEnumerable().Select
                (x =>
                {
                    var _prescription = x.Prescriptions.ToArray()[0];
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
                        Prescription = new PrescriptionViewModel()
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
                        },
                        Pulse = x.Pulse,
                        ReExamDate = x.ReExamDate,
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
                        }).ToList(),
                    };
                }).FirstOrDefault();
            return data;
        }
        public async Task EditCheckupRecord(CheckupRecordEditModel model)
        {
            var cr = _unitOfWork.CheckupRecordRepository.Get()
                 .Where(x => x.Id == model.Id).FirstOrDefault();
            var opList = _unitOfWork.OperationRepository
                .Get()
                .Where(x => model.OperationIds.Contains(x.Id))
                .ToList();
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
            if (opList.Count != model.OperationIds.Count)
            {
                throw new Exception("Invalid operation ids");
            }

            if (model.Pulse != null)
            {
                cr.Pulse = model.Pulse;
            }
            if (model.Status != null)
            {
                cr.Status = (CheckupRecordStatus)model.Status;
            }
            if (model.Temperature != null)
            {
                cr.Temperature = model.Temperature;
            }
            if (model.ReExamDate != null)
            {
                cr.ReExamDate = model.ReExamDate;
            }
            if (model.BloodPressure != null)
            {
                cr.BloodPressure = model.BloodPressure;
            }
            if (model.DoctorAdvice != null)
            {
                cr.DoctorAdvice = model.DoctorAdvice;
            }
            if (model.IcdDiseaseId != null)
            {
                //Kiểm tra Id chính xác không
                var icd = _unitOfWork.IcdDiseaseRepository.Get().Where(x => x.Id == model.IcdDiseaseId).FirstOrDefault();
                if (icd==null)
                {
                    throw new Exception("Invalid ICD with id" + model.IcdDiseaseId);
                }
                cr.IcdDiseaseId = model.IcdDiseaseId;
            }
            if (model.OperationIds.Count > 0)
            {
                await addTestRecords(model, patient, opList);
            }
            if (model.Prescription != null)
            {
                await addPrescription(model);
            }
        }
        private async Task addTestRecords(
            CheckupRecordEditModel model, 
            PatientResponseModel patient,
            List<Operation> opList
            )
        {
            //tạo bill
            Bill bill = new Bill()
            {
                Status = BillStatus.CHUA_TT,
                Content = "Hóa đơn thanh toán viện phí cho bệnh nhân " + patient.Name + "cho " + model.OperationIds.Count + "mục.",
                TimeCreated = DateTime.Now.AddHours(7),
                PatientName = patient.Name,
            };
            foreach (var opId in model.OperationIds)
            {
                Operation _op = null;
                foreach (var op in opList)
                {
                    if (op.Id == opId)
                    {
                        _op = op;
                    }
                }
                //tạo record
                TestRecord tc = new TestRecord()
                {
                    OperationId = opId,

                    OperationName = _op.Name,
                    PatientId = model.PatientId,
                    PatientName = patient.Name,
                    CheckupRecordId = model.Id,
                    EstimatedDate = null,
                    ResultFileLink = null,
                    //tự gán tùy vào phòng operation available tùy theo, lưu ROOM_AVAILABLE_xxx lên cache
                    Status = TestRecord.TestRecordStatus.DA_DAT_LICH,
                    //Gọi service lấy số, nhớ lock lại, để singleton
                    Date = DateTime.Now.AddHours(7),
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
                    CheckupRecordId = model.Id,
                    BillId = bill.Id,
                };
                bill.Total = bill.Total += _op.Price;
                await _unitOfWork.BillDetailRepository.Add(bd);
                await _unitOfWork.SaveChangesAsync();
            }
            //Tạo thêm test record cho bệnh nhân
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task addPrescription(CheckupRecordEditModel model)
        {
            //query xem presc có chưa
            Prescription presc = null;
            presc = _unitOfWork.PrescriptionRepository
               .Get()
               .Where(x => x.CheckupRecordId == model.Id)
               .FirstOrDefault();
            //chưa có thì tạo một cái mới
            if (presc == null)
            {
                presc = new Prescription()
                {
                    Note = "Đơn thuốc cho bệnh nhân ",
                    CheckupRecordId = model.Id,
                    TimeCreated = DateTime.Now.AddHours(7),
                };
                await _unitOfWork.PrescriptionRepository.Add(presc);
            }
            //clear hết presc detail
            presc.PrescriptionDetails = new List<PrescriptionDetail>();
            await _unitOfWork.SaveChangesAsync();
            //tạo từng cái presc detail add vào presc
            foreach (var detail in model.Prescription.Details)
            {
                var med = _unitOfWork.MedicineRepository.Get()
                    .Where(x => x.Id == detail.MedicineId).FirstOrDefault();
                if (med == null)
                {
                    throw new Exception("Invalid medicine with id" + detail.MedicineId);
                }
                presc.PrescriptionDetails.Add(new PrescriptionDetail()
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
                });
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }

}
