using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.ResponseModels.SearchModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "Doctor")]
    public class CheckupQueueController : BaseDoctorController
    {

        private readonly ICheckupRecordService _checkupRecordService;

        public CheckupQueueController(ICheckupRecordService service)
        {
            _checkupRecordService = service;
        }
        [SwaggerOperation(Summary = "Lấy hàng chờ khám của bệnh nhân trong ngày (giả)")]
        [HttpGet]
        public async Task<IActionResult> GetCheckupQueue([FromQuery] long roomId)
        {
            try
            {
                //paging = PagingUtil.checkDefaultPaging(paging);
                //var products = await _checkupRecordService.GetProductList(BrandId, searchModel, paging);
                //return Ok(products);
                List<CheckupAppointmentResponseModel> data = new List<CheckupAppointmentResponseModel>
                {
                    new CheckupAppointmentResponseModel(){
                    Id = 1,
                    EstimatedStartTime = DateTime.Now,
                    NumericalOrder = 15,
                    PatientId = 15,
                    PatientName = "Bệnh nhân này là khám trước",
                    Status = 0,
                    },
                    new CheckupAppointmentResponseModel(){
                    Id = 1,
                    EstimatedStartTime = DateTime.Now,
                    NumericalOrder = 16,
                    PatientId = 11,
                    PatientName = "Bệnh nhân này khám sau",
                    Status = 0,
                    },
                };
                var pagingmodel = new BasePagingViewModel<CheckupAppointmentResponseModel>()
                {
                    Data = data,
                    PageIndex = 1,
                    PageSize = 5
                };

                return Ok(pagingmodel);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Xác nhận khám cho bệnh nhân trong hàng đợi, trả về full dữ liệu để hiển thị (giả)")]
        [HttpPut]
        public async Task<IActionResult> ConfirmCheckup([FromQuery] long checkupRecordId)
        {
            try
            {
                //paging = PagingUtil.checkDefaultPaging(paging);
                //var products = await _checkupRecordService.GetProductList(BrandId, searchModel, paging);
                //return Ok(products);

                return Ok(pagingmodel);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}
