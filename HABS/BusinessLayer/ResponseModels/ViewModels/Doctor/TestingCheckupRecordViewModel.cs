using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class TestingCheckupRecordViewModel
    {
        public long Id { get; set; }
        public int Status { get; set; }
        public int? NumericalOrder { get; set; }
        public DateTime? EstimatedStartTime { get; set; }
        public string PatientName { get; set; }
        public long? PatientId { get; set; }
        public bool IsReExam { get; set; }
        public List<string> OperationList { get; set; }
    }
}
