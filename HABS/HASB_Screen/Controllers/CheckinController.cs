using BusinessLayer.Interfaces.Screen;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.Screen;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Screen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;

namespace HASB_Screen.Controllers
{
    [Route(ScreenRoute)]
    [ApiController]
    public class CheckinController : BaseScreenController
    {

        private readonly ICheckinService _checkinService;

        public CheckinController(ICheckinService service)
        {
            _checkinService = service;
        }
        [SwaggerOperation(Summary = "Checkin phòng khám hoặc phòng xét nghiệ (giả)")]
        [HttpPost]
        public async Task<IActionResult> CheckinCheckupRoom([FromBody] CheckinModel model)
        {
            //sau enable lên lại
            try
            {
                long roomId = 0;
                ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    roomId = long.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                if (model == null)
                {
                    return BadRequest();
                }
                await _checkinService.Checkin(model.QrCode, roomId);
                return Ok();
            } catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
