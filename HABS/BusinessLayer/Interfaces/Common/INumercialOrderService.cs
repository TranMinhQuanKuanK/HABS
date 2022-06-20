using BusinessLayer.RequestModels.CreateModels.Doctor;
using BusinessLayer.RequestModels.SearchModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Common
{
    public interface INumercialOrderService
    {
        Room GetAppropriateRoomForOperation(Operation op);
        int GetNumOrderForExaminationRoom(Room room);
    }
}
