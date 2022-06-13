using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.Firebase
{
    public class StoreApprovedRejectedNotificationModel
    {
        public string StoreName { get; set; }
        public int StoreId { get; set; }
        public string ClickAction { get; set; }
        public string Screen { get; set; }
        public int BrandId { get; set; }
    }
}
