using BusinessLayer.Constants;
using BusinessLayer.Interfaces.Payment;
using BusinessLayer.ResponseModels.ViewModels.Payment;
using DataAccessLayer.Models;
using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static DataAccessLayer.Models.CheckupRecord;
using static DataAccessLayer.Models.TestRecord;

namespace BusinessLayer.Services.Payment.VnPay
{
    public class VnPayService : BaseService, IVnPayService
    {
        private readonly VnPayConfig _vnpayConfig;
        public VnPayService(IUnitOfWork unitOfWork, VnPayConfig vnpayConfig) : base(unitOfWork)
        {
            _vnpayConfig = vnpayConfig;
        }
        public async Task<string> CreateVnPayRequest(long billId, long accountId, string remoteIpAddress)
        {
            var bill = _unitOfWork.BillRepository.Get().Include(x => x.Patient)
                .Where(x => x.Id == billId && x.Patient.AccountId == accountId).FirstOrDefault();
            if (bill == null)
            {
                throw new Exception("Bill doesn't exist");
            }
            //xét status

            string vnp_Returnurl = VnPayConfig.VnpReturnurl; //Redirect đến URL này sau khi giao dịch được thực hiện. Code một trang riêng để thông báo kết quả thanh toán cho bệnh nhân.
            string vnp_Url = VnPayConfig.VnpUrl; //URL thanh toán của VNPAY.
            string vnp_TmnCode = VnPayConfig.VnpTmnCode; //Mã website
            string vnp_HashSecret = VnPayConfig.VnpHashSecret; //Key để hash

            long ORDER_ID = billId; // Giả lập mã giao dịch merchant gửi cho VNPAY
            long AMOUNT = bill.Total; // Số tiền cần thanh toán
            DateTime CREATED_DATE = DateTime.Now.AddHours(7); // Thời gian tạo hóa đơn
            int EXPIRED_MIN = _vnpayConfig.ExpireTime; // Thời gian tối đa để thanh toán trước khi hết hạn.
            string MOBILE = bill.PhoneNo; // SĐT bệnh nhân 
            string FULL_NAME = bill.PatientName; // Tên bệnh nhân

            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (AMOUNT * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", CREATED_DATE.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");

            vnpay.AddRequestData("vnp_IpAddr", remoteIpAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", bill.Content);
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", ORDER_ID.ToString());
            vnpay.AddRequestData("vnp_ExpireDate", CREATED_DATE.AddMinutes(EXPIRED_MIN).ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_Bill_Mobile", MOBILE.Trim());
            vnpay.AddRequestData("vnp_Bill_Country", "VN");

            var fullName = FULL_NAME.Trim();
            if (!String.IsNullOrEmpty(fullName))
            {
                var indexof = fullName.IndexOf(' ');
                vnpay.AddRequestData("vnp_Bill_FirstName", fullName.Substring(0, indexof));
                vnpay.AddRequestData("vnp_Bill_LastName", fullName.Substring(indexof + 1, fullName.Length - indexof - 1));
            }
            return vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        }
        private async Task<BankResponseVnPay> GetBank(string bankCode)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("tmn_code", VnPayConfig.VnpTmnCode),
            });
            HttpClient client = new HttpClient();
            var response = await client.PostAsync(
                "https://sandbox.vnpayment.vn/qrpayauth/api/merchant/get_bank_list", formContent);
            List<BankResponseVnPay> bankList = (List<BankResponseVnPay>)await
                response.Content.ReadFromJsonAsync(new List<BankResponseVnPay>().GetType());
            return bankList.Where(x => x.bank_code == bankCode).FirstOrDefault();
        }
        public async Task<string> IpnReceiver(string vnp_TmnCode, string vnp_SecureHash,
            string vnp_txnRef, string vnp_TransactionStatus, string vnp_ResponseCode,
            string vnp_TransactionNo, string vnp_BankCode, string vnp_Amount,
            string vnp_PayDate, string vnp_BankTranNo, string vnp_CardType, NameValueCollection requestNameValue)
        {
            VnPayLibrary vnpay = new VnPayLibrary();
            foreach (string s in requestNameValue)
            {
                //get all querystring data
                if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(s, requestNameValue[s]);
                }
            }
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, VnPayConfig.VnpHashSecret);
            if (checkSignature)
            {
                long billId = long.Parse(vnp_txnRef);
                var bill = _unitOfWork.BillRepository.Get().Include(x=>x.BillDetails).Where(x => x.Id == billId).FirstOrDefault();
                if (bill == null)
                {
                    //Bill không tìm thấy
                    return "01";
                }
                if (bill.Status == Bill.BillStatus.DA_TT_EBANKING
                    || bill.Status == Bill.BillStatus.DA_TT_TIEN_MAT
                    || bill.Status == Bill.BillStatus.HUY)
                {
                    //Bill đã thanh toán hoặc bị hủy
                    return "02";
                }
                if (bill.Total != long.Parse(vnp_Amount.Substring(0, vnp_Amount.Length - 2)))
                {
                    //Số tiền không đúng
                    return "04";
                }
                //sửa thêm thông tin của bill (sửa lại nếu sau scaffold bill)
                var bank = await GetBank(vnp_BankCode);
                bill.BankCode = vnp_BankCode;

                //https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                if (vnp_ResponseCode == "00")
                {
                    bill.Status = Bill.BillStatus.DA_TT_EBANKING;
                    foreach (var bd in bill.BillDetails)
                    {
                        //nếu là CR
                        if (bd.TestRecordId == null && bd.CheckupRecordId != null)
                        {
                            var cr = _unitOfWork.CheckupRecordRepository.Get()
                                .Include(x => x.TestRecords)
                                .Include(x => x.Patient)
                                .Where(x => x.Id == bd.CheckupRecordId).FirstOrDefault();
                            //kiểm tra nếu chưa đặt lịch thì báo lỗi (tái khám)
                            if (cr.Status == CheckupRecordStatus.CHO_TAI_KHAM)
                            {
                                throw new Exception("No schedule for this checkup record");
                            }
                            //nếu là đã tái khám thì 
                            if ((bool)cr.IsReExam)
                            {
                                if (cr.TestRecords.Count > 0)
                                {
                                    foreach (var tr in cr.TestRecords)
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
                                 .Include(x => x.Patient)
                                .Where(x => x.Id == tr.CheckupRecordId).FirstOrDefault();
                            cr.Status = CheckupRecordStatus.CHO_KQXN;
                            tr.Status = TestRecordStatus.DA_THANH_TOAN;
                        }
                    }
                }
                else
                {
                    bill.Status = Bill.BillStatus.CHUA_TT;
                }
                bill.PaymentMethod = Bill.PaymentMethodEnum.VNPAY;
                bill.BankName = bank.bank_name;
                bill.BankLogoLink = "https://sandbox.vnpayment.vn/paymentv2" + bank.logo_link.Substring(1, bank.logo_link.Length - 1);
                bill.BankTranNo = vnp_BankTranNo;
                bill.CardType = vnp_CardType;
                bill.PayDate = DateTime.ParseExact(vnp_PayDate, "yyyyMMddHHmmss",
                                        CultureInfo.InvariantCulture);
                bill.VnPayTransactionNo = vnp_TransactionNo;
                bill.TransactionStatus = vnp_TransactionStatus;


                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    bill.Status = DataAccessLayer.Models.Bill.BillStatus.DA_TT_EBANKING;

                    //check đã thanh toán cho từng record tương ứng của bill detail
                    CheckupRecord cr = null;
                    foreach (var bd in bill.BillDetails)
                    {
                        //nếu là CR
                        if (bd.TestRecordId == null && bd.CheckupRecordId != null)
                        {
                            cr = _unitOfWork.CheckupRecordRepository.Get()
                                .Include(x => x.TestRecords)
                                .Include(x => x.Patient)
                                .Where(x => x.Id == bd.CheckupRecordId).FirstOrDefault();
                            //kiểm tra nếu chưa đặt lịch thì báo lỗi (tái khám)
                            if (cr.Status == CheckupRecordStatus.CHO_TAI_KHAM)
                            {
                                throw new Exception("No schedule for this checkup record");
                            }
                            //nếu là đã tái khám thì 
                            if ((bool)cr.IsReExam)
                            {
                                if (cr.TestRecords.Count > 0)
                                {
                                    foreach (var tr in cr.TestRecords)
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
                            cr = _unitOfWork.CheckupRecordRepository.Get()
                                 .Include(x => x.Patient)
                                .Where(x => x.Id == tr.CheckupRecordId).FirstOrDefault();
                            cr.Status = CheckupRecordStatus.CHO_KQXN;
                            tr.Status = TestRecordStatus.DA_THANH_TOAN;
                        }
                    }
                }
                else
                {
                    bill.Status = DataAccessLayer.Models.Bill.BillStatus.DANG_XU_LI_EBANKING;
                }
                await _unitOfWork.SaveChangesAsync();
                return "00";
            }
            else
            {
                //Invalid signature
                return "97";
            }
        }
    }
}
