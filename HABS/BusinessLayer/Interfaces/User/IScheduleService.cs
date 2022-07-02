using BusinessLayer.RequestModels.SearchModels.User;
using BusinessLayer.ResponseModels.SearchModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.User
{
    public interface IScheduleService
    {
        List<CheckupSlotResponseModel> GetAvailableSlots(SlotSearchModel search);
        List<CheckupAppointmentResponseModel> GetCheckupAppointment(CheckupAppointmentSearchModel searchModel);
    }
}
