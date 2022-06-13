using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class CheckupRecord
    {
        public CheckupRecord()
        {
            BillDetails = new HashSet<BillDetail>();
            Prescriptions = new HashSet<Prescription>();
            TestRecords = new HashSet<TestRecord>();
        }

        public long Id { get; set; }
        public int Status { get; set; }
        public int? NumericalOrder { get; set; }
        public DateTime? EstimatedStartTime { get; set; }
        public DateTime? EstimatedDate { get; set; }
        public string ClinicalSymptom { get; set; }
        public string Diagnosis { get; set; }
        public string DoctorAdvice { get; set; }
        public int? Pulse { get; set; }
        public int? BloodPressure { get; set; }
        public int? Temperature { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public long? PatientId { get; set; }
        public long? DoctorId { get; set; }
        public long? DepartmentId { get; set; }
        public long? IcdDiseaseId { get; set; }
        public string IcdDiseaseName { get; set; }
        public long? BillId { get; set; }

        public virtual Bill Bill { get; set; }
        public virtual Department Department { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual IcdDisease IcdDisease { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<TestRecord> TestRecords { get; set; }
    }
}
