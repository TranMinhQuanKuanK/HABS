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
using BusinessLayer.Constants;

namespace BusinessLayer.Services.Doctor
{
    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public DepartmentService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public List<DepartmentViewModel> GetDepartments(bool includeGeneral)
        { 
            List<DepartmentViewModel> departmentsData = new List<DepartmentViewModel>();
            departmentsData = _unitOfWork.DepartmentRepository.Get()
                .Where(x=> includeGeneral==true || x.Id != IdConfig.ID_DEPARTMENT_DA_KHOA)
                .Where(x=>x.Status == DataAccessLayer.Models.Department.DepartmentStatus.CO_MO_KHAM)
            .Select
               (x => new DepartmentViewModel()
               {
                   Id = x.Id,
                   Name = x.Name
               }
               ).ToList();
            return departmentsData;
        }
        public DepartmentViewModel GetDepartmentById(long id)
        {
            //sau này cache lại
            var dep = _unitOfWork.DepartmentRepository.Get()
                .Where(x => x.Id == id)
            .Select
               (x => new DepartmentViewModel()
               {
                   Id = x.Id,
                   Name = x.Name
               }
               ).FirstOrDefault();
            return dep;
        }
    }
}
