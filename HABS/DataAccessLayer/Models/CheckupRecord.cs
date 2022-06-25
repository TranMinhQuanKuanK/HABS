using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class CheckupRecord
    {
        public enum CheckupRecordStatus
        {
            CHO_TAI_KHAM,
            DA_DAT_LICH,
            DA_THANH_TOAN,
            DANG_KHAM,
            CHO_KQXN,
            DA_CO_KQXN,
            KET_THUC,
            CHUYEN_KHOA,
            NHAP_VIEN,
            DA_HUY,
            DA_XOA
        }
        public CheckupRecord()
        {
            BillDetails = new HashSet<BillDetail>();
            Prescriptions = new HashSet<Prescription>();
            TestRecords = new HashSet<TestRecord>();
        }

        public long Id { get; set; }
        public CheckupRecordStatus Status { get; set; }
        public int? NumericalOrder { get; set; }
        public DateTime? EstimatedStartTime { get; set; }
        public DateTime? EstimatedDate { get; set; }
        public string ClinicalSymptom { get; set; }
        public string Diagnosis { get; set; }
        public string DoctorAdvice { get; set; }
        public int? Pulse { get; set; }
        public int? BloodPressure { get; set; }
        public double? Temperature { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string DepartmentName { get; set; }
        public long? PatientId { get; set; }
        public long? DoctorId { get; set; }
        public long? DepartmentId { get; set; }
        public long? IcdDiseaseId { get; set; }
        public string IcdDiseaseCode { get; set; }
        public string IcdDiseaseName { get; set; }
        public DateTime? Date { get; set; }
        public bool? IsReExam { get; set; }
        public long? RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }

        public virtual Department Department { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual IcdDisease IcdDisease { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Room Room { get; set; }
        public virtual ICollection<BillDetail> BillDetails { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
        public virtual ICollection<TestRecord> TestRecords { get; set; }
    }
}
