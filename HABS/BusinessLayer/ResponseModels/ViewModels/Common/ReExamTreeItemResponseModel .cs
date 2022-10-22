using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.ViewModels.Common
{
    public class ReExamTreeCheckupRecordItemResponseModel
    {
        public string Department { get; set; }
        public string Doctor { get; set; }
        public int TestQuantity { get; set; }
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public DateTime? Date { get; set; }
    }
    public class ReExamTreeDateItemResponseModel
    {
        public DateTime? Date { get; set; }
        public List<ReExamTreeCheckupRecordItemResponseModel> CheckupRecords { get; set; }

    }
}
