﻿using BusinessLayer.ResponseModels.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.User
{
    public interface IPatientService
    {
        List<PatientResponseModel> GetPatients(long accountId);
        PatientResponseModel GetPatientById(long patientId);
    }
}
