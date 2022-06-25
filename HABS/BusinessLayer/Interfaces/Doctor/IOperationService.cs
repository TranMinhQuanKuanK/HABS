using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Doctor
{
    public interface IOperationService
    {
        List<OperationViewModel> GetOperations();
        OperationViewModel GetOperationForDepartment(long depId);
    }
}
