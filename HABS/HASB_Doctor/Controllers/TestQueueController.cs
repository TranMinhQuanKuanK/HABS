using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels.CreateModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
    //[Authorize(Roles = "Doctor")]
    public class TestQueueController : BaseDoctorController
    {

        private readonly ICheckupRecordService _checkupRecordService;
        private readonly IScheduleService _scheduleService;
        private readonly ITestRecordService _testRecordService;

        public TestQueueController(ICheckupRecordService service, IScheduleService scheduleService,
            ITestRecordService testRecordService)
        {
            _scheduleService = scheduleService;
            _checkupRecordService = service;
            _testRecordService = testRecordService;
        }
        [SwaggerOperation(Summary = "Lấy hàng chờ xét nghiệm trong ngày")]
        [HttpGet]
        public IActionResult GetTestQueue([FromQuery] long RoomId, [FromQuery]bool IsWaitingForResult)
        {
            try
            {
                var queue = _scheduleService.GetTestQueue(RoomId, IsWaitingForResult);
                return Ok(queue);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Lấy thông tin một xét nghiệm")]
        [HttpGet("{id}")]
        public IActionResult GetTestQueueItem(long Id)
        {
            try
            {
                var data = _scheduleService.GetItemInTestQueue(Id);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Xác nhận xét nghiệm cho bệnh nhân trong hàng đợi")]
        [HttpPost("confirm/{id}")]
        public async Task<IActionResult> ConfirmCheckup(long id)
        {
            try
            {
                int doctorId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    doctorId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _testRecordService.ConfirmTest(doctorId,id);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Cập nhật trạng thái và file kết quả của test record, 4 là đã xn xong, 5 là đã có kq - kết thúc, ")]
        [HttpPut()]
        public async Task<IActionResult> UpdateRecord([FromForm] TestRecordEditModel model)
        {
            try
            {
                int doctorId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    doctorId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _testRecordService.UpdateTestRecordResult(model, doctorId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
