using BusinessLayer.Interfaces.User;
using BusinessLayer.Interfaces.Notification;
using BusinessLayer.ResponseModels.Firebase;
using BusinessLayer.ResponseModels.ViewModels.User;
using DataAccessLayer.Models;
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
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System.Text.Json;

namespace BusinessLayer.Services.Notification
{
    public class NotificationService : BaseService, INotificationService
    {
        private readonly FirebaseApp _firebaseApp;
        private readonly IFCMTokenService _tokenService;
        private readonly ICheckupRecordService _checkupService;
        public NotificationService(FirebaseApp firebaseApp, IFCMTokenService tokenService, IUnitOfWork unitOfWork,
            ICheckupRecordService checkupService) : base(unitOfWork)
        {
            _checkupService = checkupService;
            _firebaseApp = firebaseApp;
            _tokenService = tokenService;
        }
        public async Task SendUpdateCheckupInfoReminder(long checkupRecordId,long accountId)
        {
            var cr = _checkupService.GetCheckupRecordFullData(checkupRecordId,accountId,true);
            GeneralFirebaseNotificationModel<PatientRecordFullDataResponseModel> data =
                new GeneralFirebaseNotificationModel<PatientRecordFullDataResponseModel>()
            {
                Data = cr,
                Type = GeneralFirebaseNotificationModel<PatientRecordFullDataResponseModel>.NotiType.CheckupStatusChangeReminder
            };
            await sendNotification(data, "","Trạng thái khám thay đổi","Trạng thái khám đã thay đổi.", accountId);
        }
        public async Task SendDepartmentChangeNoti(List<DepartmentChangeNoti> listDepartment, long accountId)
        {
            GeneralFirebaseNotificationModel<List<DepartmentChangeNoti>> data = 
                new GeneralFirebaseNotificationModel<List<DepartmentChangeNoti>>()
            {
                Data = listDepartment,
                Type = GeneralFirebaseNotificationModel<List<DepartmentChangeNoti>>
                        .NotiType.DepartmentChangeReminder
            };
            await sendNotification(data, "", "Thông báo chuyển khoa", "Thông báo chuyển khoa.", accountId);
        }
        private async Task sendNotification(object data, string topic, string title, string body, long accountId)
        {
            try
            {
                var dataForMesssage = new Dictionary<string, string>();
                dataForMesssage.Add("data", JsonSerializer.Serialize(data, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                }).ToString()); 
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
                    Data = dataForMesssage
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
