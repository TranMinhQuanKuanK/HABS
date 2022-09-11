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
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using static DataAccessLayer.Models.Operation;
using BusinessLayer.RequestModels.SearchModels.Doctor;
using DataAccessLayer.Models;
using BusinessLayer.Interfaces.Common;
using BusinessLayer.Constants;

namespace BusinessLayer.Services.Common
{
    public class ConfigService : BaseService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public ConfigService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public string UpdateRedis_Config(string cfgKey, ConfigObjecT???)
        {

            return "???";
        }
        public string GetValueFromConfig(string cfgKey)
        {
            //lấy config từ database và trả về hoặc lấy từ redis
            return "???";
        }
        public void EditConfigValue(string cfgKey, string value)
        {
            //lấy config từ database và trả về
        }
    }
}
