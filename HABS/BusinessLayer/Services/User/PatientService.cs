using BusinessLayer.RequestModels;
using BusinessLayer.RequestModels.CreateModels;
using BusinessLayer.RequestModels.SearchModels;
using BusinessLayer.ResponseModels.ViewModels;
using BusinessLayer.Services;
using DataAcessLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using BusinessLayer.Services.Redis;
using Newtonsoft.Json;
using BusinessLayer.Interfaces.User;
using BusinessLayer.ResponseModels.ViewModels.User;
using BusinessLayer.RequestModels.CreateModels.User;

namespace BusinessLayer.Services.User
{
    public class PatientService : BaseService, IPatientService
    {
        public PatientService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        public List<PatientResponseModel> GetPatients(long accountId)
        {
            List<PatientResponseModel> data = new List<PatientResponseModel>();
            data = _unitOfWork.PatientRepository.Get()
                .Where(x => x.AccountId == accountId)
                .Where(x => x.Status == DataAccessLayer.Models.Patient.PatientStatus.HOAT_DONG)
                .Select(x => new PatientResponseModel()
                {
                    Id = x.Id,
                    Address = x.Address,
                    Bhyt = x.Bhyt,
                    DateOfBirth = x.DateOfBirth,
                    Gender = x.Gender,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber
                })
                .ToList();
            return data;
        }
        public PatientResponseModel GetPatientById(long patientId)
        {
            var data = _unitOfWork.PatientRepository.Get()
                .Where(x => x.Id == patientId)
                .Where(x=>x.Status==DataAccessLayer.Models.Patient.PatientStatus.HOAT_DONG)
                .Select(x => new PatientResponseModel()
                {
                    Id = x.Id,
                    Address = x.Address,
                    Bhyt = x.Bhyt,
                    DateOfBirth = x.DateOfBirth,
                    Gender = x.Gender,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber
                }).FirstOrDefault();
            return data;
        }
        public async Task RegisterANewPatient(long accountId, PatientCreateEditModel model)
        {
            //check user phone, email
            var preUser = _unitOfWork.AccountRepository.Get()
                .Where(x => x.PhoneNumber == model.PhoneNumber || x.Email == model.Email)
                .Where(x => x.Status == Account.UserStatus.BINH_THUONG)
                .FirstOrDefault();
            if (preUser != null)
            {
                throw new Exception("Email/phone number existed");
            }
            var user = new Account()
            {
                Name = model.Name,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
            };
            await _unitOfWork.AccountRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();

        }
        public async Task EditUser(long userId, UserCreateEditModel edit)
        {
            //check phone
            var preUser = _unitOfWork.AccountRepository.Get().Where(x => x.PhoneNumber == edit.PhoneNumber).FirstOrDefault();
            if (preUser != null)
            {
                throw new Exception("Phone number is already used!");
            }

            var user = _unitOfWork.AccountRepository.Get().Where(x => x.Id == userId)
                .Where(x => x.Status == Account.UserStatus.BINH_THUONG)
                .FirstOrDefault();
            if (user == null)
            {
                throw new Exception("User doesn't exist");
            }
            if (!string.IsNullOrEmpty(user.Name))
            {
                user.Name = edit.Name;
            }
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                user.Name = edit.Name;
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
