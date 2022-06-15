﻿using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class PatientDataForCheckupResponseModel
    {
        public PatientResponseModel PatientData { get; set; }
        public List<TestRecordResponseModel> TestRecords { get; set; }
        public PrescriptionResponseModel Prescription { get; set; }
        public long Id { get; set; }
        public int Status { get; set; }
        public int? NumericalOrder { get; set; }
        public string ClinicalSymptom { get; set; }
        public string Diagnosis { get; set; }
        public string DoctorAdvice { get; set; }
        public int? Pulse { get; set; }
        public int? BloodPressure { get; set; }
        public int? Temperature { get; set; }
        public string PatientName { get; set; }
        public long? IcdDiseaseId { get; set; }
        public string IcdDiseaseName { get; set; }
    }
}
