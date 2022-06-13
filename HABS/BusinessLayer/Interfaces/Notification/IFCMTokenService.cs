using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Notification
{
    public interface IFCMTokenService
    {
        List<string> GetTokenList(int userId);
        Task AddToken(string tokenId, int userId);
        Task DeleteToken(string tokenId, int userId);

    }
}
