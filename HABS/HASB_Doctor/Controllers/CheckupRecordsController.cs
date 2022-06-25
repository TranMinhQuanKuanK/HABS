﻿using BusinessLayer.Interfaces.Doctor;
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
using System.Threading.Tasks;

namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "Doctor")
    public class CheckupRecordsController : BaseDoctorController
    {

        private readonly ICheckupRecordService _checkupRecordService;

        public CheckupRecordsController(ICheckupRecordService service)
        {
            _checkupRecordService = service;
        }

        [SwaggerOperation(Summary = "Lấy lịch sử BỆNH ÁN của bệnh nhân, chỉ trả metadata ")]
        [HttpGet]
        public IActionResult GetCheckupRecord([FromQuery] CheckupAppointmentSearchModel searchModel, [FromQuery] PagingRequestModel paging)
        {
            paging = PagingUtil.checkDefaultPaging(paging);
            if (searchModel is null)
            {
                throw new ArgumentNullException(nameof(searchModel));
            }
            try
            {
                var data = _checkupRecordService.GetCheckupRecordMetadata(searchModel.PatientId, searchModel.FromTime, searchModel.ToTime, searchModel.DepartmentId);
                
                int totalItem = data.Count;
                if (totalItem == 0)
                {
                    return NotFound();
                }

                data = data.Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Take(paging.PageSize).ToList();

                var result = new BasePagingViewModel<PatientRecordMetadataViewModel>()
                {
                    PageIndex = paging.PageIndex,
                    PageSize = paging.PageSize,
                    TotalItem = totalItem,
                    TotalPage = (int)Math.Ceiling((decimal)totalItem / (decimal)paging.PageSize),
                    Data = data
                };
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        
        [SwaggerOperation(Summary = "Lấy BỆNH ÁN của bệnh nhân theo id, đầy đủ thông tin (chưa check được patientId có hợp lệ ko)")]
        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            try
            {
                var data = _checkupRecordService.GetCheckupRecordFullData(id);
                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [SwaggerOperation(Summary = "Chỉnh sửa bệnh án của bệnh nhân (chỉ gửi những field cần edit) (giả)")]
        [HttpPut("{id}")]
        public IActionResult ChangeCheckupRecord([FromBody]CheckupRecordEditModel model)
        {
            try
            {
                //var data = _checkupRecordService.GetCheckupRecordFullData(id);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Đặt thêm lịch xét ngiệm cho bệnh nhân (giả)")]
        [HttpPost("{id}/tests")]
        public IActionResult ArrangeTest([FromBody] TestRequestCreateModel model)
        {
            try
            {
                //var data = _checkupRecordService.GetCheckupRecordFullData(id);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Chuyển khoa cho bệnh nhân (giả)")]
        [HttpPost("{id}/redirect")]
        public IActionResult RedirectPatient([FromBody] RedirectCreateModel model)
        {
            try
            {
                //var data = _checkupRecordService.GetCheckupRecordFullData(id);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Tạo đơn thuốc cho bệnh nhân (giả)")]
        [HttpPost("{id}/prescription")]
        public IActionResult CreatePrescriptin([FromBody] PrescriptionCreateModel model)
        {
            try
            {
                //var data = _checkupRecordService.GetCheckupRecordFullData(id);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
