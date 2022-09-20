using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.SearchModels.User
{
    public class CheckupSlotResponseModel
    {
        public int? NumericalOrder { get; set; }
        public DateTime? EstimatedStartTime { get; set; }
        public bool IsAvailable { get; set; }
        public long RoomId { get; set; }
        public string RoomNumber { get; set; }
        public string Floor { get; set; }
        public int Session { get; set; }
    }
}
