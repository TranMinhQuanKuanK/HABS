using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class PrescriptionViewModel
    {
        public long Id { get; set; }
        public DateTime? TimeCreated { get; set; }
        public string Note { get; set; }
        public long? CheckupRecordId { get; set; }
        public List<PrescriptionDetailViewModel> Details { get; set; }
    }
}
