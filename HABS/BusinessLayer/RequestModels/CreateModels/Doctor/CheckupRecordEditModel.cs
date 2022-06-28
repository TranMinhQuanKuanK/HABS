using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.Doctor
{
    public class CheckupRecordEditModel
    {
        public long Id { get; set; }
        public long PatientId { get; set; }
        public int? Status { get; set; }
        public string Diagnosis { get; set; }
        public string DoctorAdvice { get; set; }
        public int? Pulse { get; set; }
        public int? BloodPressure { get; set; }
        public double? Temperature { get; set; }
        public long? IcdDiseaseId { get; set; }
    }
}
