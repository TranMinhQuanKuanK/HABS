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
using BusinessLayer.ResponseModels.ViewModels.Common;

namespace BusinessLayer.Services.Common
{
    public class ReExamTreeService : BaseService, IReExamTreeService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public ReExamTreeService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        //public List<ReExamTreeResponseModel> GetReExamTreeList(Operation op, bool isTestRoom)
        //{
        // Tối ưu tương lai: Tạo một service 
        // Expect: nhận vào Id của 1 Record
        //}
        public ReExamTreeResponseModel GetReExamTree(string reExamTreeId)
        {
            //TODO: Tối ưu TestQuantity vào table CR, khi đó khỏi include rồi count
            int checkupRecordQuantity = 0, testQuantity = 0;
            List<string> relatedDepartments = new List<string>();
            var itemList = _unitOfWork.CheckupRecordRepository.Get()
                .Include(x => x.TestRecords)
                .Where(x => x.ReExamTreeCode == reExamTreeId)
                .Where(x => x.Status != CheckupRecord.CheckupRecordStatus.CHO_TAI_KHAM &&
                x.Status != CheckupRecord.CheckupRecordStatus.DA_XOA &&
                 x.Status != CheckupRecord.CheckupRecordStatus.DA_HUY)
                .OrderBy(x => x.Date)
                .Select(x => new ReExamTreeCheckupRecordItemResponseModel()
                {
                    Id = x.Id,
                    ParentId = x.ParentRecordId,
                    Date = x.Date,
                    Department = x.DepartmentName,
                    Doctor = x.DoctorName,
                    TestQuantity = x.TestRecords.Count(),
                }).ToList();
            if (itemList.Count() == 0)
            {
                return null;
            }
            //map ngày và list CR
            var dateDictionary = new Dictionary<DateTime, List<ReExamTreeCheckupRecordItemResponseModel>>();
            DateTime startDate = (DateTime)itemList[0].Date?.Date;
            DateTime endDate = (DateTime)itemList[0].Date?.Date;
            foreach (var item in itemList)
            {
                testQuantity += item.TestQuantity;
                checkupRecordQuantity++;
                relatedDepartments.Add(item.Department);
                var dateKey = (DateTime)item.Date?.Date;
                List<ReExamTreeCheckupRecordItemResponseModel> listCR = null;
                if (!dateDictionary.TryGetValue(dateKey, out listCR))
                {
                    listCR = new List<ReExamTreeCheckupRecordItemResponseModel>() { item };
                    dateDictionary.Add(dateKey, listCR);
                    if (dateKey < startDate)
                    {
                        startDate = dateKey;
                    }
                    if (dateKey > endDate)
                    {
                        endDate = dateKey;
                    }
                }
                else
                {
                    listCR.Add(item);
                }
            }
            var details = new List<ReExamTreeDateItemResponseModel>();
            foreach (var item in dateDictionary)
            {
                details.Add(new ReExamTreeDateItemResponseModel()
                {
                    Date = item.Key,
                    CheckupRecords = item.Value
                });
            }
            var result = new ReExamTreeResponseModel()
            {
                Details = details,
                CheckupRecordQuantity = checkupRecordQuantity,
                TestQuantity = testQuantity,
                RelatedDepartments = relatedDepartments,
                StartDate = startDate,
                EndDate = endDate,
                Id = reExamTreeId,
            };
            return result;
        }
        //lấy số thứ tự tự tăng cho phòng xét nghiệm 
        public int GetNumOrderForAutoIncreaseRoom(Room room, DateTime date)
        {
            int curNumOfPeople = 0;
            if (room.RoomTypeId != IdConfig.ID_ROOMTYPE_PHONG_KHAM)
            {
                curNumOfPeople = room.TestRecords.Where(x => x.Date != null ? ((DateTime)x.Date).Day == date.Day : false)
                 .Where(x => x.Status == TestRecord.TestRecordStatus.DA_THANH_TOAN
                  || x.Status == TestRecord.TestRecordStatus.CHO_KET_QUA
                  || x.Status == TestRecord.TestRecordStatus.DANG_TIEN_HANH
                  || x.Status == TestRecord.TestRecordStatus.HOAN_THANH
                  || x.Status == TestRecord.TestRecordStatus.CHECKED_IN
                  || x.Status == TestRecord.TestRecordStatus.DA_DAT_LICH
                  ).Where(x => x.RoomId == room.Id)
                 .ToList().Count;
            }
            else
            {
                curNumOfPeople = room.CheckupRecords
                .Where(x => x.Date != null ? ((DateTime)x.Date).Day == date.Day : false)
                .Where(x => x.Status == CheckupRecord.CheckupRecordStatus.CHO_KQXN
                 || x.Status == CheckupRecord.CheckupRecordStatus.CHUYEN_KHOA
                 || x.Status == CheckupRecord.CheckupRecordStatus.DANG_KHAM
                 || x.Status == CheckupRecord.CheckupRecordStatus.DA_CO_KQXN
                 || x.Status == CheckupRecord.CheckupRecordStatus.CHECKED_IN
                 || x.Status == CheckupRecord.CheckupRecordStatus.CHECKED_IN_SAU_XN
                 || x.Status == CheckupRecord.CheckupRecordStatus.DA_THANH_TOAN
                 || x.Status == CheckupRecord.CheckupRecordStatus.KET_THUC
                 || x.Status == CheckupRecord.CheckupRecordStatus.NHAP_VIEN
                 || x.Status == CheckupRecord.CheckupRecordStatus.DA_DAT_LICH
                 ).Where(x => x.RoomId == room.Id)
                .ToList().Count;
            }
            return curNumOfPeople + 1;
        }
    }
}
