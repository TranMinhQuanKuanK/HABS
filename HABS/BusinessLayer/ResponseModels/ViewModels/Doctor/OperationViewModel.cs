using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Doctor
{
    public class OperationViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int InsuranceStatus { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string Note { get; set; }
        public long? RoomTypeId { get; set; }
        public long? DepartmentId { get; set; }
        public string Department { get; set; }
    }
}
