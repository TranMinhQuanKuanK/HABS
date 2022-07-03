using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.SearchModels.User;
using BusinessLayer.ResponseModels.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "User")]
    public class AppointmentController : BaseUserController
    {

        private readonly ICheckupRecordService _checkupRecordService;
        private readonly IScheduleService _scheduleService;


        public AppointmentController(ICheckupRecordService service, IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
            _checkupRecordService = service;
        }

        [SwaggerOperation(Summary = "Lấy lịch khám của bệnh nhân trong khoảng thời gian, theo khoa và bệnh nhân")]
        [HttpGet]
        //lấy lịch khám của bệnh nhân
        public IActionResult GetCheckupAppointment([FromQuery] CheckupAppointmentSearchModel searchModel)
        {
            if (searchModel is null)
            {
                throw new ArgumentNullException(nameof(searchModel));
            }

            try
            {
                var result = _scheduleService.GetCheckupAppointment(searchModel);
                //paging = PagingUtil.checkDefaultPaging(paging);
                //var products = await _checkupRecordService.GetProductList(BrandId, searchModel, paging);
                //return Ok(products);
                List < CheckupAppointmentResponseModel > data = new List<CheckupAppointmentResponseModel>
                {
                    new CheckupAppointmentResponseModel(){
                    Id = 1,
                    EstimatedStartTime = DateTime.Now,
                    NumericalOrder = 15,
                    PatientId = 15,
                    PatientName = "Trần Thị Mock data",
                    Status = 0,
                    DepartmentId = 3,
                    DoctorId = 3,
                    DoctorName = "Ai dị",
                    EstimatedDate = DateTime.Now,
                    DepartmentName = "Suyễn",
                    IsReExam = true,
                    },
                    new CheckupAppointmentResponseModel(){
                    Id = 1,
                    EstimatedStartTime = DateTime.Now,
                    NumericalOrder = 16,
                    PatientId = 11,
                    PatientName = "Trần Thị Mock data 2",
                    Status = 0,
                    DepartmentId = 3,
                    DoctorId = 3,
                    DoctorName = "Ai dị",
                    EstimatedDate = DateTime.Now,
                     DepartmentName = "Chấn thương chỉnh hình",
                    IsReExam = false,
                    },
                };
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Lấy lịch khám của bệnh nhân theo id (giả)")]
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute]long id)
        {
            try
            {
                //get by id
                var model = new CheckupAppointmentResponseModel()
                {
                    Id = 1,
                    EstimatedStartTime = DateTime.Now,
                    NumericalOrder = 15,
                    PatientId = 15,
                    PatientName = "Trần Thị Mock data",
                    Status = 0,
                };
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost()]
        [SwaggerOperation(Summary = "Book lịch khám mới (hình như real rồi, gọi thử đi)")]
        public async Task<IActionResult> BookAppointment([FromBody] AppointmentCreateModel model)
        {
            try
            {
                //kiểm tra patientId có thuộc account không
                int accountId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    accountId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                //create new checkup record or return error
                if (model.IsReExam && model.Id!=null)
                {
                    await _checkupRecordService.CreatReExamAppointment(model.PatientId,(int)model.Id,model.Date,model.DoctorId,
                        model.NumericalOrder,model.ClinicalSymptom,accountId);
                }
                else
                {
                    await _checkupRecordService.CreatNewAppointment(model.PatientId, model.Date, model.DoctorId,
                  model.NumericalOrder, model.ClinicalSymptom, accountId);
                }
              
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
