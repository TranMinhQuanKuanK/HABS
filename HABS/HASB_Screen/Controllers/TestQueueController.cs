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
    public class TestQueueController : BaseScreenController
    {

        private readonly IScheduleService _scheduleService;

        public TestQueueController( IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }
        [SwaggerOperation(Summary = "Lấy hàng chờ xét nghiệm")]
        [HttpGet]
        public IActionResult GetExamQueue()
        {
            int roomId = 0;
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                roomId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            try
            {
                var queue = _scheduleService.GetTestQueue(roomId,false);
                return Ok(queue);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Lấy hàng đợi kết quả xét nghiệm")]
        [HttpGet("waiting-for-result")]
        public IActionResult GetWaitingForResultQueue()
        {
            int roomId = 0;
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                roomId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            try
            {
                var queue = _scheduleService.GetTestQueue(roomId, true);
                return Ok(queue);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
