﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class TestRecordViewModel
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public int? NumericalOrder { get; set; }
        public int Status { get; set; }
        public string ResultFileLink { get; set; }
        public string PatientName { get; set; }
        public long OperationId { get; set; }
        public string OperationName { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }
        public long? RoomId { get; set; }
        public long? PatientId { get; set; }
        public long? CheckupRecordId { get; set; }
        public long? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string QrCode { get; set; }
        public string ResultDescription { get; set; }
    }
}
