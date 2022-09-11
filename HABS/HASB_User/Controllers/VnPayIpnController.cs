using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Payment;
using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels.SearchModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace HASB_User.Controllers
{
    [Route(UserRoute)]
    [ApiController]
    public class VnPayIpnController : BaseUserController
    {

        private readonly IVnPayService _vnPayService;
        private readonly ILogger<VnPayIpnController> _logger;

        public VnPayIpnController(IVnPayService vnPayService, ILogger<VnPayIpnController> logger)
        {
            _logger = logger;
            _vnPayService = vnPayService;
        }
        [SwaggerOperation(Summary = "API IPN để VnPay gọi đến")]
        [HttpGet("/payments/vnpay")]
        public async Task<IActionResult> ipnGet(string vnp_TmnCode, string vnp_SecureHash,
            string vnp_txnRef, string vnp_TransactionStatus, string vnp_ResponseCode,
            string vnp_TransactionNo, string vnp_BankCode, string vnp_Amount,
            string vnp_PayDate, string vnp_BankTranNo, string vnp_CardType
            )
        {
            var requestNameValue = System.Web.HttpUtility.ParseQueryString(HttpContext.Request.QueryString.ToString());
            _logger.LogInformation(HttpContext.Request.QueryString.ToString());
            _logger.LogInformation($"VNPAY IPN get request: vnp_txnRef {vnp_txnRef}, " +
                $"vnp_TransactionStatus {vnp_TransactionStatus}, " +
                $"vnp_TransactionNo {vnp_TransactionNo}, " +
                $"vnp_TmnCode {vnp_TmnCode}, " +
                $"vnp_SecureHash {vnp_SecureHash}, " +
                $"vnp_Amount {vnp_Amount}, " +
                $"vnp_BankCode {vnp_BankCode}, " +
                $"vnp_ResponseCode {vnp_ResponseCode}, " +
                $"vnp_PayDate {vnp_PayDate}, " +
                $"vnp_PayDate {vnp_CardType}, " +
                $"");
            var responseCode = await _vnPayService.IpnReceiver(vnp_TmnCode, vnp_SecureHash, vnp_txnRef, vnp_TransactionStatus,
                vnp_ResponseCode, vnp_TransactionNo, vnp_BankCode,
                vnp_Amount, vnp_PayDate, vnp_BankTranNo, vnp_CardType, requestNameValue);
            //_logger.LogInformation("Response code from IPN get request: "+ responseCode);
            switch (responseCode)
            {
                case "00":
                    return Ok(new { RspCode = "00", Message = "Hóa đơn đã được cập nhật thành công" });
                case "01":
                    return Ok(new { RspCode = "01", Message = "Hóa đơn không tìm thấy" });
                case "02":
                    return Ok(new { RspCode = "02", Message = "Bill đã được thanh toán hoặc đã bị hủy" });
                case "97":
                    return Ok(new { RspCode = "97", Message = "Chữ kí không hợp lệ" });
                case "04":
                    return Ok(new { RspCode = "04", Message = "Số tiền không đúng" });
            }
            return BadRequest();
        }
    }
}
