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
using BusinessLayer.ResponseModels.ViewModels.Admin;

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
        private string UpdateRedis_Config(string cfgKey)
        {
            string redisKey = $"config-{cfgKey}";
            var cfgValue = _unitOfWork.ConfigRepository.Get().Where(x => x.Key == cfgKey).FirstOrDefault();
            _redisService.SetValueToKey(redisKey, cfgValue.Value);
            return cfgValue.Value;
        }
        public string GetValueFromConfig(string cfgKey)
        {
            string resultValue;
            string redisKey = $"config-{cfgKey}";
            string dataFromRedis = _redisService.GetValueFromKey(redisKey);
            if (!String.IsNullOrEmpty(dataFromRedis))
            {
                resultValue = dataFromRedis;
            }
            else
            {
                resultValue = UpdateRedis_Config(cfgKey);
            }

            return resultValue;
        }
        public List<ConfigViewModel> GetConfigsList()
        {
            //get config list
            var configList = _unitOfWork.ConfigRepository
                .Get()
                .Select(x => new ConfigViewModel()
                {
                    Id = x.Id,
                    Description = x.Description,
                    Key = x.Key,
                    Name = x.Name,
                    Type = x.Type == null ? 0 : (int)x.Type,
                    Value = x.Value,
                })
                .ToList();
            return configList;
        }
        public async Task EditConfigValue(string cfgKey, string value)
        {
            var config = _unitOfWork.ConfigRepository.Get().Where(x => x.Key == cfgKey).FirstOrDefault();
            if (config == null)
            {
                throw new Exception("Config key doesn't exist");
            }
            config.Value = value;
            await _unitOfWork.SaveChangesAsync();
            UpdateRedis_Config(cfgKey);
        }
    }
}
