using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    [Authorize(Roles = "User")]
    public class UserController : BaseUserController
    {

        private readonly IUserService _userService;

        public UserController(IUserService service)
        {
            _userService = service;
        }
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Đăng kí một người dùng mới")]
        [HttpPost]
        public async Task<IActionResult> RegisterNewAccount([FromBody] UserCreateEditModel model)
        {
            try
            {
                await _userService.RegisterANewUser(model);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [SwaggerOperation(Summary = "Thay đổi thông tin người dùng")]
        [HttpPut]
        public async Task<IActionResult> EditUser([FromBody] UserCreateEditModel model)
        {
            try
            {
                int userId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    userId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _userService.EditUser(userId,model);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
