using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Cashier
{
    public class BillViewModel
    {
        public long Id { get; set; }
        public DateTime? TimeCreated { get; set; }
        public int Total { get; set; }
        public string TotalInWord { get; set; }
        public int Status { get; set; }
        public string Content { get; set; }
        public string PatientName { get; set; }
        public long PatientId { get; set; }
        public int Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string CashierName { get; set; }
        public long? CashierId { get; set; }
        public List<BillDetailViewModel> Details { get; set; }
    }
}
