using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.ResponseModels.ViewModels.User;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.User
{
    public interface ILoginService
    {
        UserLoginViewModel Login(LoginModel login);
       UserLoginViewModel GetAccountInfo(long accountId);
    }
}
