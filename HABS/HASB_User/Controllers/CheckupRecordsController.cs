using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.User;
using BusinessLayer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Security.Claims;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    //[Authorize(Roles = "User")]
    public class CheckupRecordsController : BaseUserController
    {

        private readonly ICheckupRecordService _checkupRecordService;

        public CheckupRecordsController(ICheckupRecordService service)
        {
            _checkupRecordService = service;
        }

        [SwaggerOperation(Summary = "Lấy metadata BỆNH ÁN của bệnh nhân")]
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
                int accountId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    accountId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var data = _checkupRecordService.GetCheckupRecordMetadata(null, searchModel.FromTime, 
                    searchModel.ToTime, searchModel.DepartmentId, accountId);

                int totalItem = data.Count;
                if (totalItem == 0)
                {
                    return NotFound();
                }

                data = data.Skip((paging.PageIndex - 1) * paging.PageSize)
                    .Take(paging.PageSize).ToList();

                var result = new BasePagingViewModel<PatientRecordMetadataResponseModel>()
                {
                    PageIndex = paging.PageIndex,
                    PageSize = paging.PageSize,
                    TotalItem = totalItem,
                    TotalPage = (int)Math.Ceiling((decimal)totalItem / (decimal)paging.PageSize),
                    Data = data
                };
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Lấy đầy đủ thông tin BỆNH ÁN của bệnh nhân theo record id")]
        [HttpGet("{id}")]
        public IActionResult GetById(long id)
        {
            try
            {
                int accountId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    accountId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var data = _checkupRecordService.GetCheckupRecordFullData(id, accountId,true);
                if (data == null)
                {
                    return NotFound();
                }
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

    }
}
