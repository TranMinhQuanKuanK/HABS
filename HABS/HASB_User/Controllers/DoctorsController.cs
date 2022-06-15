using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
    [Authorize(Roles = "User")]
    public class DoctorsController : BaseUserController
    {

        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService service)
        {
            _doctorService = service;
        }

    }
}
