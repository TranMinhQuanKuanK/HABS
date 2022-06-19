using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.ResponseModels.ViewModels.User;

namespace BusinessLayer.Interfaces.User
{
    public interface ILoginService
    {
        UserLoginViewModel Login(LoginModel login);
    }
}
