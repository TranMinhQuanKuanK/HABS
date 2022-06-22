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
    }
}
