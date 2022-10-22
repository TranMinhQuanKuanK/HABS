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
    [Authorize(Roles = "Doctor")]
    public class ReExamTreeController : BaseDoctorController
    {

        private readonly IReExamTreeService _reExamTreeServiceService;

        public ReExamTreeController(IReExamTreeService service)
        {
            _reExamTreeServiceService = service;
        }
        [SwaggerOperation(Summary = "Lấy lịch sử bệnh án liên quan")]
        [HttpGet("{id}")]
        public IActionResult GetReExamTree(string Id)
        {
            try
            {
                var data = _reExamTreeServiceService.GetReExamTree(Id);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
