﻿using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.User
{
    public class PatientRecordFullDataResponseModel
    {
        public List<BillViewModel> Bill { get; set; }
        public PatientResponseModel PatientData { get; set; }
        public List<TestRecordResponseModel> TestRecords { get; set; }
        public PrescriptionResponseModel Prescription { get; set; }
        public long Id { get; set; }
        public int Status { get; set; }
        public int? NumericalOrder { get; set; }
        public DateTime? EstimatedStartTime { get; set; }
        public DateTime? EstimatedDate { get; set; }
        public DateTime? Date { get; set; }
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
        public string IcdCode { get; set; }
        public string IcdDiseaseName { get; set; }
        public bool IsReExam { get; set; }
        public long RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }
        public string RoomType { get; set; }
        public string QrCode { get; set; }
        public bool HasReExam { get; set; }
        public string ReExamNote { get; set; }
    }
}
