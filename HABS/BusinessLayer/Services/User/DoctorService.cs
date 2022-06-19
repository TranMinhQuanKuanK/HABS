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
    public class DoctorService : BaseService, IDoctorService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public DoctorService(IUnitOfWork unitOfWork,IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);

        }
        public List<DoctorResponseModel> GetDoctors(DateTime? date, long departmentId)
        {
            List<DoctorResponseModel> data = new List<DoctorResponseModel>();
            data = _unitOfWork.ScheduleRepository.Get()
                .Include(x=>x.Doctor)
                .Include(x => x.Room)
                .ThenInclude(x => x.Department)
                .Where(x => x.Weekday == ((DateTime)date).DayOfWeek)
                .Where(x => x.Room.Department.Id == departmentId).Select(x=>new DoctorResponseModel()
                {
                    Id = x.DoctorId,
                    Name = x.Doctor.Name
                })
                .ToList();
            return data;
        }
    }
}
