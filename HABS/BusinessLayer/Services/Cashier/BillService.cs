﻿using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.Interfaces.Common;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels.SearchModels.Cashier;
using BusinessLayer.ResponseModels.ViewModels.Cashier;
using BusinessLayer.Services.Redis;
using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Models.CheckupRecord;
using static DataAccessLayer.Models.TestRecord;

namespace BusinessLayer.Services.Cashier
{
    public class BillService : BaseService, IBillService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public BillService(IUnitOfWork unitOfWork,
            IDistributedCache distributedCache) : base(unitOfWork)

        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public List<BillViewModel> GetBills(BillSearchModel search)
        {
            var bills = _unitOfWork.BillRepository.Get()
                .Where(x => string.IsNullOrEmpty(search.PatientName) ? true : x.PatientName.Contains(search.PatientName))
                .Where(x => x.Status == DataAccessLayer.Models.Bill.BillStatus.CHUA_TT)
                .Where(x => search.MinTotal == null ? true : x.Total >= search.MinTotal)
                .Where(x => search.MaxTotal == null ? true : x.Total <= search.MaxTotal)
                .Select(x => new BillViewModel()
                {
                    Id = x.Id,
                    PatientName = x.PatientName,
                    Content = x.Content,
                    TimeCreated = x.TimeCreated,
                    Status = (int)x.Status,
                    Total = x.Total,
                    TotalInWord = x.TotalInWord,
                })
                .ToList();
            return bills;
        }
        public async Task PayABill(long billId, long cashierId)
        {
            var cashier = _unitOfWork.BillRepository
                .Get().Where(x => x.Id == cashierId).FirstOrDefault();
            if (cashier == null)
            {
                throw new Exception("Cashier doesn't exist");
            }
            var bill = _unitOfWork.BillRepository
                .Get()
                .Include(x => x.BillDetails)
                .Where(x => x.Id == billId).FirstOrDefault();
            if (bill == null)
            {
                throw new Exception("Bill doesn't exist");
            }
            foreach (var bd in bill.BillDetails)
            {
                if (bd.TestRecordId == null && bd.CheckupRecordId != null)
                {
                    var cr = _unitOfWork.CheckupRecordRepository.Get().Where(x => x.Id == bd.CheckupRecordId).FirstOrDefault();
                    cr.Status = CheckupRecordStatus.DA_THANH_TOAN;
                }
                else if (bd.TestRecordId != null && bd.CheckupRecordId == null)
                {
                    var tr = _unitOfWork.TestRecordRepository.Get().Include(x=>x.CheckupRecord)
                        .Where(x => x.Id == bd.CheckupRecordId).FirstOrDefault();
                    tr.CheckupRecord.Status = CheckupRecordStatus.CHO_KQXN;
                    tr.Status = TestRecordStatus.DA_THANH_TOAN;
                }
            }
            bill.Status = DataAccessLayer.Models.Bill.BillStatus.TT_TIEN_MAT;

            bill.BillDetails.ElementAt(0).CheckupRecord.Status = CheckupRecordStatus.DA_THANH_TOAN;
            bill.CashierId = cashierId;
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task CancelABill(long billId, long cashierId)
        {
            var cashier = _unitOfWork.BillRepository
                .Get().Where(x => x.Id == cashierId).FirstOrDefault();
            if (cashier == null)
            {
                throw new Exception("Cashier doesn't exist");
            }
            var bill = _unitOfWork.BillRepository
                .Get()
                .Include(x => x.BillDetails)
                .ThenInclude(x => x.CheckupRecord)
                .Where(x => x.Id == billId).FirstOrDefault();
            if (bill == null)
            {
                throw new Exception("Bill doesn't exist");
            }
            bill.Status = DataAccessLayer.Models.Bill.BillStatus.HUY;
            bill.BillDetails.ElementAt(0).CheckupRecord.Status =
                DataAccessLayer.Models.CheckupRecord.CheckupRecordStatus.DA_HUY;
            bill.CashierId = cashierId;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
