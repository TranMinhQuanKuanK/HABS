using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.Cashier;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Cashier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace HASB_Cashier.Controllers
{
    [Route(CashierRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "Doctor")]
    public class LoginController : BaseCashierController
    {

        private readonly ILoginService _loginService;

        public LoginController(ILoginService service)
        {
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
                string tokenString = CreateAuthenToken.GetToken(Role,doc.Id,"Secretttttt@#$@#$@#$@#$23423423423$@#$@#$@#$");
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
