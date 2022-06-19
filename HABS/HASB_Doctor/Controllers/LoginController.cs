using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "Doctor")]
    public class LoginController : BaseDoctorController
    {

        private readonly ILoginService _loginService;

        public LoginController(ILoginService service)
        {
            _loginService = service;
        }
        [SwaggerOperation(Summary = "Đăng nhập bằng mật khẩu/password và id phòng (default \"doctor\"-\"123123\")")]
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
