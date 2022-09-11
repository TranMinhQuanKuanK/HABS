using BusinessLayer.Interfaces.Cashier;
using BusinessLayer.Interfaces.Common;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.Interfaces.Notification;
using BusinessLayer.Interfaces.User;
using BusinessLayer.RequestModels.SearchModels.Cashier;
using BusinessLayer.ResponseModels.ViewModels.Cashier;
using BusinessLayer.Services.Redis;
using DataAccessLayer.Models;
using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Models.Bill;
using static DataAccessLayer.Models.CheckupRecord;
using static DataAccessLayer.Models.TestRecord;

namespace BusinessLayer.Services.Cashier
{
    public class BillService : BaseService, IBillService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        private readonly Interfaces.Doctor.IScheduleService _scheduleService;
        private readonly INotificationService _notiService;

        public BillService(IUnitOfWork unitOfWork,
            IDistributedCache distributedCache,
             Interfaces.Doctor.IScheduleService scheduleService,
              INotificationService notiService
            ) : base(unitOfWork)

        {
            _notiService = notiService;
            _scheduleService = scheduleService;
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public List<BillViewModel> GetBills(BillSearchModel search)
        {
            var bills = _unitOfWork.BillRepository.Get()
                .Include(x=>x.Patient)
                .Where(x => search.PatientId==null ? true : x.PatientId==search.PatientId)
                .Where(x => string.IsNullOrEmpty(search.PatientName) ? true : x.PatientName.Contains(search.PatientName))
                .Where(x => string.IsNullOrEmpty(search.PhoneNo) ? true : x.PhoneNo.Contains(search.PhoneNo))
                .Where(x => string.IsNullOrEmpty(search.SearchTerm) ? true : x.PhoneNo.Contains(search.SearchTerm)
                || x.PatientName.Contains(search.SearchTerm)
                || x.AccountPhoneNo.Contains(search.SearchTerm))
                .Where(x => search.From == null ? true : x.TimeCreated >= search.From)
                .Where(x => search.To == null ? true : x.TimeCreated <= search.To)
                .Where(x => search.Status==null ? true : x.Status == (BillStatus)search.Status)
                .Where(x => x.Status!= DataAccessLayer.Models.Bill.BillStatus.HUY)
                .OrderByDescending(x=>x.TimeCreated)
                .Select(x => new BillViewModel()
                {
                    Id = x.Id,
                    PatientName = x.PatientName,
                    PhoneNo = x.PhoneNo,
                    AccountPhoneNo = x.AccountPhoneNo,
                    Content = x.Content,
                    TimeCreated = x.TimeCreated,
                    Status = (int)x.Status,
                    Total = x.Total,
                    TotalInWord = x.TotalInWord,
                    CashierId = x.CashierId,
                    CashierName = x.CashierName,
                    PatientId=(long)x.PatientId,
                    DateOfBirth = x.Patient.DateOfBirth,
                    Gender =(int) x.Patient.Gender,
                })
                .ToList();
            return bills;
        }
        public BillViewModel GetBillById(long id)
        {
            var bill = _unitOfWork.BillRepository.Get()
                .Where(x => x.Id == id)
                .Include(x=>x.BillDetails)
                .OrderBy(x => x.TimeCreated)
                .Select(x => new BillViewModel()
                {
                    Id = x.Id,
                    PatientName = x.PatientName,
                    PatientId = (long)x.PatientId,
                    Content = x.Content,
                    TimeCreated = x.TimeCreated,
                    Status = (int)x.Status,
                    Total = x.Total,
                    TotalInWord = x.TotalInWord,
                    CashierId = x.CashierId,
                    CashierName = x.CashierName,
                    DateOfBirth = x.Patient.DateOfBirth,
                    Gender = (int)x.Patient.Gender,
                    PhoneNo = x.PhoneNo,
                    AccountPhoneNo = x.AccountPhoneNo,
                    Details = x.BillDetails.Select(d=>new BillDetailViewModel()
                    {
                        Id = d.Id,
                        InsuranceStatus = (int)d.InsuranceStatus,
                        OperationId = d.OperationId,
                        OperationName = d.OperationName,
                        Price = d.Price,
                        Quantity = d.Quantity,
                        SubTotal = d.SubTotal,
                    }).ToList()
                })
                .FirstOrDefault();
            return bill;
        }

        public async Task PayABill(long billId, long cashierId)
        {
            long tempRoomForCheckup = 0;
            List<long> tempRoomForTest = new List<long>();
            bool doUpdateTestQueue = false;
            bool doUpdateCheckupQueue = false;

            var cashier = _unitOfWork.CashierRepository
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
            CheckupRecord cr = null;
            foreach (var bd in bill.BillDetails)
            {
                //nếu là CR
                if (bd.TestRecordId == null && bd.CheckupRecordId != null)
                {
                    cr = _unitOfWork.CheckupRecordRepository.Get()
                        .Include(x=>x.TestRecords)
                        .Include(x=>x.Patient)
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
                    if (((DateTime)cr.EstimatedDate).Date==DateTime.Now.AddHours(7).Date)
                    {
                        doUpdateCheckupQueue = true;
                        tempRoomForCheckup = (long)cr.RoomId;
                    }
                    //bắn status cho mobile nếu có
                }
                //nếu là TR
                else if (bd.TestRecordId != null && bd.CheckupRecordId == null)
                {
                    var tr = _unitOfWork.TestRecordRepository.Get()
                        .Where(x => x.Id == bd.TestRecordId).FirstOrDefault();
                    cr = _unitOfWork.CheckupRecordRepository.Get()
                         .Include(x => x.Patient)
                        .Where(x => x.Id == tr.CheckupRecordId).FirstOrDefault();
                    cr.Status = CheckupRecordStatus.CHO_KQXN;
                    tr.Status = TestRecordStatus.DA_THANH_TOAN;
                    doUpdateTestQueue = true;
                    tempRoomForTest.Add((long)tr.RoomId);
                }
            }
            bill.Status = DataAccessLayer.Models.Bill.BillStatus.DA_TT_TIEN_MAT;
            bill.CashierId = cashierId;
            bill.CashierName = cashier.Name;

            await _unitOfWork.SaveChangesAsync();
            //if (doUpdateCheckupQueue)
            //{
            //    _scheduleService.UpdateRedis_CheckupQueue(tempRoomForCheckup);
            //}
            //if (doUpdateTestQueue)
            //{
            //    foreach(var id in tempRoomForTest)
            //    {
            //        _scheduleService.UpdateRedis_TestQueue(id, false);
            //    }
            //}
            await _notiService.SendUpdateCheckupInfoReminder(cr.Id,cr.Patient.AccountId);
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
            if (bill.BillDetails.Count>0)
            {
                bill.BillDetails.ElementAt(0).CheckupRecord.Status =
             CheckupRecordStatus.DA_HUY;
            }
            bill.CashierId = cashierId;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
