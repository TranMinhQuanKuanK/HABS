using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.RequestModels.CreateModels.Admin;
using BusinessLayer.RequestModels.SearchModels.Admin;
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
    [Authorize(Roles = "Admin")]
    public class ConfigsController : BaseAdminController
    {
        private readonly ConfigService _configService;
        private readonly BaseConfig _baseConfig;

        public ConfigsController(ConfigService configService,
             BaseConfig baseConfig)
        {
            _baseConfig = baseConfig;
            _configService = configService;
        }
        [SwaggerOperation(Summary = "Lấy danh sách config")]
        [HttpGet]
        public IActionResult GetConfigs([FromQuery] ConfigSearchModel model)
        {
            try
            {
                var data = _configService.GetConfigsList(model);
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
                string key = await _configService.EditConfigValue(Id, model.Value);
                //this solution for refreshing on memory config will fail when we have 2 or more
                //instance of this app
                _baseConfig.RefreshOnMemoryConfig(key);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
