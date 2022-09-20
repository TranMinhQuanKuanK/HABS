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
using DataAccessLayer.Models;
using BusinessLayer.Constants;
using BusinessLayer.RequestModels.CreateModels.Admin;
using BusinessLayer.Interfaces.Admin;

namespace BusinessLayer.Services.Admin
{
    public class LoginService : BaseService, ILoginService
    {
        //config
        private readonly BaseConfig _baseConfig;
        public LoginService(IUnitOfWork unitOfWork,BaseConfig baseConfig) : base(unitOfWork)
        {
            _baseConfig = baseConfig;
        }
        public bool Login(LoginModel login)
        {
            string username = _baseConfig.AppSecret.AdminUsername;
            string password = _baseConfig.AppSecret.AdminPassword;
            if (login.Username == _baseConfig.AppSecret.AdminUsername
                && login.Password == _baseConfig.AppSecret.AdminPassword)
            {
                return true;
            }
            return false;
        }
    }

}
