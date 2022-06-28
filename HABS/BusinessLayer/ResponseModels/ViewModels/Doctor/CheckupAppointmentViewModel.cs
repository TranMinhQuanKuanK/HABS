using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    //lịch khám của bệnh nhân trong tương lai
    public class CheckupAppointmentViewModel
    {
        public long Id { get; set; }
        public int Status { get; set; }
        public int? NumericalOrder { get; set; }
        public DateTime? EstimatedStartTime { get; set; }
        public string PatientName { get; set; }
        public long? PatientId { get; set; }
        public bool IsReExam { get; set; }
    }
}
