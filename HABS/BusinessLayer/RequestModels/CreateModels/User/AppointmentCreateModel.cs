using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.User
{
    public class AppointmentCreateModel
    {
        public long PatientId { get; set; }
        public int? NumericalOrder { get; set; }
        public DateTime Date { get; set; }
        public long DoctorId { get; set; }
        public string ClinicalSymptom { get; set; }

    }
}
