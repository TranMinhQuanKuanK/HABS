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
    //[Authorize(Roles = "Doctor")]
    public class OperationsController : BaseDoctorController
    {

        private readonly IOperationService _operationService;

        public OperationsController(IOperationService service)
        {
            _operationService = service;
        }
        [SwaggerOperation(Summary = "Lấy các xét nghiệm để yêu cầu bệnh nhân xét nghiệm, search cho operation tính sau ")]
        [HttpGet]
        public async Task<IActionResult> GetOperations()
        {
            try
            {
                var data = _operationService.GetOperations();
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
