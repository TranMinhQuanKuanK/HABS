using BusinessLayer.ResponseModels.ViewModels.Doctor;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Notification
{
    public interface INotificationService
    {
        Task SendUpdateCheckupInfoReminder(long recordId, long accountId);
        Task SendDepartmentChangeNoti(List<DepartmentChangeNoti> listDepartment, long accountId);
    }
}
