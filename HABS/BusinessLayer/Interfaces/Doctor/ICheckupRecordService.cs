using BusinessLayer.ResponseModels.ViewModels.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Doctor
{
    public interface ICheckupRecordService
    {
        List<PatientRecordMetadataResponseModel> GetCheckupRecordMetadata(long? patientId, DateTime? fromTime, DateTime? toTime, long? departmentId);
        PatientRecordFullDataResponseModel GetCheckupRecordFullData(long patientId);
    }
}
