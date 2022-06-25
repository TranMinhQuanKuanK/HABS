using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels;
using BusinessLayer.RequestModels.SearchModels;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.Services;
using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using BusinessLayer.Services.Redis;
using Newtonsoft.Json;
using BusinessLayer.Interfaces.User;
using BusinessLayer.ResponseModels.ViewModels.User;
using DataAccessLayer.Models;
using BusinessLayer.RequestModels.CreateModels.User;
using BusinessLayer.Constants;

namespace BusinessLayer.Services.User
{
    public class LoginService : BaseService, ILoginService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public LoginService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public UserLoginViewModel Login(LoginModel login)
        {
            var user = _unitOfWork.AccountRepository
                .Get()
                .Where(x => x.PhoneNumber == login.PhoneNo && x.Password == login.Password)
                .Select(x => new UserLoginViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email
                })
                .FirstOrDefault();
            return user;
        }
        public UserLoginViewModel GetAccountInfo(long accountId)
        {
            var user = _unitOfWork.AccountRepository
                .Get()
                .Where(x => x.Id == accountId)
                .Select(x => new UserLoginViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email
                })
                .FirstOrDefault();
            return user;
        }
    }

}
