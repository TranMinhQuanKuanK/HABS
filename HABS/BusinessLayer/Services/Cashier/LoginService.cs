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
using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.ResponseModels.ViewModels.Cashier;
using DataAccessLayer.Models;
using BusinessLayer.RequestModels.CreateModels.Cashier;
using BusinessLayer.Constants;

namespace BusinessLayer.Services.Cashier
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
        public CashierLoginViewModel Login(LoginModel login)
        {
            var cashier = _unitOfWork.CashierRepository
                .Get()
                .Where(_doc => _doc.Username == login.Username && _doc.Password == login.Password)
                .Select(_doc => new CashierLoginViewModel()
                {
                    Id = _doc.Id,
                    Name = _doc.Name,
                    Username = _doc.Username,
                })
                .FirstOrDefault();
            if (cashier == null) return null;
            return cashier;
        }
    }

}
