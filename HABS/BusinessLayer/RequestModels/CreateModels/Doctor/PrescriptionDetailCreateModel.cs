using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.Doctor
{
    public class PrescriptionDetailCreateModel
    {
        public int Quantity { get; set; }
        public string Usage { get; set; }
        public int MorningDose { get; set; }
        public int MiddayDose { get; set; }
        public int EveningDose { get; set; }
        public int NightDose { get; set; }
        public long? MedicineId { get; set; }
    }
}
