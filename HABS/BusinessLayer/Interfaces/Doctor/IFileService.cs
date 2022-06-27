using BusinessLayer.ResponseModels.ViewModels.Doctor;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces.Doctor
{
    public interface IFileService
    {
        Task<string> UploadToFirebase(IFormFile file, long patientId, long trId);
    }
}
