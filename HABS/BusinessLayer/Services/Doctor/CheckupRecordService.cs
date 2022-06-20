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

namespace BusinessLayer.Services.Doctor
{
    public class CheckupRecordService : BaseService, Interfaces.Doctor.ICheckupRecordService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IPatientService _patientService;
        private readonly RedisService _redisService;
        public CheckupRecordService(IUnitOfWork unitOfWork,IDistributedCache distributedCache, IPatientService patientService) : base(unitOfWork)
        {
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
            CheckupRecord data = _unitOfWork.CheckupRecordRepository.Get()
                 .Where(x => x.Id == model.Id).FirstOrDefault();
            var opList = _unitOfWork.OperationRepository
                .Get()
                .Where(x=> model.OperationIds.Contains(x.Id))
                .ToList();
            var patient = _patientService.GetPatientById(model.PatientId);
            if (data==null)
            {
                throw new Exception("Invalid checkup record id");
            }
            if (opList.Count != model.OperationIds.Count)
            {
                throw new Exception("Invalid operation ids");
            }

            if (model.Pulse != null)
            {
                data.Pulse = model.Pulse;
            }
            if (model.Status != null)
            {
                data.Status = (CheckupRecordStatus)model.Status;
            }
            if (model.Temperature != null)
            {
                data.Temperature = model.Temperature;
            }
            if (model.ReExamDate != null)
            {
                data.ReExamDate = model.ReExamDate;
            }
            if (model.BloodPressure != null)
            {
                data.BloodPressure = model.BloodPressure;
            }
            if (model.DoctorAdvice != null)
            {
                data.DoctorAdvice = model.DoctorAdvice;
            }
            if (model.IcdDiseaseId != null)
            {
                //Kiểm tra Id chính xác không
                data.IcdDiseaseId = model.IcdDiseaseId;
            }
            if (model.OperationIds.Count > 0)
            {
                //tạo bill
                Bill bill = new Bill()
                {
                    Status = BillStatus.CHUA_TT,
                    Content = "Hóa đơn thanh toán viện phí cho bệnh nhân " + patient.Name,
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
                        RoomId = 0,
                        Floor = "Later",
                        RoomNumber = "A???",

                        Status = TestRecord.TestRecordStatus.DA_DAT_LICH,
                        //Gọi service lấy số, nhớ lock lại, để singleton
                        NumericalOrder = 0,
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
                        
                        
                    }
                }
                //Tạo thêm test record cho bệnh nhân

            }

            return data;
        }
    }

}
