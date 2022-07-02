using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.SearchModels.User;
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
    //[Authorize(Roles = "User")]
    public class SlotsController : BaseUserController
    {

        private readonly ICheckupRecordService _checkupRecordService;
        private readonly IScheduleService _scheduleService;

        public SlotsController(ICheckupRecordService service, IScheduleService scheduleService)
        {
            _checkupRecordService = service;
            _scheduleService = scheduleService;

        }

        [SwaggerOperation(Summary = "Lấy lịch khám theo ngày, bác sĩ")]
        [HttpGet]
        //Lấy lịch khám theo filter
        public IActionResult GetCheckupSlot([FromQuery] SlotSearchModel model)
        {
            try
            {
               var data=  _scheduleService.GetAvailableSlots(model);
                List<CheckupSlotResponseModel> result = new List<CheckupSlotResponseModel>
                {
                    new CheckupSlotResponseModel()
                    {
                        EstimatedStartTime = DateTime.Now,
                        IsAvailable = true,
                        NumericalOrder = 1,
                        Floor = "sdsd",
                        RoomNumber = "SD33",
                        RoomId = 2,
                    },
                    new CheckupSlotResponseModel()
                    {
                        EstimatedStartTime = DateTime.Now,
                        IsAvailable = false,
                        NumericalOrder = 2,
                            Floor = "sdsd",
                        RoomNumber = "SD33",
                        RoomId = 2,
                    },
                    new CheckupSlotResponseModel()
                    {
                        EstimatedStartTime = DateTime.Now,
                        IsAvailable = false,
                        NumericalOrder = 3,
                        Floor = "sdsd",
                        RoomNumber = "SD33",
                        RoomId = 2,
                    },
                    new CheckupSlotResponseModel()
                    {
                        EstimatedStartTime = DateTime.Now,
                        IsAvailable = true,
                        NumericalOrder = 4,
                            Floor = "sdsd",
                        RoomNumber = "SD33",
                        RoomId = 2,
                    }
                };
                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest();
            }
            //mock data
        }

    }
}
