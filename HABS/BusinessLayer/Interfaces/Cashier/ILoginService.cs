using BusinessLayer.RequestModels.CreateModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Admin
{
    public interface ILoginService
    {
        bool Login(LoginModel login);
    }
}
