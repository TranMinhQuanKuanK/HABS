using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Screen;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.Screen;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.ResponseModels.ViewModels.Screen;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace HASB_Screen.Controllers
{
    [Route(ScreenRoute)]
    [ApiController]
    public class LoginController : BaseScreenController
    {

        private readonly ILoginService _loginService;
        private readonly IConfiguration _cfg;


        public LoginController(ILoginService service, IConfiguration iConfig)
        {
            _cfg = iConfig;
            _loginService = service;
        }
        [SwaggerOperation(Summary = "Đăng nhập bằng password và id phòng (MK: ABC123)")]
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel model)
        {
            
            if (model == null)
            {
                return BadRequest();
            }
            try
            {
                var room = _loginService.LoginRoom(model.RoomId, model.Password);
                if (room != null)
                {
                    //Lấy secret từ secret của ứng dụng
                    string tokenString = CreateAuthenToken.GetToken(Role, room.Id, _cfg["AppSecret:ScreenSecret"]);
                    return Ok(new BaseLoginViewModel<ScreenLoginViewModel>()
                    {
                        Token = tokenString,
                        Information = room
                    });
                }
                else
                {
                    return Unauthorized();
                }
            } catch (Exception e)
            {
                return Unauthorized();
            }

        }
    }
}
