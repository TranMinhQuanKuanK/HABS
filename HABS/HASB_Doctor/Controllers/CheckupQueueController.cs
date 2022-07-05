using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Xác nhận khám cho bệnh nhân trong hàng đợi")]
        [HttpPost("confirm/{id}")]
        public async Task<IActionResult> ConfirmCheckup(long id)
        {
            try
            {
                int doctorId = 0;
                ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    doctorId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _checkupRecordService.ConfirmCheckup(id, doctorId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
