using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.ViewModels.User;
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
    public class PatientsController : BaseUserController
    {

        private readonly IPatientService _patientService;

        public PatientsController(IPatientService service)
        {
            _patientService = service;
        }

        [SwaggerOperation(Summary = "Lấy danh sách bệnh nhân của tài khoản (giả)")]
        [HttpGet]
        public async Task<IActionResult> GetPatients([FromQuery] long accountId)
        {
            try
            {
                //paging = PagingUtil.checkDefaultPaging(paging);
                //var products = await _checkupRecordService.GetProductList(BrandId, searchModel, paging);
                //return Ok(products);
                List<PatientResponseModel> result = new List<PatientResponseModel>()
                {
                   new PatientResponseModel()
                   {
                       Id = 12,
                       Address = "Quận 19",
                       Bhyt = "123123-dfdf-sf-df1-231",
                       DateOfBirth = DateTime.Now,
                       Gender = 0,
                       Name = "Quốccc",
                       PhoneNumber = "0978634119",
                   },
                    new PatientResponseModel()
                   {
                       Id = 21,
                       Address = "Quận 9",
                       Bhyt = "12vbfgffhf1-231",
                       DateOfBirth = DateTime.Now,
                       Gender = 0,
                       Name = "Quân Dâu tây",
                       PhoneNumber = "0923713623",
                   },
                     new PatientResponseModel()
                   {
                       Id = 2332,
                       Address = "Sao hỏa",
                       Bhyt = "1231dfdff-sf-df1-231",
                       DateOfBirth = DateTime.Now,
                       Gender = 0,
                       Name = "Thành Phước",
                       PhoneNumber = "0923789123",
                   }
                };
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
            //mock data
        }
        [SwaggerOperation(Summary = "Lấy bệnh nhân theo id (giả)")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById([FromQuery] long accountId, [FromQuery] long patientId)
        {
            try
            {
                //paging = PagingUtil.checkDefaultPaging(paging);
                //var products = await _checkupRecordService.GetProductList(BrandId, searchModel, paging);
                //return Ok(products);
                var result = new PatientResponseModel()
                {
                    Id = 12,
                    Address = "Quận 19",
                    Bhyt = "123123-dfdf-sf-df1-231",
                    DateOfBirth = DateTime.Now,
                    Gender = 0,
                    Name = "Quốccc",
                    PhoneNumber = "0978634119",
                };
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
            //mock data
        }

    }
}
