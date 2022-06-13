using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Notification
{
    public interface IFCMTokenService
    {
        List<string> GetTokenList(long accountId);
        Task AddToken(string tokenId, long accountId);
        Task DeleteToken(string tokenId, long accountId);

    }
}
