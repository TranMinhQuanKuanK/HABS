﻿using BusinessLayer.RequestModels;
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
        //Lấy phòng available cho PK chuyên khoa hoặc phòng xét nghiệm
        public Room GetAppropriateRoomForOperation(Operation op)
        {
            //tìm các phòng có roomType = operationRoomType
            var roomList = _unitOfWork.RoomRepository
                .Get()
                .Include(x => x.TestRecords)
                .Where(x => x.RoomTypeId == op.RoomTypeId).ToList();
            Room roomWithLeastPeople = null;
            int max = 0;
            //Nhớ: Để constant "Id của Loại phòng khám"
            if (op.RoomTypeId != IdConstant.ID_ROOMTYPE_PHONG_KHAM)
            {
                //Flow cho phòng xét nghiệm
                foreach (var room in roomList)
                {
                    int cur = room.TestRecords.Where(x => ((DateTime)x.Date).Day == DateTime.Now.AddHours(7).Day)
                     .Where(x => x.Status == TestRecord.TestRecordStatus.DA_THANH_TOAN
                      || x.Status == TestRecord.TestRecordStatus.CHO_KET_QUA
                      || x.Status == TestRecord.TestRecordStatus.HOAN_THANH
                      )
                     .ToList().Count;
                    if (cur > max)
                    {
                        roomWithLeastPeople = room;
                        max = cur;
                    }
                }
            }
            else
            {
                //Flow cho phòng khám chuyên khoa
                foreach (var room in roomList)
                {
                    //
                    int cur = room.CheckupRecords
                        .Where(x => ((DateTime)x.Date).Day == DateTime.Now.AddHours(7).Day)
                     .Where(x => x.Status == CheckupRecord.CheckupRecordStatus.CHO_KQXN
                      || x.Status == CheckupRecord.CheckupRecordStatus.CHUYEN_KHOA
                      || x.Status == CheckupRecord.CheckupRecordStatus.DANG_KHAM
                      || x.Status == CheckupRecord.CheckupRecordStatus.DA_CO_KQXN
                      || x.Status == CheckupRecord.CheckupRecordStatus.DA_THANH_TOAN
                      || x.Status == CheckupRecord.CheckupRecordStatus.KET_THUC
                      || x.Status == CheckupRecord.CheckupRecordStatus.NHAP_VIEN
                      || x.Status == CheckupRecord.CheckupRecordStatus.DA_DAT_LICH
                      )
                     .ToList().Count;
                    if (cur > max)
                    {
                        roomWithLeastPeople = room;
                        max = cur;
                    }
                }
            }
            return roomWithLeastPeople;
        }
        //lấy số thứ tự cho phòng xét nghiệm
        public int GetNumOrderForAutoIncreaseRoom(Room room)
        {
            lock (padlock)
            {
                int curNumOfPeople = 0;
                if (room.RoomTypeId != IdConstant.ID_ROOMTYPE_PHONG_KHAM)
                {
                    curNumOfPeople = room.TestRecords.Where(x => ((DateTime)x.Date).Day == DateTime.Now.AddHours(7).Day)
                     .Where(x => x.Status == TestRecord.TestRecordStatus.DA_THANH_TOAN
                      || x.Status == TestRecord.TestRecordStatus.CHO_KET_QUA
                      || x.Status == TestRecord.TestRecordStatus.HOAN_THANH
                      )
                     .ToList().Count;
                }
                else
                {
                    curNumOfPeople = room.CheckupRecords
                       .Where(x => ((DateTime)x.Date).Day == DateTime.Now.AddHours(7).Day)
                    .Where(x => x.Status == CheckupRecord.CheckupRecordStatus.CHO_KQXN
                     || x.Status == CheckupRecord.CheckupRecordStatus.CHUYEN_KHOA
                     || x.Status == CheckupRecord.CheckupRecordStatus.DANG_KHAM
                     || x.Status == CheckupRecord.CheckupRecordStatus.DA_CO_KQXN
                     || x.Status == CheckupRecord.CheckupRecordStatus.DA_THANH_TOAN
                     || x.Status == CheckupRecord.CheckupRecordStatus.KET_THUC
                     || x.Status == CheckupRecord.CheckupRecordStatus.NHAP_VIEN
                     || x.Status == CheckupRecord.CheckupRecordStatus.DA_DAT_LICH
                     )
                    .ToList().Count;
                }
                return curNumOfPeople + 1;
            }
        }
    }
}