using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.Screen
{
    public class LoginModel
    {
        public string Password { get; set; }
        public long RoomId { get; set; }

    }
}
