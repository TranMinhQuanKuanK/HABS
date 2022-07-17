using BusinessLayer.Interfaces.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HASB_Doctor.Controllers
{
    [Route(DoctorRoute)]
    [ApiController]
    [Authorize(Roles = "Doctor")]
    public class SchedulesController : BaseDoctorController
    {

        private readonly IOperationService _operationService;

        public SchedulesController(IOperationService service)
        {
            _operationService = service;
        }
        
    }
}
