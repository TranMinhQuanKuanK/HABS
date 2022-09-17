using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Notification;
using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    public class LoginController : BaseUserController
    {

        private readonly ILoginService _loginService;
        private readonly IFCMTokenService _fcmService;
        private readonly IConfiguration _cfg;

        public LoginController(ILoginService service, IFCMTokenService fcmService, IConfiguration iConfig)
        {
            _cfg = iConfig;
            _loginService = service;
            _fcmService = fcmService;
        }
        [SwaggerOperation(Summary = "Đăng kí Fcm token khi đăng nhập một thiết bị mới")]
        [HttpPost("token")]
        public async Task<IActionResult> RegisterDevice([FromBody] FcmTokenModel model)
        {
            try
            {
                int accountId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    accountId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _fcmService.AddToken(model.TokenId, accountId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Lấy thông tin tài khoản cá nhân")]
        [HttpGet("info")]
        public IActionResult GetMyAccountInfo()
        {
            try
            {
                int accountId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    accountId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                //lấy accountId từ claim
                var user = _loginService.GetAccountInfo(accountId);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Xóa đăng kí Fcm token khi đăng xuất")]
        [HttpDelete("token")]
        public async Task<IActionResult> UnregisterDevice([FromBody] FcmTokenModel model)
        {
            try
            {
                int accountId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    accountId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _fcmService.DeleteToken(model.TokenId, accountId);
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Đăng nhập SĐT-password, default 0978610119 - 123")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel login)
        {
            try
            {
                var user = _loginService.Login(login);
                if (user != null)
                {
                    //Lấy secret từ secret của ứng dụng

                    string tokenString = CreateAuthenToken.GetToken(Role, user.Id, _cfg["AppSecret:UserSecret"]);
                    return Ok(new BaseLoginViewModel<UserLoginViewModel>()
                    {
                        Token = tokenString,
                        Information = user
                    });
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
