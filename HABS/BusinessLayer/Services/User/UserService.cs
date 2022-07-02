
using DataAcessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using BusinessLayer.Services.Redis;
using BusinessLayer.Interfaces.User;
using BusinessLayer.ResponseModels.SearchModels.User;
using BusinessLayer.RequestModels.SearchModels.User;
using static DataAccessLayer.Models.CheckupRecord;
using BusinessLayer.Constants;
using static DataAccessLayer.Models.Doctor;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.RequestModels.CreateModels.User;
using System.Threading.Tasks;

namespace BusinessLayer.Services.User
{
    public class UserService : BaseService, IUserService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public UserService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);

        }
        public async Task RegisterANewUser(UserCreateEditModel model)
        {
            //check user phone, email
            var preUser = _unitOfWork.AccountRepository.Get()
                .Where(x => x.PhoneNumber == model.PhoneNumber || x.Email == model.Email)
                .Where(x=>x.Status==Account.UserStatus.BINH_THUONG)
                .FirstOrDefault();
            if (preUser != null)
            {
                throw new Exception("Email/phone number existed");
            }
            var user = new Account()
            {
                Name = model.Name,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
            };
            await _unitOfWork.AccountRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();

        }
        public async Task EditUser(long userId, UserCreateEditModel edit)
        {
            //check phone
            var preUser = _unitOfWork.AccountRepository.Get().Where(x => x.PhoneNumber == edit.PhoneNumber).FirstOrDefault();
            if (preUser != null)
            {
                throw new Exception("Phone number is already used!");
            }

            var user = _unitOfWork.AccountRepository.Get().Where(x => x.Id == userId)
                .Where(x=>x.Status==Account.UserStatus.BINH_THUONG)
                .FirstOrDefault();
            if (user == null)
            {
                throw new Exception("User doesn't exist");
            }
            if (!string.IsNullOrEmpty(user.Name))
            {
                user.Name = edit.Name;
            }
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                user.Name = edit.Name;
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteUser(long userId, UserCreateEditModel edit)
        {
            //check user
            var user = _unitOfWork.AccountRepository.Get().Where(x => x.Id == userId).FirstOrDefault();
            if (user == null)
            {
                throw new Exception("User doesn't exist");
            }
            //sửa status
            user.Status = Account.UserStatus.DA_XOA;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
