using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.SearchModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    [Authorize(Roles = "User")]
    public class RoomController : BaseUserController
    {

        private readonly ICheckupRecordService _checkupRecordService;
        private readonly IScheduleService _scheduleService;

        public RoomController(ICheckupRecordService service, IScheduleService scheduleService)
        {
            _checkupRecordService = service;
            _scheduleService = scheduleService;

        }

        [SwaggerOperation(Summary = "Lấy số thứ tự hiện tại của phòng khám")]
        [HttpGet("checkup/current")]
        public IActionResult GetCurrentNumberCheckupRoom([FromQuery] long RoomId)
        {
            try
            {
                var data = _scheduleService.GetCurrentNumberCheckupRoom(RoomId);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Lấy số thứ tự hiện tại của phòng xét nghiệm")]
        [HttpGet("test/current")]
        public IActionResult GetCurrentNumberTestRoom([FromQuery] long RoomId)
        {
            try
            {
                var data = _scheduleService.GetCurrentNumberTestRoom(RoomId);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
