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
                return int.Parse(_cfgService.GetValueFromConfig("VNPAY_EXPIRE_TIME"));
            }
        }
        public string VnpUrl
        {
            get
            {
                return _cfgService.GetValueFromConfig("VNPAY_URL");
            }
        }
        //public static string VnpUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        public string VnpTmnCode
        {
            get
            {
                return _cfgService.GetValueFromConfig("VNPAY_TMN_CODE");
            }
        }
        //public static string VnpTmnCode = "XIYQEJD5";

        public string VnpHashSecret
        {
            get
            {
                return _cfgService.GetValueFromConfig("VNPAY_HASH_SECRET");
            }
        }
        //public static string VnpHashSecret = "XUDKXJIVPEMSMPBEMUYSWLXBEYSQOFCI";

        public string VnpReturnurl
        {
            get
            {
                return _cfgService.GetValueFromConfig("VNPAY_RETURN_URL");
            }
        }
        //public static string VnpReturnurl = "https://fap.fpt.edu.vn/";

      
    }
}
