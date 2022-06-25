using BusinessLayer.Interfaces.Notification;
using BusinessLayer.ResponseModels.Firebase;
using DataAcessLayer.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Notification
{
    public class NotificationService : BaseService, INotificationService
    {
        private readonly FirebaseApp _firebaseApp;
        private readonly IFCMTokenService _tokenService;

        public NotificationService(FirebaseApp firebaseApp, IFCMTokenService tokenService, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _firebaseApp = firebaseApp;
            _tokenService = tokenService;
        }
        //public async Task SendNotificationStoreRejected(int storeId, int brandId, string storeName)
        //{
        //    var notiModel = new StoreApprovedRejectedNotificationModel()
        //    {
        //        BrandId = brandId,
        //        StoreId = storeId,
        //        StoreName = storeName,
        //        ClickAction = "FLUTTER_NOTIFICATION_CLICK",
        //        Screen = "/stores",
        //    };
        //    string title = "YÊU CẦU BỊ TỪ CHỐI";
        //    string body = $"Yêu cầu mở tiệm \"{storeName}\" của bạn đã bị từ chối. Liên hệ chúng tôi để biết thêm chi tiết.";
        //    await SendNotification(notiModel, "StoreRejected", title, body, brandId);
        //}
        public async Task SendUpdateCheckupInfoReminder(long patientId, long checkupRecordId)
        {

        }
        private async Task sendNotification(object data, string topic, string title, string body, long accountId)
        {
            try
            {
                var dataForMesssage = new Dictionary<string, string>();

                foreach (PropertyInfo prop in data.GetType().GetProperties())
                {
                    dataForMesssage.Add(prop.Name, prop.GetValue(data).ToString());
                }

                List<string> tokenList = _tokenService.GetTokenList(accountId);
                List<Message> messageList = new List<Message>();
                tokenList.ForEach(_token => messageList.Add(new Message()
                {
                    Token = _token,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Body = body,
                        Title = title
                    },
                    Data = dataForMesssage,
                }));
                var response = await FirebaseMessaging.DefaultInstance.SendAllAsync(messageList);
                // Response is a message ID string.
                Console.WriteLine("Successfully sent message: " + response.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Debug exception: " + e.Message);
            }
        }

    }
}
