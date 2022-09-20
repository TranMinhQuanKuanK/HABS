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
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using BusinessLayer.Services.Redis;
using DataAccessLayer.Models;
using BusinessLayer.ResponseModels.ViewModels.Admin;
using DataAcessLayer;
using Microsoft.Extensions.Configuration;
using BusinessLayer.RequestModels.SearchModels.Admin;
using Utilities;

namespace BusinessLayer.Services.Common
{
    public class ConfigService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;

        private readonly IGenericRepository<Config> _genericRepo;
        private readonly HospitalAppointmentBookingContext _dbContext;

        private readonly IConfiguration _cfg;

        public ConfigService(IDistributedCache distributedCache, IConfiguration configuration)
        {
            _distributedCache = distributedCache;
            _cfg = configuration;
            _redisService = new RedisService(_distributedCache);
            //We init a new db context here because we can't inject a scoped db context into a singletion configService
            var contextOptions = new DbContextOptionsBuilder<HospitalAppointmentBookingContext>()
            .UseSqlServer(_cfg.GetConnectionString("HospitalCloud"))
            .Options;
            var context = new HospitalAppointmentBookingContext(contextOptions);
            _genericRepo = new GenericRepository<Config>(context);
            _dbContext = context;
        }
        private string UpdateRedis_Config(string cfgKey)
        {
            string redisKey = $"config-{cfgKey}";
            var cfgValue = _genericRepo.Get().Where(x => x.Key == cfgKey).FirstOrDefault();
            _redisService.SetValueToKey(redisKey, cfgValue.Value, 5*60);
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
        public List<ConfigViewModel> GetConfigsList(ConfigSearchModel model)
        {
            //get config list
            var configList = _genericRepo
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
            var result = configList
                .Where(x => (!string.IsNullOrEmpty(model.Name))? 
                StringNormalizer.VietnameseNormalize(x.Name)
                .Contains(StringNormalizer.VietnameseNormalize(model.Name)) 
                : true)
                .Where(x => !string.IsNullOrEmpty(model.Key) ? x.Key.Contains(model.Key) : true)
                .Where(x => model.Id != null ? x.Id == model.Id : true)
                .Where(x => model.Type != null ? (int)x.Type == model.Type : true)
                .ToList();
            return result;
        }
        public async Task<string> EditConfigValue(long id, string value)
        {
            var config = _genericRepo.Get().Where(x => x.Id == id).FirstOrDefault();
            if (config == null)
            {
                throw new Exception("Config key doesn't exist");
            }
            config.Value = value;

            await _dbContext.SaveChangesAsync();
            
            UpdateRedis_Config(config.Key);
            return config.Key;
        }
    }
}
