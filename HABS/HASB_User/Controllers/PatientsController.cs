using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
                int accountId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    accountId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
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
        [SwaggerOperation(Summary = "Đăng kí một người bệnh nhân mới")]
        [HttpPost]
        public async Task<IActionResult> RegisterNewPatient([FromBody] PatientCreateEditModel model)
        {
            try
            {
                int userId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    userId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _patientService.RegisterANewPatient(userId,model);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [SwaggerOperation(Summary = "Thay đổi thông tin bệnh nhân")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditPatient(long Id,[FromBody] PatientCreateEditModel model)
        {
            try
            {
                int userId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    userId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _patientService.EditPatient(userId, Id,model);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Xóa bệnh nhân")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(long Id)
        {
            try
            {
                int userId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    userId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _patientService.DeletePatient(userId, Id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
