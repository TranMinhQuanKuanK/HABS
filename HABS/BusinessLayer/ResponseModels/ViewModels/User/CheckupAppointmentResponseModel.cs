using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.SearchModels.User
{
    //lịch khám của bệnh nhân trong tương lai
    public class CheckupAppointmentResponseModel
    {
        public long Id { get; set; }
        public int Status { get; set; }
        public int? NumericalOrder { get; set; }
        public DateTime? EstimatedStartTime { get; set; }
        public DateTime? EstimatedDate { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public long? PatientId { get; set; }
        public long? DoctorId { get; set; }
        public long? DepartmentId { get; set; }
    }
}
