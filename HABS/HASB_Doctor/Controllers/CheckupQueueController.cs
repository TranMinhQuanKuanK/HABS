using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "Doctor")]
    public class CheckupQueueController : BaseDoctorController
    {

        private readonly ICheckupRecordService _checkupRecordService;

        public CheckupQueueController(ICheckupRecordService service)
        {
            _checkupRecordService = service;
        }
        [SwaggerOperation(Summary = "Lấy hàng chờ khám của bệnh nhân trong ngày (giả)")]
        [HttpGet]
        public async Task<IActionResult> GetCheckupQueue([FromQuery] long roomId)
        {
            try
            {
                //paging = PagingUtil.checkDefaultPaging(paging);
                //var products = await _checkupRecordService.GetProductList(BrandId, searchModel, paging);
                //return Ok(products);
                List<CheckupAppointmentResponseModel> data = new List<CheckupAppointmentResponseModel>
                {
                    new CheckupAppointmentResponseModel(){
                    Id = 1,
                    EstimatedStartTime = DateTime.Now,
                    NumericalOrder = 15,
                    PatientId = 15,
                    PatientName = "Bệnh nhân này là khám trước",
                    Status = 0,
                    },
                    new CheckupAppointmentResponseModel(){
                    Id = 1,
                    EstimatedStartTime = DateTime.Now,
                    NumericalOrder = 16,
                    PatientId = 11,
                    PatientName = "Bệnh nhân này khám sau",
                    Status = 0,
                    },
                };
                var pagingmodel = new BasePagingViewModel<CheckupAppointmentResponseModel>()
                {
                    Data = data,
                    PageIndex = 1,
                    PageSize = 5
                };

                return Ok(pagingmodel);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Xác nhận khám cho bệnh nhân trong hàng đợi, trả về full dữ liệu để hiển thị (giả)")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ConfirmCheckup(long id)
        {
            try
            {
                //paging = PagingUtil.checkDefaultPaging(paging);
                //var products = await _checkupRecordService.GetProductList(BrandId, searchModel, paging);
                //return Ok(products);
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
                            Date = DateTime.Now,
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
                            Date = DateTime.Now,
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
