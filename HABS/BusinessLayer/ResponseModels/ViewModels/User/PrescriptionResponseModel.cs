using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.User
{
    public class PrescriptionResponseModel
    {
        public long Id { get; set; }
        public DateTime? TimeCreated { get; set; }
        public string Note { get; set; }
        public long? CheckupRecordId { get; set; }
        public List<PrescriptionDetailResponseModel> Details { get; set; }
    }
}
