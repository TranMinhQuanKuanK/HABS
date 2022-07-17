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
    [Authorize(Roles = "Doctor")]
    public class IcdController : BaseDoctorController
    {

        private readonly IIcdService _icdService;

        public IcdController(IIcdService service)
        {
            _icdService = service;
        }
        [SwaggerOperation(Summary = "Lấy danh sách ICD")]
        [HttpGet]
        public IActionResult GetIcdList([FromQuery]IcdSearchModel search)
        {
            try
            {
                var data = _icdService.GetIcdList(search);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
