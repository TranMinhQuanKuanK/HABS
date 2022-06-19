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

namespace BusinessLayer.Services.User
{
    public class PatientService : BaseService, IPatientService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public PatientService(IUnitOfWork unitOfWork,IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);

        }
        public List<PatientResponseModel> GetPatients(long accountId)
        {
            List<PatientResponseModel> data = new List<PatientResponseModel>();
            data = _unitOfWork.PatientRepository.Get()
                .Where(x => x.AccountId == accountId)
                .Select(x => new PatientResponseModel()
                {
                    Id = x.Id,
                    Address = x.Address,
                    Bhyt = x.Bhyt,
                    DateOfBirth = x.DateOfBirth,
                    Gender = x.Gender,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber
                })
                .ToList();
            return data;
        }
        public PatientResponseModel GetPatientById(long patientId)
        {
            var data = _unitOfWork.PatientRepository.Get()
                .Where(x => x.Id == patientId)
                .Select(x => new PatientResponseModel()
                {
                    Id = x.Id,
                    Address = x.Address,
                    Bhyt = x.Bhyt,
                    DateOfBirth = x.DateOfBirth,
                    Gender = x.Gender,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber
                }).FirstOrDefault();
            return data;
        }
    }
}
