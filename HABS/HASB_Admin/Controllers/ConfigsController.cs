using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.RequestModels.CreateModels.Admin;
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

        [SwaggerOperation(Summary = "Thay đổi config")]
        [HttpPost("{id}")]
        public async Task<IActionResult> EditConfig([FromRoute] long Id, [FromBody] ConfigEditModel model)
        {
            try
            {
                await _configService.EditConfigValue(Id, model.Value);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
