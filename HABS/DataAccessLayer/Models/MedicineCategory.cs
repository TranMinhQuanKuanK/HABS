using System;
using System.Collections.Generic;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class MedicineCategory
    {
        public MedicineCategory()
        {
            Medicines = new HashSet<Medicine>();
        }

        public long Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Medicine> Medicines { get; set; }
    }
}
