using BusinessLayer.RequestModels.CreateModels.Cashier;
using BusinessLayer.ResponseModels.ViewModels.Cashier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Cashier
{
    public interface ILoginService
    {
        CashierLoginViewModel Login(LoginModel login);
    }
}
