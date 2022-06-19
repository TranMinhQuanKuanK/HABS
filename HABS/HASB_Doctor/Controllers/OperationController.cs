using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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
    public class OperationController : BaseDoctorController
    {

        private readonly IOperationService _operationService;

        public OperationController(IOperationService service)
        {
            _operationService = service;
        }
        [SwaggerOperation(Summary = "Lấy các xét nghiệm để yêu cầu bệnh nhân xét nghiệm, search cho operation tính sau ")]
        [HttpGet]
        public async Task<IActionResult> GetOperations()
        {
            try
            {
                //List<OperationResponseModel> data = new List<OperationResponseModel>()
                //{
                //    new OperationResponseModel()
                //    {
                //          Id = 2,
                //          DepartmentId = 2,
                //          InsuranceStatus = 0,
                //          Name = "Chụp X Quang ngực vói công nghệ nano",
                //          Note = "Khó chịu cực kì",
                //          Price = 10000000,
                //          RoomTypeId = 3,
                //          Status = 2,
                //          Type = 2,
                //    },
                //     new OperationResponseModel()
                //    {
                //          Id = 2,
                //          DepartmentId = 2,
                //          InsuranceStatus = 0,
                //          Name = "Xét nghiệm máu với công nghệ tế bào bự thù lù",
                //          Note = "Khó chịu cực kì",
                //          Price = 30000000,
                //          RoomTypeId = 2,
                //          Status = 2,
                //          Type = 2,
                //    },
                //       new OperationResponseModel()
                //    {
                //          Id = 2,
                //          DepartmentId = 2,
                //          InsuranceStatus = 0,
                //          Name = "Nội soi dạ dày bằng tia laze không đau",
                //          Note = "Có lúc hơi đau",
                //          Price = 20000000,
                //          RoomTypeId = 6,
                //          Status = 2,
                //          Type = 2,
                //    },
                //};
                var data = _operationService.GetOperations();
                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
