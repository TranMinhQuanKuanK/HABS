using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.SearchModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    [Authorize(Roles = "User")]
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
        public IActionResult GetCheckupAppointment([FromQuery] CheckupAppointmentSearchModel searchModel)
        {
            if (searchModel is null)
            {
                throw new ArgumentNullException(nameof(searchModel));
            }

            try
            {
                var result = _scheduleService.GetCheckupAppointment(searchModel);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost()]
        [SwaggerOperation(Summary = "Đặt lịch khám mới")]
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
                    return Ok(await _checkupRecordService.CreatReExamAppointment(model.PatientId, (int)model.Id, model.Date, model.DoctorId,
                        model.NumericalOrder, model.ClinicalSymptom, accountId));
                }
                else
                {
                    return Ok(await _checkupRecordService.CreatNewAppointment(model.PatientId, model.Date, model.DoctorId,
                  model.NumericalOrder, model.ClinicalSymptom, accountId));
                }
              
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
