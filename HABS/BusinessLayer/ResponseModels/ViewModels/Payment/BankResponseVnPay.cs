using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Payment
{
    public class BankResponseVnPay
    {
        public string bank_code { get; set; }
        public string bank_name { get; set; }
        public string logo_link { get; set; }
        public int bank_type { get; set; }
        public int display_order { get; set; }
    }
}
