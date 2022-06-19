using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class PrescriptionDetailViewModel
    {
        public long Id { get; set; }
        public int Quantity { get; set; }
        public string Usage { get; set; }
        public string Unit { get; set; }
        public int MorningDose { get; set; }
        public int MiddayDose { get; set; }
        public int EveningDose { get; set; }
        public int NightDose { get; set; }
        public string MedicineName { get; set; }
        public long? PrescriptionId { get; set; }
        public long? MedicineId { get; set; }
    }
}
