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
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http;
using Firebase.Auth;
using System.Threading;
using Firebase.Storage;

namespace BusinessLayer.Services.Doctor
{
    public class FileService : BaseService, IFileService
    {

        public FileService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        private string ApiKey = "AIzaSyD2PQiIO4CVsif4kgTRDJH6h0YrYlS-ceY";
        private string Bucket = "hospitalmanagement-42da9.appspot.com";
        private string AuthEmail = "hostpitaldoan@gmail.com";
        private string AuthPassword = "hospitaldoan";
        private string[] permittedExtensions = { "pdf" };
        private long sizeLimit = 7 * 1048576;

        private string GetFileName(long patientId, long trId)
        {
            return $"test-result/patient-{patientId}/result-{trId}-{((DateTimeOffset)DateTime.Now.AddHours(7)).ToUnixTimeMilliseconds()}.pdf";
        }
        private bool IsValidFileExtension(string fileName, Stream data, string[] permittedExtensions)
        {
            if (string.IsNullOrEmpty(fileName) || data == null || data.Length == 0)
            {
                return false;
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }
            data.Position = 0;

            return true;
        }
        public async Task<string> UploadToFirebase(IFormFile file, long patientId, long trId)
        {
            if (file.Length == 0)
            {
                throw new Exception("Empty file detected!");
            }

            if (file.Length > sizeLimit)
            {
                var megabyteSizeLimit = sizeLimit / 1048576;
                throw new Exception($"File exceeds ${megabyteSizeLimit:N1} MB");
            }
            //kiểm tra đuôi
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);

                    if (memoryStream.Length == 0)
                    {
                        throw new Exception("Empty file detected!");
                    }

                    if (!IsValidFileExtension(
                        file.FileName, memoryStream, permittedExtensions))
                    {
                        throw new Exception("Invalid file type. PDF is required");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            //upload fireabse
            string fileName = GetFileName(patientId, trId);
            if (file.Length > 0)
            {
                var memoStream = new MemoryStream();
                await file.CopyToAsync(memoStream);
                memoStream.Position = 0;

                var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
                var authLink = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

                var cancellation = new CancellationTokenSource();
                var reference = new FirebaseStorage(
                    Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(authLink.FirebaseToken),
                        ThrowOnCancel = true 
                    })
                    .Child(fileName);

                var task = reference.PutAsync(memoStream);
                await task;
                return await reference.GetDownloadUrlAsync();
            }
            return string.Empty;
        }
    }
}
