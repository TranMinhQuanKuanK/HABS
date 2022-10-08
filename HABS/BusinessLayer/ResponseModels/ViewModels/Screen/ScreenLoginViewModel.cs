using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Screen
{
    public class ScreenLoginViewModel
    {
        public long Id { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }
        public string Note { get; set; }
        public long? DepartmentId { get; set; }
        public long? RoomTypeId { get; set; }
        public string Department { get; set; }
        public string RoomType { get; set; }
        public bool IsCheckupRoom { get; set; }
    }
}
