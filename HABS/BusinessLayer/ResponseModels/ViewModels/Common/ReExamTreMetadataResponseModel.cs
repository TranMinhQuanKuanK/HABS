using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Common
{
    public class ReExamTreeResponseModel
    {
        public string Id { get; set; }
        public int TestQuantity { get; set; }
        public int CheckupRecordQuantity { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<string> RelatedDepartments { get; set; }
        public List<ReExamTreeDateItemResponseModel> Details { get; set; }
    }
}
