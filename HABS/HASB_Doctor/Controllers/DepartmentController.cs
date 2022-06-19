using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels;
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
    //[Authorize(Roles = "Doctor")]
    public class DepartmentController : BaseDoctorController
    {

        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService service)
        {
            _departmentService = service;
        }
        [SwaggerOperation(Summary = "Lấy khoa (để chuyển viện)")]
        [HttpGet]
        public IActionResult GetDepartments()
        {
            try
            {
                
                //List<DepartmentResponseModel> data = new List<DepartmentResponseModel>()
                //{
                //    new DepartmentResponseModel()
                //    {
                //          Id = 2,
                //    Name = "Khoa tim mạch"
                //    },
                //   new DepartmentResponseModel()
                //    {
                //          Id = 3,
                //    Name = "Khoa chấn thương chỉnh hình",
                //    },
                //   new DepartmentResponseModel()
                //    {
                //          Id = 4,
                //    Name = "Khoa phổi",
                //    },
                //};

                return Ok(_departmentService.GetDepartments());
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
