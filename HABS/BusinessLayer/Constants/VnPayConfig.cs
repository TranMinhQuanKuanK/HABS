using BusinessLayer.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer.Constants
{
    public class VnPayConfig
    {
        private readonly ConfigService _cfgService;
        public VnPayConfig(ConfigService cfgService)
        {
            _cfgService = cfgService;
        }
        //public static int ExpireTime = 15;
        public int ExpireTime
        {
            get
            {
                return int.Parse(_cfgService.GetValueFromConfig("ExpireTime"));
            }
            set
            {
                ExpireTime = value;
            }
        }
        public static string VnpUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public static string Querydr = "http://sandbox.vnpayment.vn/merchant_webapi/merchant.html";
        public static string VnpTmnCode = "XIYQEJD5";
        public static string VnpHashSecret = "XUDKXJIVPEMSMPBEMUYSWLXBEYSQOFCI";
        public static string VnpReturnurl = "https://fap.fpt.edu.vn/";

      
    }
}
