using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.Doctor;
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
    //[Authorize(Roles = "Doctor")]
    public class MedicinesController : BaseDoctorController
    {

        private readonly IMedicineService _medicineService;

        public MedicinesController(IMedicineService service)
        {
            _medicineService = service;
        }
        [SwaggerOperation(Summary = "Lấy danh sách thuốc")]
        [HttpGet]
        public IActionResult GetMedicines([FromQuery] MedicineSearchModel model)
        {
            try
            {
                var data = _medicineService.GetMedicines(model);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
