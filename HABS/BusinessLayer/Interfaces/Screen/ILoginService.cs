using BusinessLayer.RequestModels.CreateModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Screen
{
    public interface ILoginService
    {
        ScreenLoginViewModel LoginRoom(long roomId, string password);
    }
}
