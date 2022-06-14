using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.SearchModels.User
{
    public class SlotSearchModel
    {
        public DateTime Date { get; set; }
        public long RoomId { get; set; }
        public int? Session { get; set; }
    }
}
