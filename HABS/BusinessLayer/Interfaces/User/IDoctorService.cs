using BusinessLayer.ResponseModels.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.User
{
    public interface IDoctorService
    {
        List<DoctorResponseModel> GetDoctorsLBySearchTerm(string searchTerm);
        List<DoctorResponseModel> GetDoctors(DateTime? date, long departmentId);
        List<DateTime> GetDoctorWorkingDay(long doctorId, int maxDateAhead);
    }
}
