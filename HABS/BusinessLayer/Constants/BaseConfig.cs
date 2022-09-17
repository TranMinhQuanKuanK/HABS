using BusinessLayer.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Constants
{
    public class BaseConfig
    {
        private readonly ConfigService _cfgSevice;
        public BaseConfig(ConfigService cfgSevice)
        {
            _cfgSevice = cfgSevice;
            VnpayConfig = new VnPayConfig(_cfgSevice);
            WorkingShiftConfig = new WorkingShiftConfig(_cfgSevice);
            AppSecret = new AppSecret(_cfgSevice);
        }
        public VnPayConfig VnpayConfig { get; }
        public WorkingShiftConfig WorkingShiftConfig { get; }
        public AppSecret AppSecret { get; }
    }
}
