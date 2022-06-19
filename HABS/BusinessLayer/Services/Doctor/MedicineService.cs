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
    public class MedicineService : BaseService, IMedicineService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public MedicineService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public List<MedicineViewModel> GetMedicines(MedicineSearchModel search)
        {
            //Search???
            List<MedicineViewModel> data = new List<MedicineViewModel>();
            IQueryable<Medicine> tempData;
            tempData = _unitOfWork.MedicineRepository.Get();
            if (!string.IsNullOrEmpty(search.Name))
            {
                tempData = tempData.Where(x => x.Name.Contains(search.Name));
            }
            if (search.CategoryId != null)
            {
                tempData = tempData.Where(x => x.MedicineCategoryId == search.CategoryId);
            }
            data = tempData
                .Include(x => x.MedicineCategory)
                .Select
               (x => new MedicineViewModel()
               {
                   Id = x.Id,
                   MedicineCategory = x.MedicineCategory.Name,
                   Name = x.Name,
                   Status = (int)x.Status,
                   Manufacturer = x.Manufacturer,
                   ManufacturingCountry = x.ManufacturingCountry,
                   Note = x.Note,
                   MedicineCategoryId = x.MedicineCategoryId,
                   Unit = x.Unit,
                   Usage = x.Usage
               }
               ).ToList();
            return data;
        }
    }
}
