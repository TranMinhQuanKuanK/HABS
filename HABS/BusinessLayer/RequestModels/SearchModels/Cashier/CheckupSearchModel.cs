using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.SearchModels.Cashier

{
    public class BillSearchModel
    {
        public int? Status { get; set; }
        public long? PatientId { get; set; }
        public string PhoneNo { get; set; }
        public string PatientName { get; set; }
        public string SearchTerm { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

    }
}