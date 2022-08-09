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
        [SwaggerOperation(Summary = "Tạo 3 bệnh nhân đặt khám trong ngày hôm đấy, đã thanh toán, room 10001")]
        [HttpGet("checkup-book-3")]
        public async Task<IActionResult> CreateThreePatientsAppointment()
        {
            try
            {
                await ts.CreatNewAppointment(10000,
                    DateTime.Now.AddHours(7), 10007, null, "Ói mửa lung tung, khóc la om xòm, con tôi quấy khóc quá");
                await ts.CreatNewAppointment(10003,
                    DateTime.Now.AddHours(7), 10007, null, "Sốt cao 39 độ, quá bất ổn luôn");
                await ts.CreatNewAppointment(10006,
                    DateTime.Now.AddHours(7), 10007, null, "Tự nhiên ra máu hơi nhiều ở vùng lưng");
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Tạo 3 bệnh nhân đặt khám nhưng chưa checkin, room 10001")]
        [HttpGet("checkup-book-3-without-checkin")]
        public async Task<IActionResult> CreateThreePatientsAppointmentWithoutCheckin()
        {
            try
            {
                List<string> QrList = new List<string>();
                var qr1 = await ts.CreatNewAppointmentWithoutCheckin(10000,
                    DateTime.Now.AddHours(7), 10007, null, "Ói mửa lung tung, khóc la om xòm, con tôi quấy khóc quá");
                var qr2 = await ts.CreatNewAppointmentWithoutCheckin(10003,
                    DateTime.Now.AddHours(7), 10007, null, "Sốt cao 39 độ, quá bất ổn luôn");
                var qr3 = await ts.CreatNewAppointmentWithoutCheckin(10006,
                    DateTime.Now.AddHours(7), 10007, null, "Tự nhiên ra máu hơi nhiều ở vùng lưng");
                QrList.Add(qr1);
                QrList.Add(qr2);
                QrList.Add(qr3);
                return Ok(QrList);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Tạo 1 bệnh nhân đã xong hết KQXN nhưng chưa checkin quay về, room 10001")]
        [HttpGet("book-comeback")]
        public async Task<IActionResult> CreatePatientComeback()
        {
            try
            {
                var qr1 = await ts.CreatNewAppointmentWithPreviousTestFinished(10000,
                    DateTime.Now.AddHours(7), 10007, null, "Ói mửa lung tung, khóc la om xòm, con tôi quấy khóc quá");
                return Ok(qr1);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Tạo 2 bệnh nhân đã được đưa vào phòng chờ xét nghiệm, room 10001")]
        [HttpGet("test-book-2")]
        public async Task<IActionResult> CreateTwoPatientsIntoExamRoom()
        {
            try
            {
                await ts.CreatNewAppointmentWithPreviousTest(10006,
                   DateTime.Now.AddHours(7), 10007, null, "Khóc la om sòm vì mãi không chạy được");
                await ts.CreatNewAppointmentWithPreviousTest(10006,
                 DateTime.Now.AddHours(7), 10007, null, "Bực quá vậy ta?");
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Xóa everything !!! Dangerous!!!")]
        [HttpGet("clear-everything")]
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

        [SwaggerOperation(Summary = "Tạo 5 bệnh án cho Trần Minh Quân")]
        [HttpGet("make-quan-sick")]
        public async Task<IActionResult> dfdfdsdsfdf()
        {
            try
            {
                await ts.CreateAHistory(10000, new DateTime(2022, 5, 9, 9, 15, 0), 10005, 15, "Tự nhiên tôi quá buồn nên tôi đi khám. Thế thôi", 10000);
                await ts.CreateAHistory(10000, new DateTime(2022, 5, 10, 3, 15, 0), 10005, 16, "Tự nhiên tôi quá buồn nên tôi đi khám. Thế thôi", 10001);
                await ts.CreateAHistory(10000, new DateTime(2022, 5, 11, 4, 15, 0), 10005, 17, "Tự nhiên tôi quá buồn nên tôi đi khám. Thế thôi", 10002);
                await ts.CreateAHistory(10000, new DateTime(2022, 5, 8, 5, 15, 0), 10005, 18, "Tự nhiên tôi quá buồn nên tôi đi khám. Thế thôi", 10003);
                await ts.CreateAHistory(10000, new DateTime(2021, 5, 7, 6, 15, 0), 10005, 19, "Tự nhiên tôi quá buồn nên tôi đi khám. Thế thôi", 10004);
                await ts.CreateAHistory(10000, new DateTime(2021, 5, 6, 7, 15, 0), 10005, 20, "Tự nhiên tôi quá buồn nên tôi đi khám. Thế thôi", 10004);
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
