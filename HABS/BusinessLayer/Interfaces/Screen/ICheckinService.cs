using BusinessLayer.RequestModels.CreateModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Screen
{
    public interface ICheckinService
    {
        Task<List<CheckupAppointmentViewModel>> CheckinForCheckupRecord(string qrCode, long roomId);
        Task<List<TestAppointmentViewModel>> CheckinForTestRecord(string qrCode, long roomId);
    }
}
