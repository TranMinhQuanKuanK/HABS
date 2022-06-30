using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.Doctor
{
    public class ReExamCreateModel
    {
        public long PatientId { get; set; }
        public DateTime ReExamDate { get; set; }
        public TestRequestCreateModel RequiredTest { get; set; }
        public string Note { get; set; }
        public long DepartmentId { get; set; }

    }
}
