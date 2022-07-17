using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class RoomViewModel
    {
        public long Id { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }
        public string Note { get; set; }
        public long RoomTypeId { get; set; }
        public long? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string RoomTypeName { get; set; }
        public bool IsGeneralRoom { get; set; }
    }
}
