using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.ResponseModels.Firebase
{
    public class GeneralFirebaseNotificationModel<TDataModel>
    {
        public enum NotiType
        {
            CheckupStatusChangeReminder,
        }
        public NotiType Type { get; set; }
        public TDataModel Data { get; set; }
    }
}
