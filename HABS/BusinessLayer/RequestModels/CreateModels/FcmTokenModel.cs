using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels
{
    public class FcmTokenModel
    {
        public string TokenId { get; set; }
        public int AccountId { get; set; }
    }
}
