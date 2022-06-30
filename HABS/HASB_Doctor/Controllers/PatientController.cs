using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.Doctor;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using BusinessLayer.Utilities;
using HASB_Doctor.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "Doctor")
    public class PatientController : BaseDoctorController
    {

        private readonly ICheckupRecordService _checkupRecordService;

        public PatientController(ICheckupRecordService service)
        {
            _checkupRecordService = service;
        }
        [SwaggerOperation(Summary = "Đặt thêm lịch tái khám cho bệnh nhân, id url là id bệnh nhân (giả)")]
        [HttpPost("{id}/reexam")]
        public async Task<IActionResult> ArrangeTest(long Id, [FromBody] ReExamCreateModel model)
        {
            try
            {
                int doctorId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    doctorId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _checkupRecordService.CreateReExamCheckupRecord(Id, doctorId, model);
                return Ok($"Bạn đã gửi bệnh nhân {Id} với lời nhắn {model.Note}, ngày tái khám: {model.ReExamDate}");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
       
    }
}
