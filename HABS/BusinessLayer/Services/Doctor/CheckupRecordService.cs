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

namespace BusinessLayer.Services.Doctor
{
    public class CheckupRecordService : BaseService, ICheckupRecordService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public CheckupRecordService(IUnitOfWork unitOfWork,IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
    }
}
