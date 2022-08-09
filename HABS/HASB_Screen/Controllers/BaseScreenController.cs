using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HASB_Screen.Controllers
{
    public class BaseScreenController : BaseController
    {
        protected const string Role = "Screen";
        protected const string ScreenRoute = "api/" + Version + "/[controller]";
    }
}
