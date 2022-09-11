using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HASB_Admin.Controllers
{
    public class BaseAdminController : BaseController
    {
        protected const string Role = "Admin";
        protected const string AdminRoute = "api/" + Version + "/[controller]";
    }
}
