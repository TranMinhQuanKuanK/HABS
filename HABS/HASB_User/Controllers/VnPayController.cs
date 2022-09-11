using BusinessLayer.Interfaces.Payment;
using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels.CreateModels.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    [Authorize(Roles = "User")]
    public class VnPayController : BaseUserController
    {

        private readonly IVnPayService _vnPayService;
        private readonly IScheduleService _scheduleService;

        public VnPayController(IVnPayService vnPayService, IScheduleService scheduleService)
        {
            _vnPayService = vnPayService;
            _scheduleService = scheduleService;
        }

        [SwaggerOperation(Summary = "Tạo giao dịch thanh toán VnPay")]
        [HttpPost]
        public async Task<IActionResult> CreateVnPayRequest([FromBody] VnPayCreateModel model)
        {
            try
            {
                var remoteIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                int accountId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    accountId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var url = await _vnPayService.CreateVnPayRequest(model.BillId, accountId, remoteIpAddress);
                return Ok(url);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
