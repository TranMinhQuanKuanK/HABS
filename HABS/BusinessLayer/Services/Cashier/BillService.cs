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
                .Where(x => string.IsNullOrEmpty(search.SearchTerm) ? true : 
                        x.PhoneNo.Contains(search.SearchTerm)
                        || x.PatientName.Contains(search.SearchTerm)
                        || x.AccountPhoneNo.Contains(search.SearchTerm))
                .Where(x => string.IsNullOrEmpty(search.QrCode) ? true : x.QrCode == search.QrCode)
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
                    QrCode = x.QrCode,
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
                    QrCode = x.QrCode,
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
        public BillViewModel GetBillByQr(string qrCode)
        {
            var bill = _unitOfWork.BillRepository.Get()
                .Where(x => x.QrCode == qrCode)
                .Include(x => x.BillDetails)
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
                    QrCode = x.QrCode,
                    Gender = (int)x.Patient.Gender,
                    PhoneNo = x.PhoneNo,
                    AccountPhoneNo = x.AccountPhoneNo,
                    Details = x.BillDetails.Select(d => new BillDetailViewModel()
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
                    //lấy CheckupRecord
                    cr = _unitOfWork.CheckupRecordRepository.Get()
                        .Include(x=>x.TestRecords)
                        .Include(x=>x.Patient)
                        .Where(x => x.Id == bd.CheckupRecordId).FirstOrDefault();
                    //kiểm tra nếu chưa đặt lịch thì báo lỗi (tái khám)
                    if (cr.Status== CheckupRecordStatus.CHO_TAI_KHAM)
                    {
                        throw new Exception("No schedule for this checkup record");
                    }
                    //thay đổi status cho CR, nếu là tái khám có XN thì là Chờ KQXN (đồng thời đổi status của các TestRecord,
                    //nếu là tái khám thường và khám thường thì là Đã thanh toán
                    if ((bool)cr.IsReExam && cr.TestRecords.Count>0)
                    {
                        foreach (var tr in cr.TestRecords)
                        {
                            tr.Status = TestRecordStatus.DA_THANH_TOAN;
                        }
                        cr.Status = CheckupRecordStatus.CHO_KQXN;
                    } else
                    {
                        cr.Status = CheckupRecordStatus.DA_THANH_TOAN;
                    }
                }
                //nếu là TR
                else if (bd.TestRecordId != null && bd.CheckupRecordId == null)
                {
                    var tr = _unitOfWork.TestRecordRepository.Get()
                        .Where(x => x.Id == bd.TestRecordId).FirstOrDefault();
                    cr = _unitOfWork.CheckupRecordRepository.Get()
                         .Include(x => x.Patient)
                        .Where(x => x.Id == tr.CheckupRecordId).FirstOrDefault();
                    //khi có TR trong bill thì đều đổi CR status thành chờ KQXN
                    cr.Status = CheckupRecordStatus.CHO_KQXN;
                    tr.Status = TestRecordStatus.DA_THANH_TOAN;
                }
            }
            bill.Status = DataAccessLayer.Models.Bill.BillStatus.DA_TT_TIEN_MAT;
            bill.CashierId = cashierId;
            bill.CashierName = cashier.Name;

            await _unitOfWork.SaveChangesAsync();
            //bắn noti cho mobile
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
