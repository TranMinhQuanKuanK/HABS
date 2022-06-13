using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Prescription
    {
        public Prescription()
        {
            PrescriptionDetails = new HashSet<PrescriptionDetail>();
        }

        public long Id { get; set; }
        public DateTime? TimeCreated { get; set; }
        public string Note { get; set; }
        public long? CheckupRecordId { get; set; }

        public virtual CheckupRecord CheckupRecord { get; set; }
        public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; }
    }
}
