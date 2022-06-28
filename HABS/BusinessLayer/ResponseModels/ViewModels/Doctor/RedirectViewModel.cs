using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class RedirectViewModel
    {
        public string DepartmentName { get; set; }
        public int? NumericalOrder { get; set; }
        public long? RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }
    }
}
