using BusinessLayer.Interfaces.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
    //[Authorize(Roles = "Doctor,User")]
    public class RoomsController : BaseDoctorController
    {

        private readonly IRoomService _roomService;

        public RoomsController(IRoomService service)
        {
            _roomService = service;
        }
        [SwaggerOperation(Summary = "Lấy danh sách phòng khám")]
        [HttpGet]
        public IActionResult GetRoomList()
        {
            try
            {
                var data = _roomService.GetRooms(false);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
