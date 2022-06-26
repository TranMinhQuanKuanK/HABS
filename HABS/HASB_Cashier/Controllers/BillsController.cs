﻿using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.Interfaces.Common;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.Cashier;
using BusinessLayer.RequestModels.SearchModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HASB_Cashier.Controllers
{
    [Route(CashierRoute)]
    [ApiController]
    //[ApiExplorerSettings(GroupName = Role)]
    //[Authorize(Roles = "Cashier")]
    public class BillsController : BaseCashierController
    {
        private readonly IBillService _billService;
        public BillsController(IBillService billService)
        {
            _billService = billService;
        }
        [SwaggerOperation(Summary = "Lấy danh sách hóa đơn chưa thanh toán")]
        [HttpGet]
        public IActionResult GetBills([FromQuery] BillSearchModel search, [FromQuery] PagingRequestModel paging)
        {
            try
            {
                var data = _billService.GetBills(search);
                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Xác nhận thanh toán một hóa đơn")]
        [HttpPost("{id}/pay")]
        public IActionResult PayBill(long Id)
        {
            try
            {
                int cashierId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    cashierId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var data = _billService.PayABill(Id,cashierId);
                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Hủy thanh toán một đơn")]
        [HttpDelete("{id}")]
        public IActionResult CancelBill(long Id)
        {
            try
            {
                int cashierId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    cashierId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                var data = _billService.CancelABill(Id, cashierId);
                return Ok(data);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}