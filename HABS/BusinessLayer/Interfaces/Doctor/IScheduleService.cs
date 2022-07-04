using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Doctor
{
    public interface IScheduleService
    {
        List<CheckupAppointmentViewModel> UpdateRedis_CheckupQueue(long RoomId);
        List<TestAppointmentViewModel> UpdateRedis_TestQueue(long RoomId, bool isWaitingForResult);
        List<CheckupAppointmentViewModel> GetCheckupQueue(long RoomId);
        List<TestAppointmentViewModel> GetTestQueue(long RoomId, bool isWaitingForResult);
        TestAppointmentViewModel GetItemInTestQueue(long testId);
    }
}
