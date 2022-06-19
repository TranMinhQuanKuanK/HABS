using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class MedicineViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Usage { get; set; }
        public int Status { get; set; }
        public string Unit { get; set; }
        public string Note { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturingCountry { get; set; }
        public long? MedicineCategoryId { get; set; }
        public string MedicineCategory { get; set; }
    }
}
