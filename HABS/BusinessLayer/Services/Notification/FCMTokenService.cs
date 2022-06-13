using AutoMapper;
using BusinessLayer.Interfaces.Notification;
using DataAcessLayer.Interfaces;
using DataAcessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Notification
{
    public class FCMTokenService:BaseService, IFCMTokenService
    {
        public FCMTokenService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }
        public List<string> GetTokenList(int userId)
        {
            return _unitOfWork.FcmTokenMobileRepository.Get()
              .Where(x => x.UserId == userId).Select(x=>x.TokenId).ToList();
        }
        public async Task AddToken(string tokenId, int userId)
        {
                FcmtokenMobile token = new FcmtokenMobile()
                {
                    TokenId = tokenId,
                    UserId = userId
                };
                await _unitOfWork.FcmTokenMobileRepository.Add(token);
                await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteToken(string tokenId, int userId)
        {
            try
            {
                var token = _unitOfWork.FcmTokenMobileRepository.Get()
               .Where(x => x.TokenId == tokenId)
               .Where(x => x.UserId == userId).FirstOrDefault();
                await _unitOfWork.FcmTokenMobileRepository.Delete(token.Id);
                await _unitOfWork.SaveChangesAsync();
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
           
        }
    }
}
