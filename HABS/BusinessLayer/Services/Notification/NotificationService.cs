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
            await sendNotification(data, "",$"Trạng thái đổi thành: {cr.Status.ToString()}", $"Trạng thái đổi thành: {cr.Status}, record {cr.Id}, BN: {cr.PatientName}, BS: {cr.DoctorName}", accountId);
        }
        public async Task SendDepartmentChangeNoti(List<DepartmentChangeInfoNoti> listDepartment, long previousRecordId, long accountId)
        {

            GeneralFirebaseNotificationModel<DepartmentChangeNoti> data = 
                new GeneralFirebaseNotificationModel<DepartmentChangeNoti>()
            {
                Data = new DepartmentChangeNoti()
                {
                    PreviousRecordId = previousRecordId,
                    Departments = listDepartment
                },
                Type = GeneralFirebaseNotificationModel<DepartmentChangeNoti>
                        .NotiType.DepartmentChangeReminder
            };
            await sendNotification(data, "", $"Thông báo chuyển khoa cho record {previousRecordId}, số lượng khoa: {listDepartment.Count}",
                $"Thông báo chuyển khoa {previousRecordId}, số lượng khoa: {listDepartment.Count}.", accountId);
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Send notification: "+e.Message);
            }
        }

    }
}
