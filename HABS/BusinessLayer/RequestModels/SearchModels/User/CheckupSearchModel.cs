using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.SearchModels.User
{
    public class CheckupSearchModel
    {
        public long PatientId { get; set; }
        public DateTime From { get; set; }
    }
}
