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

namespace BusinessLayer.Services.Doctor
{
    public class OperationService : BaseService, IOperationService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public OperationService(IUnitOfWork unitOfWork,IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public List<OperationResponseModel> GetOperations()
        {
            List<OperationResponseModel> data = new List<OperationResponseModel>();
            data = _unitOfWork.OperationRepository.Get().Select
               (x => new OperationResponseModel()
               {
                   Id = x.Id,
                   DepartmentId = x.DepartmentId,
                   InsuranceStatus = x.InsuranceStatus,
                   Name = x.Name,
                   Note = x.Note,
                   Price = x.Price,
                   RoomTypeId = x.RoomTypeId,
                   Status = x.Status,
                   Type = (int)x.Type
               }
               ).ToList();
            return data;
        }
    }
}
