using AutoMapper;
using BusinessLayer.Interfaces.Notification;
using BusinessLayer.ResponseModels.Firebase;
using CorePush.Google;
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

        public NotificationService(FirebaseApp firebaseApp, IFCMTokenService tokenService, IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _firebaseApp = firebaseApp;
            _tokenService = tokenService;
        }
        public async Task SendNotificationOutOfStockProduct(int productId, int brandId, string productName)
        {
            var notiModel = new OutOfStockFirebaseNotificationModel()
            {
                BrandId = brandId,
                ProductId = productId,
                ProductName = productName,
                ClickAction = "FLUTTER_NOTIFICATION_CLICK",
                Screen = "/outOfStock",
            };
            string title = "SẮP HẾT HÀNG";
            string body = $"Sản phẩm \"{productName}\" sắp hết hàng. Hãy nhập hàng ngay!";
            await SendNotification(notiModel, "OutOfStock", title, body,brandId);
        }
        public async Task SendNotificationStoreApproved(int storeId, int brandId, string storeName)
        {
            var notiModel = new StoreApprovedRejectedNotificationModel()
            {
                BrandId = brandId,
                StoreId = storeId,
                StoreName = storeName,
                ClickAction = "FLUTTER_NOTIFICATION_CLICK",
                Screen = "/stores",
            };
            string title = "YÊU CẦU ĐƯỢC PHÊ DUYỆT";
            string body = $"Yêu cầu mở tiệm \"{storeName}\" của bạn đã được admin phê duyệt. Hãy bắt đầu quản lý ngay nào.";
            await SendNotification(notiModel, "StoreApproved", title, body, brandId);
        }
        public async Task SendNotificationStoreRejected(int storeId, int brandId, string storeName)
        {
            var notiModel = new StoreApprovedRejectedNotificationModel()
            {
                BrandId = brandId,
                StoreId = storeId,
                StoreName = storeName,
                ClickAction = "FLUTTER_NOTIFICATION_CLICK",
                Screen = "/stores",
            };
            string title = "YÊU CẦU BỊ TỪ CHỐI";
            string body = $"Yêu cầu mở tiệm \"{storeName}\" của bạn đã bị từ chối. Liên hệ chúng tôi để biết thêm chi tiết.";
            await SendNotification(notiModel, "StoreRejected", title, body, brandId);
        }
        private async Task SendNotification(object data, string topic, string title, string body, int brandId)
        {
            try
            {
                var userIDList = await _unitOfWork.UserBrandRepository.Get().Where(x => x.BrandId == brandId).Select(x => x.UserId).ToListAsync();
                var dataForMesssage = new Dictionary<string, string>();

                foreach (PropertyInfo prop in data.GetType().GetProperties())
                {
                    dataForMesssage.Add(prop.Name, prop.GetValue(data).ToString());
                }
                List<string> tokenList = new List<string>();
                userIDList.ForEach(x => tokenList.AddRange(_tokenService.GetTokenList(x)));
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
