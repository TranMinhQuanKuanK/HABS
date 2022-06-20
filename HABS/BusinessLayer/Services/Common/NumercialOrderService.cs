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

namespace BusinessLayer.Services.Doctor
{
    public class NumercialOrderService : BaseService, INumercialOrderService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public NumercialOrderService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        private static readonly object padlock = new object();
        public Room GetAppropriateRoomForOperation(Operation op)
        {
            ////tìm các phòng có roomType = operationRoomType
            //var roomList = _unitOfWork.RoomRepository
            //    .Get()
            //    .Include(x => x.TestRecords)
            //    .Where(x => x.RoomTypeId == op.RoomTypeId).ToList();
            //Room roomWithLeastPeople = null;
            //int max = 0;
            //if (op.RoomTypeId != 10002)
            //{
            //    foreach (var room in roomList)
            //    {
            //        //

            //        int cur = room.TestRecords.Where(x => ((DateTime)x.Date).Day == DateTime.Now.AddHours(7).Day)
            //         .Where(x => x.Status == TestRecord.TestRecordStatus.DA_THANH_TOAN)
            //         .ToList().Count;
            //        if (cur > max)
            //        {
            //            roomWithLeastPeople = room;
            //            max = cur;
            //        }
            //    }
            //} else
            //{
            //    foreach (var room in roomList)
            //    {
            //        //
            //        int cur = room.Check.Where(x => ((DateTime)x.Date).Day == DateTime.Now.AddHours(7).Day)
            //         .Where(x => x.Status == TestRecord.TestRecordStatus.DA_THANH_TOAN)
            //         .ToList().Count;
            //        if (cur > max)
            //        {
            //            roomWithLeastPeople = room;
            //            max = cur;
            //        }
            //    }
            //}
            //return roomWithLeastPeople
            return null;
        }
        //lấy số thứ tự cho phòng xét nghiệm
        public int GetNumOrderForExaminationRoom(Room room)
        {
            lock (padlock)
            {
                return room.TestRecords.Where(x => ((DateTime)x.Date).Day == DateTime.Now.AddHours(7).Day)
                   .Where(x => x.Status == TestRecord.TestRecordStatus.DA_THANH_TOAN)
                   .ToList().Count;
            }
        }
    }
}
