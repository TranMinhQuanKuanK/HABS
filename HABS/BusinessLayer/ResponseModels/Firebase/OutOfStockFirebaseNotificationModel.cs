using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.Firebase
{
    public class OutOfStockFirebaseNotificationModel
    {
        public int BrandId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ClickAction { get; set; }
        public string Screen { get; set; }



    }
}
