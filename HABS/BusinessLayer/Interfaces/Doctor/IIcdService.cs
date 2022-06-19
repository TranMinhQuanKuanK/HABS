using BusinessLayer.RequestModels.CreateModels.Doctor;
using BusinessLayer.RequestModels.SearchModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Doctor
{
    public interface IIcdService
    {
        List<IcdViewModel> GetIcdList(IcdSearchModel search);
    }
}
