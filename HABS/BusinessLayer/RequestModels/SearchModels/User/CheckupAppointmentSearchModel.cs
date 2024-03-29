﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.SearchModels.User
{
    public class CheckupAppointmentSearchModel
    {
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public long? DepartmentId { get; set; }
        public long? PatientId { get; set; }
        public bool IsFutureReExam { get; set; }
    }
}
