﻿using BusinessLayer.Interfaces.Doctor;
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

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    [Authorize(Roles = "User")]
    public class DepartmentsController : BaseUserController
    {

        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService service)
        {
            _departmentService = service;
        }
        [SwaggerOperation(Summary = "Lấy danh sách khoa")]
        [HttpGet]
        public IActionResult GetDepartments()
        {
            try
            {
                return Ok(_departmentService.GetDepartments(true));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
