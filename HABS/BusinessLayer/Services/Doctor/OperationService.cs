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
        //Chỉ xét nghiệm
        public List<OperationViewModel> GetOperations()
        {
            List<OperationViewModel> data = new List<OperationViewModel>();
            data = _unitOfWork.OperationRepository.Get()
                .Where(x=>x.Type==OperationType.XET_NGHIEM)
                .Select
               (x => new OperationViewModel()
               {
                   Id = x.Id,
                   DepartmentId = x.DepartmentId,
                   InsuranceStatus = (int)x.InsuranceStatus,
                   Name = x.Name,
                   Note = x.Note,
                   Price = x.Price,
                   RoomTypeId = x.RoomTypeId,
                   Status = (int)x.Status,
                   Type = (int)x.Type
               }
               ).ToList();
            return data;
        }
        public OperationViewModel GetOperationForDepartment(long depId)
        {
            //sau này cache lại
            var op = _unitOfWork.OperationRepository.Get()
                .Where(x => x.DepartmentId == depId)
            .Select
               (x => new OperationViewModel()
               {
                   Id = x.Id,
                   Name = x.Name,
                   DepartmentId = depId,
                   InsuranceStatus = (int)x.InsuranceStatus,
                   Price = x.Price,
                   RoomTypeId = x.RoomTypeId,
                   Note = x.Note,
                   Type = (int)x.Type,
                   Status = (int)x.Status
               }
               ).FirstOrDefault();
            return op;
        }
    }
}
