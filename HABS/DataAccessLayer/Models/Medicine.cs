using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class Medicine
    {
        public Medicine()
        {
            PrescriptionDetails = new HashSet<PrescriptionDetail>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public string Usage { get; set; }
        public int Status { get; set; }
        public string Unit { get; set; }
        public string Note { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturingCountry { get; set; }
        public long? MedicineCategoryId { get; set; }

        public virtual MedicineCategory MedicineCategory { get; set; }
        public virtual ICollection<PrescriptionDetail> PrescriptionDetails { get; set; }
    }
}
