using BusinessLayer.RequestModels.CreateModels.Doctor;
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
        List<PatientRecordMetadataViewModel> GetCheckupRecordMetadata(long? patientId, DateTime? fromTime, DateTime? toTime, long? departmentId);
        PatientRecordFullDataViewModel GetCheckupRecordFullData(long patientId);
        Task ConfirmCheckup(long crId, long? doctorId);
        Task CreatePrescription(long recordId, PrescriptionCreateModel model);
        Task<List<RedirectViewModel>> RedirectPatient(RedirectCreateModel model, long recordId);
        Task<List<IncomingTestResponseModel>> RequestExamination(long recordId, TestRequestCreateModel testReqModel);
        Task EditCheckupRecord(CheckupRecordEditModel model);
        Task CreateReExamCheckupRecord(long previousCrId, long doctorId, ReExamCreateModel model);
    }
}
