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
            RefreshAll();
        }
        //public static int ExpireTime = 15;
        public void RefreshAll()
        {
            RefreshExpireTime();
            RefreshVnpUrl();
            RefreshVnpTmnCode();
            RefreshVnpHashSecret();
            RefreshVnpReturnurl();
        }
        public void RefreshSpecific(string cfgKey)
        {
            switch (cfgKey)
            {
                case "VNPAY_EXPIRE_TIME":
                    ExpireTime = int.Parse(_cfgService.GetValueFromConfig("VNPAY_EXPIRE_TIME"));
                    break;
                case "VNPAY_URL":
                    VnpUrl = _cfgService.GetValueFromConfig("VNPAY_URL");
                    break;
                case "VNPAY_TMN_CODE":
                    VnpTmnCode = _cfgService.GetValueFromConfig("VNPAY_TMN_CODE");
                    break;
                case "VNPAY_HASH_SECRET":
                    VnpHashSecret = _cfgService.GetValueFromConfig("VNPAY_HASH_SECRET");
                    break;
                case "VNPAY_RETURN_URL":
                    VnpReturnurl = _cfgService.GetValueFromConfig("VNPAY_RETURN_URL");
                    break;
            }
        }
        #region Refresh function
        public int RefreshExpireTime()
        {
            RefreshSpecific("VNPAY_EXPIRE_TIME");
            return ExpireTime;
        }
        public string RefreshVnpUrl()
        {
            RefreshSpecific("VNPAY_URL");
            return VnpUrl;
        }
        public string RefreshVnpTmnCode()
        {
            RefreshSpecific("VNPAY_TMN_CODE");
            return VnpTmnCode;
        }
        public string RefreshVnpHashSecret()
        {
            RefreshSpecific("VNPAY_HASH_SECRET");
            return VnpHashSecret;
        }
        public string RefreshVnpReturnurl()
        {
            RefreshSpecific("VNPAY_RETURN_URL");
            return VnpReturnurl;
        }
        #endregion

        #region Properties
        public int ExpireTime { get; private set; }
        public string VnpUrl { get; private set; }
        public string VnpTmnCode { get; private set; }
        public string VnpHashSecret { get; private set; }
        public string VnpReturnurl { get; private set; }
        #endregion
        //public static string VnpUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";

        //public static string VnpTmnCode = "XIYQEJD5";

        //public static string VnpHashSecret = "XUDKXJIVPEMSMPBEMUYSWLXBEYSQOFCI";

        //public static string VnpReturnurl = "https://fap.fpt.edu.vn/";


    }
}
