using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HASB_User.Controllers
{
    public class BaseUserController : BaseController
    {
        protected const string Role = "User";
        protected const string UserRoute = "api/" + Version + "/[controller]";
    }
}
