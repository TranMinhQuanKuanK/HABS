using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.SearchModels.Cashier

{
    public class BillSearchModel
    {
        public string PatientName { get; set; }
        public int? MinTotal { get; set; }
        public int? MaxTotal { get; set; }

    }
}