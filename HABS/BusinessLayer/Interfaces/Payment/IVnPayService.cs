using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Payment
{
    public interface IVnPayService
    {
        string CreateVnPayRequest(long billId, long accountId);
        Task<string> IpnReceiver(string vnp_TmnCode, string vnp_SecureHash,
            string vnp_txnRef, string vnp_TransactionStatus, string vnp_ResponseCode,
            string vnp_TransactionNo, string vnp_BankCode, string vnp_Amount, string vnp_PayDate);

    }

}
