using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.SearchModels.Cashier

{
    public class BillSearchModel
    {
        public long? PatientId { get; set; }
        public string PhoneNo { get; set; }
        public string PatientName { get; set; }
        public int? MinTotal { get; set; }
        public int? MaxTotal { get; set; }

    }
}