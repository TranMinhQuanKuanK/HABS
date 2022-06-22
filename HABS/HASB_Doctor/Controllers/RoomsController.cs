using BusinessLayer.Interfaces.Common;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
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
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
