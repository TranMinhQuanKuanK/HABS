using BusinessLayer.Interfaces.Notification;
using DataAccessLayer.Models;
using DataAcessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Notification
{
    public class FCMTokenService : BaseService, IFCMTokenService
    {
        public FCMTokenService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public List<string> GetTokenList(long accountId)
        {
            return _unitOfWork.FcmTokenMobileRepository.Get()
              .Where(x => x.AccountId == accountId).Select(x => x.TokenId).ToList();
        }
        public async Task AddToken(string tokenId, long accountId)
        {
            var token = _unitOfWork.FcmTokenMobileRepository.Get().Where(x => x.AccountId == accountId && x.TokenId == tokenId).FirstOrDefault();
            if (token != null)
            {
                return;
            }
            token = new FcmTokenMobile()
            {
                TokenId = tokenId,
                AccountId = accountId
            };
            await _unitOfWork.FcmTokenMobileRepository.Add(token);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteToken(string tokenId, long accountId)
        {
            try
            {
                var token = _unitOfWork.FcmTokenMobileRepository.Get()
               .Where(x => x.TokenId == tokenId)
               .Where(x => x.AccountId == accountId).FirstOrDefault();
                await _unitOfWork.FcmTokenMobileRepository.Delete(token.Id);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
