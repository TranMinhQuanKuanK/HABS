using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.SearchModels.User;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.User;
using BusinessLayer.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "User")]
    public class CheckupRecordsController : BaseUserController
    {

        private readonly ICheckupRecordService _checkupRecordService;

        public CheckupRecordsController(ICheckupRecordService service)
        {
            _checkupRecordService = service;
        }

        [SwaggerOperation(Summary = "Lấy BỆNH ÁN của bệnh nhân, chỉ trả metadata (chưa check được patientId có hợp lệ ko)")]
        [HttpGet]
        public IActionResult GetCheckupRecord([FromQuery] CheckupSearchModel searchModel, [FromQuery] PagingRequestModel paging)
        {
            paging = PagingUtil.checkDefaultPaging(paging);
            if (searchModel is null)
            {
                throw new ArgumentNullException(nameof(searchModel));
            }
            try
            {
                //QUAN TRỌNG:
                //Bổ sung kiểm tra patientId có thuộc accountId không? bổ sung sau khi đã thêm authentication

                var data = _checkupRecordService.GetCheckupRecordMetadata(null, searchModel.FromTime, searchModel.ToTime, searchModel.DepartmentId);

                int totalItem = data.Count;
                if (totalItem == 0)
                {
                    return NotFound();
                }

                data = data.Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Take(paging.PageSize).ToList();

                var result = new BasePagingViewModel<PatientRecordMetadataResponseModel>()
                {
                    PageIndex = paging.PageIndex,
                    PageSize = paging.PageSize,
                    TotalItem = totalItem,
                    TotalPage = (int)Math.Ceiling((decimal)totalItem / (decimal)paging.PageSize),
                    Data = data
                };
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Lấy BỆNH ÁN của bệnh nhân theo id, đầy đủ thông tin (chưa check được patientId có hợp lệ ko)")]
        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            try
            {
                //get by id
                //PatientRecordFullDataResponseModel model = new PatientRecordFullDataResponseModel()
                //{
                //    Id = 6,
                //    PatientData = new PatientResponseModel()
                //    {
                //        Id = 12,
                //        Address = "Quận 14, Tiểu vương quốc Thanh Hóa",
                //        Bhyt = "dfdf-d234-gd-fdf-df",
                //        DateOfBirth = DateTime.Now,
                //        Gender = 1,
                //        Name = "Bùi Khánh Toàn",
                //        PhoneNumber = "097861012102",
                //    },
                //    BloodPressure = 232,
                //    ClinicalSymptom = "Quá buồn bã",
                //    Diagnosis = "Tâm thần",
                //    IcdCode = "F102",
                //    DoctorAdvice = "Có bồ đi",
                //    IcdDiseaseId = 23,
                //    IcdDiseaseName = "Khùng nặng",
                //    NumericalOrder = 12,
                //    PatientName = "Bùi Khánh Toàn",
                //    Prescription = new PrescriptionResponseModel()
                //    {
                //        Id = 2,
                //        CheckupRecordId = 12,
                //        Details = new List<PrescriptionDetailResponseModel>()
                //          {
                //              new PrescriptionDetailResponseModel()
                //              {
                //                  Id = 2,
                //                  EveningDose = 1,
                //                  MorningDose = 2,
                //                  NightDose =1,
                //                  MedicineId = 23,
                //                  MedicineName = "Pararararar",
                //                  MiddayDose = 2,
                //                  PrescriptionId = 2,
                //                  Quantity = 15,
                //                  Unit = "Viên",
                //                  Usage = "Uống bằng mồm, đừng uống bằng đường nào khác",
                //              },
                //              new PrescriptionDetailResponseModel()
                //              {
                //                  Id = 3,
                //                  EveningDose = 1,
                //                  MorningDose = 2,
                //                  NightDose =1,
                //                  MedicineId = 23,
                //                  MedicineName = "C Sủi",
                //                  MiddayDose = 2,
                //                  PrescriptionId = 2,
                //                  Quantity = 15,
                //                  Unit = "Hũ",
                //                  Usage = "Uống hết một lần, đừng sợ",
                //              }
                //          }
                //    },
                //    Pulse = 23,
                //    Status = 0,
                //    Temperature = 27,
                //    TestRecords = new List<TestRecordResponseModel>()
                //    {
                //        new TestRecordResponseModel()
                //        {
                //            Id = 3,
                //            CheckupRecordId = 2,
                //            Floor = "12",
                //            NumericalOrder = 23,
                //            PatientId = 23,
                //            PatientName = "Bùi Khánh Toàn",
                //            Date = DateTime.Now,
                //            ResultFileLink = "tienganh123.com/",
                //            RoomId = 23,
                //            OperationId = 23,
                //            OperationName ="Chụp X-Quang dú",
                //            RoomNumber = "3",
                //            Status = 0
                //        },
                //          new TestRecordResponseModel()
                //        {
                //            Id = 3,
                //            CheckupRecordId = 2,
                //            Floor = "12",
                //            NumericalOrder = 23,
                //            PatientId = 23,
                //            OperationId = 23,
                //            OperationName = "Xét nghiệm HIV",
                //            PatientName = "Bùi Khánh Toàn",
                //            Date = DateTime.Now,
                //            ResultFileLink = "tienganh123.com/",
                //            RoomId = 23,
                //            RoomNumber = "3",
                //            Status = 0
                //        }
                //    },
                //    Date = DateTime.Now,
                //    DepartmentId = 5,
                //    DepartmentName = "Khoa chấn chấn sang chấn",
                //    DoctorId = 5,
                //    DoctorName = "Trần Lang Băm",
                //    EstimatedStartTime = DateTime.Now,
                //    PatientId = 4,
                //    ReExamDate = DateTime.Now
                //};
                try
                {
                    var data = _checkupRecordService.GetCheckupRecordFullData(id);
                    return Ok(data);
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}
