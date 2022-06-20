using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.Doctor
{
    public class PrescriptionCreateModel
    {
        public string Note { get; set; }
        public List<PrescriptionDetailCreateModel> Details { get; set; }
    }
}
