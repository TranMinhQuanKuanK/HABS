﻿using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.SearchModels.Cashier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HASB_Cashier.Controllers
{
    [Route(CashierRoute)]
    [ApiController]
    [Authorize(Roles = "Cashier")]
    public class BillsController : BaseCashierController
    {
        private readonly IBillService _billService;
        public BillsController(IBillService billService)
        {
            _billService = billService;
        }
        [SwaggerOperation(Summary = "Lấy danh sách hóa đơn chưa thanh toán")]
        [HttpGet]
        public IActionResult GetBills([FromQuery] BillSearchModel search)
        {
            try
            {
                //kiểm tra paging và áp dụng paging
                var data = _billService.GetBills(search);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
        [SwaggerOperation(Summary = "Lấy thông tin hóa đơn theo id hóa đơn")]
        [HttpGet("{id}")]
        public IActionResult GetBillById(long Id)
        {
            try
            {
                var data = _billService.GetBillById(Id);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Lấy thông tin hóa đơn bằng QR")]
        [HttpGet("qr/{qr}")]
        public IActionResult GetBillByQr(string Qr)
        {
            try
            {
                var data = _billService.GetBillByQr(Qr);
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Xác nhận thanh toán một hóa đơn")]
        [HttpPost("{id}/pay")]
        public async Task<IActionResult> PayBill(long Id)
        {
            try
            {
                int cashierId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    cashierId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
               await _billService.PayABill(Id,cashierId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [SwaggerOperation(Summary = "Hủy thanh toán một đơn")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelBill(long Id)
        {
            try
            {
                int cashierId = 0;
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    cashierId = int.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                }
                await _billService.CancelABill(Id, cashierId);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
