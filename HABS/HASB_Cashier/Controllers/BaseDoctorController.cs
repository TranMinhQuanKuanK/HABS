using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HASB_Cashier.Controllers
{
    public class BaseCashierController : BaseController
    {
        protected const string Role = "Cashier";
        protected const string CashierRoute = "api/" + Version + "/[controller]";
    }
}
