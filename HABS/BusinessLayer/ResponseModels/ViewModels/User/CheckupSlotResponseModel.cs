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
        public bool isAvailable { get; set; }
    }
}
