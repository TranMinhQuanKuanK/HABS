using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Cashier
{
    public class CashierLoginViewModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
    }
}
