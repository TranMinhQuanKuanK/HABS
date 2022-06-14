using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HASB_User.Controllers
{
    public class BaseDoctorController : BaseController
    {
        protected const string Role = "Doctor";
        protected const string DoctorRoute = "api/" + Version + "/[controller]";
    }
}
