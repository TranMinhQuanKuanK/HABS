using BusinessLayer.Interfaces.User;
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
    public class SlotsController : BaseUserController
    {

        private readonly ICheckupRecordService _checkupRecordService;
        private readonly IScheduleService _scheduleService;

        public SlotsController(ICheckupRecordService service, IScheduleService scheduleService)
        {
            _checkupRecordService = service;
            _scheduleService = scheduleService;

        }

        [SwaggerOperation(Summary = "Lấy danh sách lịch khám theo ngày và bác sĩ")]
        [HttpGet]
        public IActionResult GetCheckupSlot([FromQuery] SlotSearchModel model)
        {
            try
            {
               var data=  _scheduleService.GetAvailableSlots(model);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
