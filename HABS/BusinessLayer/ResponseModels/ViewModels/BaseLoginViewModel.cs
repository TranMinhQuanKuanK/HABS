using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels
{
    public class BaseLoginViewModel<RoleViewModel> 
    {
        public string Token { get; set; }

        public RoleViewModel User { get; set; }
    }
}
