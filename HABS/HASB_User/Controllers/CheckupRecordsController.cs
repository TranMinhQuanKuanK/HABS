﻿using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.SearchModels.User;
using BusinessLayer.ResponseModels.ViewModels;
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
    public class CheckupRecordsController : BaseUserController
    {

        private readonly ICheckupRecordService _checkupRecordService;

        public CheckupRecordsController(ICheckupRecordService service)
        {
            _checkupRecordService = service;
        }

        [SwaggerOperation(Summary = "Lấy BỆNH ÁN của bệnh nhân từ thời điểm From trở đi (giả)")]
        [HttpGet]
        public async Task<IActionResult> GetCheckupRecord([FromQuery] CheckupSearchModel searchModel, [FromQuery] PagingRequestModel paging)
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
                List<CheckupDataResponseModel> data = new List<CheckupDataResponseModel>
                {
                    new CheckupDataResponseModel(){
                    Id = 1,
                    EstimatedStartTime = DateTime.Now,
                    NumericalOrder = 15,
                    PatientId = 15,
                    PatientName = "Trần Thị Mock data",
                    Status = 0,
                    BloodPressure = 232,
                    ClinicalSymptom = "Đau đầu tột cùng",
                    DepartmentId = 2,
                    Diagnosis = "Ung thư vú",
                    DoctorAdvice = "Đi ngủ sớm",
                    DoctorName = "Nguyễn Trần Lang Băm",
                    EstimatedDate = DateTime.Now,
                    IcdDiseaseId = 34,
                    IcdDiseaseName = "Ung thư cổ tử cung",
                    DoctorId = 14,
                    Pulse = 12323,
                    Temperature = 23,
                    BillId = 2
                    },
                   new CheckupDataResponseModel(){
                    Id = 1,
                    EstimatedStartTime = DateTime.Now,
                    NumericalOrder = 15,
                    PatientId = 15,
                    PatientName = "Trần Thị Mock data 2",
                    Status = 0,
                    BloodPressure = 232,
                    ClinicalSymptom = "Đau đầu sơ sơ",
                    DepartmentId = 2,
                    Diagnosis = "Ung thư vú",
                    DoctorAdvice = "Đi ngủ sớm",
                    DoctorName = "Nguyễn Trần Lang Băm",
                    EstimatedDate = DateTime.Now,
                    IcdDiseaseId = 344,
                    IcdDiseaseName = "Ung thư cổ tử cung",
                    DoctorId = 14,
                    Pulse = 12323,
                    Temperature = 23,
                    BillId = 2
                    },
                };
                var pagingmodel = new BasePagingViewModel<CheckupDataResponseModel>()
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
        [SwaggerOperation(Summary = "Lấy BỆNH ÁN của bệnh nhân theo id (giả)")]
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
       
    }
}
