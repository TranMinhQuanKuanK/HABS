using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.RequestModels.CreateModels.Cashier;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Cashier;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using Utilities;

namespace HASB_Cashier.Controllers
{
    [Route(CashierRoute)]
    [ApiController]
    public class LoginController : BaseCashierController
    {

        private readonly ILoginService _loginService;
        private readonly IConfiguration _cfg;

        public LoginController(ILoginService service, IConfiguration iConfig)
        {
            _cfg = iConfig;
            _loginService = service;
        }
        [SwaggerOperation(Summary = "Đăng nhập bằng mật khẩu/password (default \"kale\"-\"123\")")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }
            var doc = _loginService.Login(model);
            if (doc != null)
            {
                //Lấy secret từ secret của ứng dụng
                string tokenString = CreateAuthenToken.GetToken(Role,doc.Id, _cfg["AppSecret:CashierSecret"]);
                return Ok(new BaseLoginViewModel<CashierLoginViewModel>()
                {
                    Token = tokenString,
                    Information = doc
                });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
