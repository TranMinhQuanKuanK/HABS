using BusinessLayer.Interfaces.User;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    //[Authorize(Roles = "User")]
    public class DoctorsController : BaseUserController
    {

        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService service)
        {
            _doctorService = service;
        }
        [SwaggerOperation(Summary = "Lấy danh sách bác sĩ ĐA KHOA làm việc theo ngày")]
        [HttpGet]
        public IActionResult GetDoctors([FromQuery] DateTime? Date)
        {
            try
            {
                if (Date == null)
                {
                    return BadRequest();
                }
                var doctors = _doctorService.GetDoctors(Date, 10000);
                return Ok(doctors);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
