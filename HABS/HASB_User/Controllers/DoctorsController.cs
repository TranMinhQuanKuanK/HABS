using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels.User;
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
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "User")]
    public class DoctorsController : BaseUserController
    {

        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService service)
        {
            _doctorService = service;
        }
        [SwaggerOperation(Summary = "Lấy danh sách bác sĩ ĐA KHOA làm việc trong ngày date")]
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
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
