using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Payment
{
    public interface IVnPayService
    {
        Task<string> CreateVnPayRequest(long billId, long accountId, string remoteIpAddress);
        Task<string> IpnReceiver(string vnp_TmnCode, string vnp_SecureHash,
            string vnp_txnRef, string vnp_TransactionStatus, string vnp_ResponseCode,
            string vnp_TransactionNo, string vnp_BankCode, 
            string vnp_Amount, string vnp_PayDate, string vnp_BankTranNo, string vnp_CardType, NameValueCollection requestNameValue);

    }

}
