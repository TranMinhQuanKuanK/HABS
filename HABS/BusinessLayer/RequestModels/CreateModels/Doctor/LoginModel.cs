using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.RequestModels.CreateModels.Doctor
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public long RoomId { get; set; }

    }
}
