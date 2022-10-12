using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.User
{
    public class AppointmenAfterBookingResponseModel
    {
        public string DoctorName { get; set; }
        public int NumericalOrder { get; set; }
        public string Floor { get; set; }
        public string RoomNumber { get; set; }
        public string DepartmentName { get; set; }
        public DateTime Date { get; set; }
        public string PatientName { get; set; }
        public string ClinicalSymptom { get; set; }
    }
}
