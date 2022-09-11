using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.RequestModels.SearchModels.Cashier;
using BusinessLayer.Services.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace HASB_Admin.Controllers
{
    [Route(AdminRoute)]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class ConfigsController : BaseAdminController
    {
        private readonly ConfigService _configService;
        public ConfigsController(ConfigService configService)
        {
            _configService = configService;
        }
        [SwaggerOperation(Summary = "Lấy danh sách config")]
        [HttpGet]
        public IActionResult GetConfigs()
        {
            try
            {
                //kiểm tra paging và áp dụng paging
                var data = _configService.GetConfigsList();
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Lấy config theo id")]
        [HttpGet("{id}")]
        public IActionResult GetConfigById(long Id)
        {
            try
            {
                //var data = _billService.GetBillById(Id);
                //return Ok(data);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [SwaggerOperation(Summary = "Thay đổi config")]
        [HttpPost()]
        public async Task<IActionResult> EditConfig()
        {
            try
            {
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
