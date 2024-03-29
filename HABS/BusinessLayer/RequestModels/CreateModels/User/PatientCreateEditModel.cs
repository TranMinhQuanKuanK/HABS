﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.User
{
    public class PatientCreateEditModel
    {
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public int? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Bhyt { get; set; }
    }
}
