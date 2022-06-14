﻿using BusinessLayer.Interfaces.User;
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
    public class SchedulesController : BaseUserController
    {

        private readonly IScheduleService _scheduleService;

        public SchedulesController(IScheduleService service)
        {
            _scheduleService = service;
        }


        [HttpGet]
        //không cần search
        public async Task<IActionResult> Get([FromQuery] long accountId)
        {
            try
            {
                //paging = PagingUtil.checkDefaultPaging(paging);
                //var products = await _checkupRecordService.GetProductList(BrandId, searchModel, paging);
                //return Ok(products);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            //mock data
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            try
            {
                //paging = PagingUtil.checkDefaultPaging(paging);
                //var products = await _checkupRecordService.GetProductList(BrandId, searchModel, paging);
                //return Ok(products);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            //mock data
        }

    }
}
