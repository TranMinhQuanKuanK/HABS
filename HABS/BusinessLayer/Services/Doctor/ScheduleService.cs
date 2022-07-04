using DataAcessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using BusinessLayer.Services.Redis;
using Newtonsoft.Json;
using BusinessLayer.Interfaces.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using static DataAccessLayer.Models.CheckupRecord;
using static DataAccessLayer.Models.TestRecord;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Services.Doctor
{
    public class ScheduleService : BaseService, IScheduleService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly RedisService _redisService;
        public ScheduleService(IUnitOfWork unitOfWork, IDistributedCache distributedCache) : base(unitOfWork)
        {
            _distributedCache = distributedCache;
            _redisService = new RedisService(_distributedCache);
        }
        public List<CheckupAppointmentViewModel> UpdateRedis_CheckupQueue(long RoomId)
        {
            string redisKey = $"checkup-queue-for-room-{RoomId}";

            var queue = _unitOfWork.CheckupRecordRepository.Get()
            .Where(x => x.RoomId == RoomId)
            .Where(x => x.Status == CheckupRecordStatus.DANG_KHAM
            || x.Status == CheckupRecordStatus.DA_THANH_TOAN
            || x.Status == CheckupRecordStatus.DA_CO_KQXN
            )
            .Where(x => ((DateTime)x.EstimatedDate).Date == DateTime.Now.AddHours(7).Date)
            .OrderBy(x => ((DateTime)x.EstimatedStartTime).TimeOfDay)
            .Select(x => new CheckupAppointmentViewModel()
            {
                Id = x.Id,
                EstimatedStartTime = x.EstimatedStartTime,
                PatientId = x.PatientId,
                PatientName = x.PatientName,
                NumericalOrder = x.NumericalOrder,
                Status = (int)x.Status,
                IsReExam = (bool)x.IsReExam

            }).ToList();
            var checkingUpPatient = queue.FirstOrDefault(x => x.Status == (int)CheckupRecordStatus.DANG_KHAM);
            if (checkingUpPatient != null)
            {
                queue.Remove(checkingUpPatient);
                queue.Insert(0, checkingUpPatient);
            }
            //lấy những đứa vừa có kết quả xét nghiệm bỏ lên đầu
            var gotResultPatients = queue.Where(x => x.Status == (int)CheckupRecordStatus.DA_CO_KQXN).OrderBy(x => x.NumericalOrder);
            if (gotResultPatients.Count() > 0)
            {
                foreach (var item in gotResultPatients)
                {
                    queue.Remove(item);
                }
                queue.InsertRange(1, gotResultPatients);
            }
            _redisService.SetValueToKey(redisKey, JsonConvert.SerializeObject(queue));
            return queue;
        }
        public List<CheckupAppointmentViewModel> GetCheckupQueue(long RoomId)
        {
            List<CheckupAppointmentViewModel> queue = null;

            string redisKey = $"checkup-queue-for-room-{RoomId}";
            string dataFromRedis = _redisService.GetValueFromKey(redisKey);
            if (!String.IsNullOrEmpty(dataFromRedis))
            {
                queue = JsonConvert.DeserializeObject<List<CheckupAppointmentViewModel>>(dataFromRedis);
            }
            else
            {
                queue = UpdateRedis_CheckupQueue(RoomId);
            }

            return queue;
        }
        public List<TestAppointmentViewModel> UpdateRedis_TestQueue(long RoomId, bool isWaitingForResult)
        {
            string redisKey = $"test-queue-for-room-{RoomId}-{isWaitingForResult}";
            var queue = _unitOfWork.TestRecordRepository.Get()
                    .Include(x=>x.Patient)
                   .Where(x => x.RoomId == RoomId)
                   .Where(x => (isWaitingForResult) ?
                    x.Status == TestRecordStatus.CHO_KET_QUA
                    :
                   x.Status == TestRecordStatus.DA_THANH_TOAN
                   || x.Status == TestRecordStatus.DANG_TIEN_HANH
                   )
                   .Where(x => ((DateTime)x.Date).Date == DateTime.Now.AddHours(7).Date)
                   .OrderBy(x => x.NumericalOrder)
                   .Select(x => new TestAppointmentViewModel()
                   {
                       Id = x.Id,
                       PatientId = x.PatientId,
                       PatientName = x.PatientName,
                       NumericalOrder = x.NumericalOrder,
                       Status = (int)x.Status,
                       OperationId = (long)x.OperationId,
                       OperationName = x.OperationName,
                       Date = x.Date,
                       Patient = new PatientViewModel()
                       {
                           Address =  x.Patient.Address,
                           Bhyt = x.Patient.Bhyt,
                           DateOfBirth = x.Patient.DateOfBirth,
                           Gender = (int)x.Patient.Gender,
                           Id = x.Patient.Id,
                           Name = x.Patient.Name,
                           PhoneNumber = x.Patient.PhoneNumber
                       }
                   }).ToList();
            var testingPatient = queue.SingleOrDefault(x => x.Status == (int)TestRecordStatus.DANG_TIEN_HANH);
            if (testingPatient != null)
            {
                queue.Remove(testingPatient);
                queue.Insert(0, testingPatient);
            }
            _redisService.SetValueToKey(redisKey, JsonConvert.SerializeObject(queue));
            return queue;
        }
        public List<TestAppointmentViewModel> GetTestQueue(long RoomId, bool isWaitingForResult)
        {
            List<TestAppointmentViewModel> queue = null;

            string redisKey = $"test-queue-for-room-{RoomId}-{isWaitingForResult}";
            string dataFromRedis = _redisService.GetValueFromKey(redisKey);
            if (!String.IsNullOrEmpty(dataFromRedis))
            {
                queue = JsonConvert.DeserializeObject<List<TestAppointmentViewModel>>(dataFromRedis);
            }
            else
            {
                queue = UpdateRedis_TestQueue(RoomId, isWaitingForResult);
            }

            return queue;
        }
        public TestAppointmentViewModel GetItemInTestQueue(long testId)
        {
            var item = _unitOfWork.TestRecordRepository.Get()
                     .Include(x => x.Patient)
                    .Where(x => x.Id == testId)
                    .Select(x => new TestAppointmentViewModel()
                    {
                        Id = x.Id,
                        PatientId = x.PatientId,
                        PatientName = x.PatientName,
                        NumericalOrder = x.NumericalOrder,
                        Status = (int)x.Status,
                        OperationId = (long)x.OperationId,
                        OperationName = x.OperationName,
                        Date = x.Date,
                        Patient = new PatientViewModel()
                        {
                            Address = x.Patient.Address,
                            Bhyt = x.Patient.Bhyt,
                            DateOfBirth = x.Patient.DateOfBirth,
                            Gender = (int)x.Patient.Gender,
                            Id = x.Patient.Id,
                            Name = x.Patient.Name,
                            PhoneNumber = x.Patient.PhoneNumber
                        }
                    }).FirstOrDefault();
            return item;
        }
    }
}
