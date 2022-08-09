using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.User
{
    public class PatientRecordMetadataResponseModel
    {
        public long Id { get; set; }
        public int Status { get; set; }
        public int? NumericalOrder { get; set; }
        public DateTime? Date { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string DepartmentName { get; set; }
        public bool IsReExam { get; set; }
        public long RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }
        public string QrCode { get; set; }
    }
}
