using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.ResponseModels.ViewModels.User;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.User
{
    public interface IUserService
    {
        Task RegisterANewUser(UserCreateEditModel model);
        Task EditUser(long userId, UserCreateEditModel edit);
    }
}
