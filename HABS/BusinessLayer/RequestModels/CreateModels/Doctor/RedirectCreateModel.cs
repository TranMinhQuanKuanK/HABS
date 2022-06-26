using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.Doctor
{
    public class RedirectCreateModelDetail
    {
        public long DepartmentId { get; set; }
        public string ClinicalSymptom { get; set; }
    }
    public class RedirectCreateModel
    {
        public List<RedirectCreateModelDetail> Details { get; set; }
    }
}
