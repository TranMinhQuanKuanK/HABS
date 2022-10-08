using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.Screen
{
    public class CheckinModel
    {
        public bool IsCheckupRecord { get; set; }
        public string QrCode { get; set; }
    }
}
