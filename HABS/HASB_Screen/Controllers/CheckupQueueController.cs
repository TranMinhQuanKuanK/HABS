using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HASB_Screen.Controllers
{
    [Route(ScreenRoute)]
    [ApiController]
    [Authorize(Roles = "Screen")]
    public class CheckupQueueController : BaseScreenController
    {

        private readonly IScheduleService _scheduleService;

        public CheckupQueueController( IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }
        [SwaggerOperation(Summary = "Lấy hàng chờ khám của bệnh nhân trong ngày")]
        [HttpGet]
        public IActionResult GetCheckupQueue()
        {
            int roomId = 0;
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                roomId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            try
            {
                var queue = _scheduleService.GetCheckupQueue(roomId);
                return Ok(queue);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
