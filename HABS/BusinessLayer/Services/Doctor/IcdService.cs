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

namespace BusinessLayer.Services.Doctor
{
    public class IcdService : BaseService, IIcdService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public IcdService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public List<IcdViewModel> GetIcdList(IcdSearchModel search)
        {
            //Search???
            List<IcdViewModel> data = new List<IcdViewModel>();
            IQueryable<IcdDisease> tempData;
            tempData = _unitOfWork.IcdDiseaseRepository.Get();
            if (!string.IsNullOrEmpty(search.Code))
            {
                tempData = tempData.Where(x => x.Code.Contains(search.Code));
            }
            if (!string.IsNullOrEmpty(search.Name))
            {
                tempData = tempData.Where(x => x.Name.Contains(search.Name));
            }
            data = tempData
                .Select
               (x => new IcdViewModel()
               {
                   Id = x.Id,
                   Name = x.Name,
                   Code = x.Code
               }
               ).ToList();
            return data;
        }
    }
}
