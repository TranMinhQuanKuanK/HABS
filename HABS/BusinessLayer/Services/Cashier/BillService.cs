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
                .Where(x => search.PatientId==null ? true : x.PatientId==search.PatientId)
                .Where(x => string.IsNullOrEmpty(search.PatientName) ? true : x.PatientName.Contains(search.PatientName))
                .Where(x => string.IsNullOrEmpty(search.PhoneNo) ? true : x.PhoneNo.Contains(search.PhoneNo))
                .Where(x => search.From == null ? true : x.TimeCreated >= search.From)
                .Where(x => search.To == null ? true : x.TimeCreated <= search.To)
                .Where(x => search.IncludeOldBills ? true : x.Status == DataAccessLayer.Models.Bill.BillStatus.CHUA_TT)
                .OrderBy(x=>x.TimeCreated)
                .Select(x => new BillViewModel()
                {
                    Id = x.Id,
                    PatientName = x.PatientName,
                    Content = x.Content,
                    TimeCreated = x.TimeCreated,
                    Status = (int)x.Status,
                    Total = x.Total,
                    TotalInWord = x.TotalInWord,
                    CashierId = x.CashierId,
                    CashierName = x.CashierName
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
            //check đã thanh toán cho từng record tương ứng của bill detail
            foreach (var bd in bill.BillDetails)
            {
                //nếu là CR
                if (bd.TestRecordId == null && bd.CheckupRecordId != null)
                {

                    var cr = _unitOfWork.CheckupRecordRepository.Get()
                        .Include(x=>x.TestRecords)
                        .Where(x => x.Id == bd.CheckupRecordId).FirstOrDefault();
                    //kiểm tra nếu chưa đặt lịch thì báo lỗi (tái khám)
                    if (cr.Status== CheckupRecordStatus.CHO_TAI_KHAM)
                    {
                        throw new Exception("No schedule for this checkup record");
                    }
                    //nếu là đã tái khám thì 
                    if ((bool)cr.IsReExam)
                    {
                        if (cr.TestRecords.Count>0)
                        {
                            foreach(var tr in cr.TestRecords)
                            {
                                tr.Status = TestRecordStatus.DA_THANH_TOAN;
                            }
                        }
                    }
                    cr.Status = CheckupRecordStatus.DA_THANH_TOAN;
                    //bắn status cho mobile nếu có
                }
                //nếu là TR
                else if (bd.TestRecordId != null && bd.CheckupRecordId == null)
                {
                    var tr = _unitOfWork.TestRecordRepository.Get()
                        .Where(x => x.Id == bd.TestRecordId).FirstOrDefault();
                    var cr = _unitOfWork.CheckupRecordRepository.Get()
                        .Where(x => x.Id == tr.CheckupRecordId).FirstOrDefault();
                    cr.Status = CheckupRecordStatus.CHO_KQXN;
                    tr.Status = TestRecordStatus.DA_THANH_TOAN;
                }
            }
            bill.Status = DataAccessLayer.Models.Bill.BillStatus.TT_TIEN_MAT;
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
