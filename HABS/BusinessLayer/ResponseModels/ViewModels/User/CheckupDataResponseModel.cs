using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.SearchModels.User
{
    public class CheckupDataResponseModel
    {
        //review lại
        public long Id { get; set; }
        public int Status { get; set; }
        public int? NumericalOrder { get; set; }
        public DateTime? EstimatedStartTime { get; set; }
        public DateTime? ReExamDate { get; set; }
        public string ClinicalSymptom { get; set; }
        public string Diagnosis { get; set; }
        public string DoctorAdvice { get; set; }
        public int? Pulse { get; set; }
        public int? BloodPressure { get; set; }
        public double? Temperature { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public long? PatientId { get; set; }
        public long? DoctorId { get; set; }
        public long? DepartmentId { get; set; }
        public long? IcdDiseaseId { get; set; }
        public string IcdDiseaseName { get; set; }
        public long? BillId { get; set; }
    }
}
