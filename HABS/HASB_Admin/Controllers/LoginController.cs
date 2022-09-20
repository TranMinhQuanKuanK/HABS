using BusinessLayer.Interfaces.Admin;
using BusinessLayer.RequestModels.CreateModels.Admin;
using BusinessLayer.ResponseModels.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using Utilities;

namespace HASB_Admin.Controllers
{
    [Route(AdminRoute)]
    [ApiController]
    public class LoginController : BaseAdminController
    {

        private readonly ILoginService _loginService;
        private readonly IConfiguration _cfg;
        public LoginController(ILoginService service, IConfiguration iConfig)
        {
            _cfg = iConfig;
            _loginService = service;
        }
        [SwaggerOperation(Summary = "Đăng nhập bằng mật khẩu/password (default \"Admin\"-\"123\")")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var success = _loginService.Login(model);
            if (success)
            {
                //Lấy secret từ secret của ứng dụng
                string tokenString = CreateAuthenToken.GetToken(Role, 0, _cfg["AppSecret:AdminSecret"]);
                return Ok(new {
                    Token = tokenString
                });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
