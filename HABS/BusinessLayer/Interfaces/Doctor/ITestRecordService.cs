﻿using BusinessLayer.RequestModels.CreateModels.Doctor;
using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Doctor
{
    public interface ITestRecordService
    {
        Task UpdateTestRecordResult(TestRecordEditModel model, long doctorId);
        Task ConfirmTest(long doctorId, long tId);
    }
}
