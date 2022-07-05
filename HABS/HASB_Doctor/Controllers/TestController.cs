using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using BusinessLayer.Services.Doctor;
using BusinessLayer.Services.Test;
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
    public class TestController : BaseDoctorController
    {
        private readonly TestService ts;
        private readonly IScheduleService _scheduleServce;

        public TestController(TestService service, IScheduleService scheduleServce)
        {
            _scheduleServce = scheduleServce;
            ts = service;
        }
        [SwaggerOperation(Summary = "Tạo 3 bệnh nhân đặt khám trong ngày hôm đấy, đã thanh toán")]
        [HttpGet("checkup-book-3")]
        public async Task<IActionResult> CreateThreePatientsAppointment()
        {
            try
            {
                await ts.CreatNewAppointment(10000,
                    DateTime.Now.AddHours(7), 10007, null, "Ói mửa lung tung nên không muốn code hàm test kĩ");
                await ts.CreatNewAppointment(10003,
                    DateTime.Now.AddHours(7), 10007, null, "Tự nhiên chán không buồn nói luôn");
                await ts.CreatNewAppointment(10006,
                    DateTime.Now.AddHours(7), 10007, null, "Khóc la om sòm vì mãi không chạy được");
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Tạo 2 bệnh nhân đã được đưa vào phòng chờ xét nghiệm")]
        [HttpGet("test-book-2")]
        public async Task<IActionResult> CreateThreePatidentsAppointment()
        {
            try
            {
                await ts.CreatNewAppointmentWithPreviousTest(10006,
                   DateTime.Now.AddHours(7), 10007, null, "Khóc la om sòm vì mãi không chạy được");
                await ts.CreatNewAppointmentWithPreviousTest(10006,
                 DateTime.Now.AddHours(7), 10007, null, "Khóc la om sòm vì mãi không chạy được");
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Hủy hết bill :D ")]
        [HttpGet("clear-bill")]
        public async Task<IActionResult> df()
        {
            try
            {
                await ts.RemoveAllBill();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Xóa hết hàng đợi checkup phòng 10001")]
        [HttpGet("remove-all-10001")]
        public async Task<IActionResult> dfdf()
        {
            try
            {
                await ts.RemoveAllPatientThatDay(10001);
                _scheduleServce.UpdateRedis_CheckupQueue(10001);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
       
        [SwaggerOperation(Summary = "Refresh queue của phòng 10001 nếu có gì sai sót")]
        [HttpGet("refresh-queue")]
        public async Task<IActionResult> reset()
        {
            try
            {
                _scheduleServce.UpdateRedis_CheckupQueue(10001);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
