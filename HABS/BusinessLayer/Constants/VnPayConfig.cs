using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Constants
{
    public static class VnPayConfig
    {
        public static int ExpireTime = 15;
        public static string VnpUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public static string Querydr = "http://sandbox.vnpayment.vn/merchant_webapi/merchant.html";
        public static string VnpTmnCode = "XIYQEJD5";
        public static string VnpHashSecret = "XUDKXJIVPEMSMPBEMUYSWLXBEYSQOFCI";
        public static string VnpReturnurl = "https://hasbuser.azurewebsites.net/payments/vnpay";
    }
}
