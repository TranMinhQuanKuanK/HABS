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

namespace BusinessLayer.Services.Common
{
    public class RoomService : BaseService, IRoomService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public RoomService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public List<RoomViewModel> GetRooms(bool isTestRoom)
        {
            List<RoomViewModel> data = new List<RoomViewModel>();
            data = _unitOfWork.RoomRepository
                .Get()
                .Where(x => isTestRoom || x.RoomTypeId == IdConstant.ID_ROOMTYPE_PHONG_KHAM)
                .Select
               (x => new RoomViewModel()
               {
                   Id = x.Id,
                   Floor = x.Floor,
                   Note = x.Note,
                   RoomNumber = x.RoomNumber,
               }
               ).ToList();
            return data;
        }
    }
}
