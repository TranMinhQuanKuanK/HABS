﻿using BusinessLayer.ResponseModels.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.User
{
    public interface ICheckupRecordService
    {
        List<PatientRecordMetadataResponseModel> GetCheckupRecordMetadata(long? patientId, DateTime? fromTime, 
            DateTime? toTime, long? departmentId, long accountId);
        PatientRecordFullDataResponseModel GetCheckupRecordFullData(long recordId, long accountId, bool includeBills);
        Task<AppointmenAfterBookingResponseModel> CreatNewAppointment(long patientId, DateTime date, long doctorId,
            int? numericalOrder, string clinicalSymptom, long accountId);
        Task<AppointmenAfterBookingResponseModel> CreatReExamAppointment(long patientId, long previousCrId, DateTime date,
           long doctorId, int? numericalOrder, string clinicalSymptom, long accountId);
    }
}
