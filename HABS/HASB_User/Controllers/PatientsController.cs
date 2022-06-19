using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.User;
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
    public class PatientsController : BaseUserController
    {

        private readonly IPatientService _patientService;

        public PatientsController(IPatientService service)
        {
            _patientService = service;
        }

        [SwaggerOperation(Summary = "Lấy danh sách bệnh nhân của tài khoản")]
        [HttpGet]
        public IActionResult GetPatients()
        {
            try
            {
                //Lấy accountId
                long accountId = 10001;
                var result = _patientService.GetPatients(accountId);
                if (result.Count==0)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Lấy bệnh nhân theo id")]
        [HttpGet("{id}")]
        public IActionResult GetPatientById(long id)
        {
            try
            {
                //Kiểm tra patient có thuộc user hay không?
                var result = _patientService.GetPatientById(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}
