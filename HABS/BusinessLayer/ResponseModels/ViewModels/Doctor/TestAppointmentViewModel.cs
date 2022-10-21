using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class TestAppointmentViewModel
    {
        public long Id { get; set; }
        public DateTime? Date { get; set; }
        public int? NumericalOrder { get; set; }
        public string Doctor { get; set; }
        public int Status { get; set; }
        public string PatientName { get; set; }
        public long OperationId { get; set; }
        public string OperationName { get; set; }
        public long? PatientId { get; set; }
        public string ResultFileLink { get; set; }
        public PatientViewModel Patient { get; set; }
        public string QrCode { get; set; }

    }
}
