using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
    public class LoginController : BaseDoctorController
    {

        private readonly ILoginService _loginService;
        private readonly IConfiguration _cfg;

        public LoginController(ILoginService service, IConfiguration iConfig)
        {
            _cfg = iConfig;
            _loginService = service;
        }
        [SwaggerOperation(Summary = "Đăng nhập bằng mật khẩu/password và id phòng (default \"doctor\"-\"123\")")]
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
                string tokenString = CreateAuthenToken.GetToken(Role,doc.Id, _cfg["AppSecret:DoctorSecret"]);
                return Ok(new BaseLoginViewModel<DoctorLoginViewModel>()
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
