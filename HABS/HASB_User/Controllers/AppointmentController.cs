using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.SearchModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
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
    public class AppointmentController : BaseUserController
    {

        private readonly ICheckupRecordService _checkupRecordService;

        public AppointmentController(ICheckupRecordService service)
        {
            _checkupRecordService = service;
        }

        [SwaggerOperation(Summary = "Lấy lịch khám của bệnh nhân từ ngày From (giả)")]
        [HttpGet]
        //lấy lịch khám của bệnh nhân
        public async Task<IActionResult> GetCheckupAppointment([FromQuery] CheckupSearchModel searchModel, [FromQuery] PagingRequestModel paging)
        {
            if (searchModel is null)
            {
                throw new ArgumentNullException(nameof(searchModel));
            }

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
                    PatientName = "Trần Thị Mock data",
                    Status = 0,
                    },
                    new CheckupAppointmentResponseModel(){
                    Id = 1,
                    EstimatedStartTime = DateTime.Now,
                    NumericalOrder = 16,
                    PatientId = 11,
                    PatientName = "Trần Thị Mock data 2",
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
        [SwaggerOperation(Summary = "Lấy lịch khám của bệnh nhân theo id (giả)")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                //get by id
                var model = new CheckupAppointmentResponseModel()
                {
                    Id = 1,
                    EstimatedStartTime = DateTime.Now,
                    NumericalOrder = 15,
                    PatientId = 15,
                    PatientName = "Trần Thị Mock data",
                    Status = 0,
                };
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("{id}")]
        [SwaggerOperation(Summary = "Book lịch khám mới (giả)")]
        public async Task<IActionResult> BookAppointment([FromBody] AppointmentCreateModel model)
        {
            try
            {
                //create new checkup record or return error
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
