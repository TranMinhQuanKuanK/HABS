using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using HASB_Doctor.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "Doctor")
    public class CheckupRecordsController : BaseDoctorController
    {

        private readonly ICheckupRecordService _checkupRecordService;

        public CheckupRecordsController(ICheckupRecordService service)
        {
            _checkupRecordService = service;
        }

        [SwaggerOperation(Summary = "Lấy lịch sử BỆNH ÁN của bệnh nhân, chỉ trả metadata (giả)")]
        [HttpGet]
        public async Task<IActionResult> GetCheckupRecord([FromQuery] CheckupSearchModel searchModel, [FromQuery] PagingRequestModel paging)
        {
            if (searchModel is null)
            {
                throw new ArgumentNullException(nameof(searchModel));
            }
            try
            {
                List<PatientRecordMetadataResponseModel> data = new List<PatientRecordMetadataResponseModel>
                {
                    new PatientRecordMetadataResponseModel(){
                    Id = 2,
                    Date = DateTime.Now,
                    DepartmentName = "Chấn thương chỉnh hình",
                    DoctorName = "Nguyễn Lang Băm",
                    NumericalOrder = 12,
                    PatientName = "Khánh Mai",
                    Status = 0
                    },
                   new PatientRecordMetadataResponseModel(){
                    Id = 2,
                    Date = DateTime.Now,
                    DepartmentName = "Tim mạch",
                    DoctorName = "Nguyễn Lang Thú",
                    NumericalOrder = 12,
                    PatientName = "Khánh Mai",
                    Status = 0
                    },
                };
                BasePagingViewModel<PatientRecordMetadataResponseModel> model = new BasePagingViewModel<PatientRecordMetadataResponseModel>()
                {
                    Data = data,
                    PageIndex = 2,
                    PageSize = 5,
                    TotalItem = 7,
                    TotalPage = 2
                };
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Lấy BỆNH ÁN của bệnh nhân theo id, đầy đủ thông tin (giả)")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                PatientRecordFullDataResponseModel model = new PatientRecordFullDataResponseModel()
                {
                    Id = 6,
                    PatientData = new PatientResponseModel()
                    {
                        Id = 12,
                        Address = "Quận 14, Tiểu vương quốc Thanh Hóa",
                        Bhyt = "dfdf-d234-gd-fdf-df",
                        DateOfBirth = DateTime.Now,
                        Gender = 1,
                        Name = "Bùi Khánh Toàn",
                        PhoneNumber = "097861012102",
                    },
                    BloodPressure = 232,
                    ClinicalSymptom = "Quá buồn bã",
                    Diagnosis = "Tâm thần",
                    DoctorAdvice = "Có bồ đi",
                    IcdDiseaseId = 23,
                    IcdDiseaseName = "Khùng nặng",
                    NumericalOrder = 12,
                    PatientName = "Bùi Khánh Toàn",
                    Prescription = new PrescriptionResponseModel()
                    {
                        Id = 2,
                        CheckupRecordId = 12,
                        Details = new List<PrescriptionDetailResponseModel>()
                          {
                              new PrescriptionDetailResponseModel()
                              {
                                  Id = 2,
                                  EveningDose = 1,
                                  MorningDose = 2,
                                  NightDose =1,
                                  MedicineId = 23,
                                  MedicineName = "Pararararar",
                                  MiddayDose = 2,
                                  PrescriptionId = 2,
                                  Quantity = 15,
                                  Unit = "Viên",
                                  Usage = "Uống bằng mồm, đừng uống bằng đường nào khác",
                              },
                              new PrescriptionDetailResponseModel()
                              {
                                  Id = 3,
                                  EveningDose = 1,
                                  MorningDose = 2,
                                  NightDose =1,
                                  MedicineId = 23,
                                  MedicineName = "C Sủi",
                                  MiddayDose = 2,
                                  PrescriptionId = 2,
                                  Quantity = 15,
                                  Unit = "Hũ",
                                  Usage = "Uống hết một lần, đừng sợ",
                              }
                          }
                    },
                    Pulse = 23,
                    Status = 0,
                    Temperature = 27,
                    TestRecords = new List<TestRecordResponseModel>()
                    {
                        new TestRecordResponseModel()
                        {
                            Id = 3,
                            CheckupRecordId = 2,
                            Floor = "12",
                            NumericalOrder = 23,
                            PatientId = 23,
                            PatientName = "Bùi Khánh Toàn",
                            RealDate = DateTime.Now,
                            ResultFileLink = "tienganh123.com/",
                            RoomId = 23,
                            OperationId = 23,
                            OperationName ="Chụp X-Quang dú",
                            RoomNumber = "3",
                            Status = 0
                        },
                          new TestRecordResponseModel()
                        {
                            Id = 3,
                            CheckupRecordId = 2,
                            Floor = "12",
                            NumericalOrder = 23,
                            PatientId = 23,
                            OperationId = 23,
                            OperationName = "Xét nghiệm HIV",
                            PatientName = "Bùi Khánh Toàn",
                            RealDate = DateTime.Now,
                            ResultFileLink = "tienganh123.com/",
                            RoomId = 23,
                            RoomNumber = "3",
                            Status = 0
                        }
                    },
                };
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
