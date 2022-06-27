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
        private readonly IScheduleService _scheduleService;


        public CheckupQueueController(ICheckupRecordService service, IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
            _checkupRecordService = service;
        }
        [SwaggerOperation(Summary = "Lấy hàng chờ khám của bệnh nhân trong ngày")]
        [HttpGet]
        public IActionResult GetCheckupQueue([FromQuery] long RoomId)
        {
            try
            {
                var queue = _scheduleService.GetCheckupQueue(RoomId);
                return Ok(queue);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Xác nhận khám cho bệnh nhân trong hàng đợi")]
        [HttpPost("confirm/{id}")]
        public async Task<IActionResult> ConfirmCheckup(long id)
        {
            try
            {
                PatientRecordFullDataViewModel model = new PatientRecordFullDataViewModel()
                {
                    Id = 6,
                    PatientData = new PatientViewModel()
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
                    Prescription = new PrescriptionViewModel()
                    {
                        Id = 2,
                        CheckupRecordId = 12,
                        Details = new List<PrescriptionDetailViewModel>()
                          {
                              new PrescriptionDetailViewModel()
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
                              new PrescriptionDetailViewModel()
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
                    TestRecords = new List<TestRecordViewModel>()
                    {
                        new TestRecordViewModel()
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
                            Status = 0,
                            DoctorId = 2,
                            DoctorName = "Nguyễn Lang Băm"
                        },
                          new TestRecordViewModel()
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
                            Status = 0,
                            DoctorId = 2,
                            DoctorName = "Nguyễn Lang Băm"
                        }
                    },
                    DoctorName = "DFDFD",
                    Date = DateTime.Now,
                    DepartmentId = 2,
                    DepartmentName = "Khoa khoa khoa chấn chấn thương",
                    DoctorId = 3,
                    EstimatedStartTime = DateTime.Now,
                    IcdCode = "A2323",
                    PatientId = 3,
                    EstimatedDate = DateTime.Now,
                };
                await _checkupRecordService.ConfirmCheckup(id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
