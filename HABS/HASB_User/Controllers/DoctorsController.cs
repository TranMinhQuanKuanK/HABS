using BusinessLayer.Constants;
using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels.SearchModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    [Authorize(Roles = "User")]
    public class DoctorsController : BaseUserController
    {

        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService service)
        {
            _doctorService = service;
        }
        [SwaggerOperation(Summary = "Lấy danh sách bác sĩ ĐA KHOA làm việc theo ngày hoặc theo tên")]
        [HttpGet]
        public IActionResult GetDoctors([FromQuery] DoctorSearchModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest();
                }
                if (!string.IsNullOrEmpty(model.SearchTerm))
                {
                    return Ok(_doctorService.GetDoctorsLBySearchTerm(model.SearchTerm));
                }
                return Ok(_doctorService.GetDoctors(model.Date, IdConfig.ID_DEPARTMENT_DA_KHOA));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Lấy danh sách ngày làm việc có thể đặt của bác sĩ ĐA KHOA")]
        [HttpGet("{id}/working-date")]
        public IActionResult GetDoctorWorkingDays(long Id)
        {
            try
            {
                if (Id <= 0)
                {
                    return BadRequest();
                }
                var doctors = _doctorService.GetDoctorWorkingDay(Id, 3);
                return Ok(doctors);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
