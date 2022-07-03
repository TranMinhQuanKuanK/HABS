using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class IncomingTestResponseModel
    {
        public int NumericalOrder { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }
        public long OperationId { get; set; }
        public string OperationName { get; set; }

    }
}
