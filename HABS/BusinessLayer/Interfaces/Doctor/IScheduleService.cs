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
        List<CheckupAppointmentViewModel> GetFinishedCheckupQueue(long RoomId);
        List<CheckupAppointmentViewModel> GetTestingCheckupQueue(long RoomId);
        List<CheckupAppointmentViewModel> UpdateRedis_FinishedCheckupQueue(long RoomId);
        List<CheckupAppointmentViewModel> UpdateRedis_TestingCheckupQueue(long RoomId);
        List<TestRecordViewModel> UpdateRedis_FinishedTestQueue(long RoomId);
        List<TestRecordViewModel> GetFinishedTestQueue(long RoomId);
    }
}
