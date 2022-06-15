using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "Doctor")]
    public class SchedulesController : BaseDoctorController
    {

        private readonly IOperationService _operationService;

        public SchedulesController(IOperationService service)
        {
            _operationService = service;
        }
        
    }
}
