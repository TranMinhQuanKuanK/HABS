﻿using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Patient
    {
        public enum PatientStatus
        {
            HOAT_DONG,
            DA_XOA
        }
        public enum GenderEnum
        {
            MALE,
            FEMALE,
            NOT_SPECIFIED
        }
        public Patient()
        {
            Bills = new HashSet<Bill>();
            CheckupRecords = new HashSet<CheckupRecord>();
            TestRecords = new HashSet<TestRecord>();
        }

        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public GenderEnum Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Bhyt { get; set; }
        public long AccountId { get; set; }
        public PatientStatus? Status { get; set; }
        public bool? IsTestPatient { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
        public virtual ICollection<CheckupRecord> CheckupRecords { get; set; }
        public virtual ICollection<TestRecord> TestRecords { get; set; }
    }
}
