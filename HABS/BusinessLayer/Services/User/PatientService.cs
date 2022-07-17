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
using System.Threading.Tasks;
using BusinessLayer.Interfaces.User;
using BusinessLayer.ResponseModels.ViewModels.User;
using BusinessLayer.RequestModels.CreateModels.User;
using DataAccessLayer.Models;
using static DataAccessLayer.Models.Patient;

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
                .Include(x=>x.Account)
                .Where(x => x.AccountId == accountId)
                .Where(x => x.Status == PatientStatus.HOAT_DONG)
                .Select(x => new PatientResponseModel()
                {
                    Id = x.Id,
                    Address = x.Address,
                    Bhyt = x.Bhyt,
                    DateOfBirth = x.DateOfBirth,
                    Gender = (int)x.Gender,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber,
                    AccountPhoneNo = x.Account.PhoneNumber,
                    AccountId = x.AccountId
                })
                .ToList();
            return data;
        }
        public PatientResponseModel GetPatientById(long patientId)
        {
            var data = _unitOfWork.PatientRepository.Get()
                .Where(x => x.Id == patientId)
                .Where(x => x.Status == PatientStatus.HOAT_DONG)
                .Select(x => new PatientResponseModel()
                {
                    Id = x.Id,
                    Address = x.Address,
                    Bhyt = x.Bhyt,
                    DateOfBirth = x.DateOfBirth,
                    Gender = (int)x.Gender,
                    Name = x.Name,
                    PhoneNumber = x.PhoneNumber,
                    AccountId = x.AccountId,
                    AccountPhoneNo = x.Account.PhoneNumber,
                }).FirstOrDefault();
            return data;
        }
        public async Task RegisterANewPatient(long accountId, PatientCreateEditModel model)
        {
            //check patient
            var prePatient = _unitOfWork.PatientRepository.Get()
                .Where(x => x.Bhyt == model.Bhyt)
                .Where(x => x.Status == PatientStatus.HOAT_DONG)
                .FirstOrDefault();
            if (prePatient != null)
            {
                throw new Exception("Health insurance code is used.");
            }
            //không cần check account
            //tạo patient
            if (model.Gender > (int)GenderEnum.NOT_SPECIFIED)
            {
                model.Gender = (int)GenderEnum.NOT_SPECIFIED;
            }
            var patient = new Patient()
            {
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                AccountId = accountId,
                Address = model.Address,
                Gender = (GenderEnum)model.Gender,
                DateOfBirth = (DateTime)model.DateOfBirth,
                Bhyt = model.Bhyt,
                Status = PatientStatus.HOAT_DONG
            };
            await _unitOfWork.PatientRepository.Add(patient);
            await _unitOfWork.SaveChangesAsync();

        }
        public async Task EditPatient(long userId, long patientId, PatientCreateEditModel edit)
        {
            //check user exist
            var user = _unitOfWork.PatientRepository
                .Get()
                .Where(x => x.Id == patientId)
                .Where(x => x.AccountId == userId)
                .Where(x => x.Status == PatientStatus.HOAT_DONG)
                .FirstOrDefault();
            if (user == null)
            {
                throw new Exception("User doesn't exist");
            }
            //check phone no exist
            var prePatient = _unitOfWork.PatientRepository.Get()
               .Where(x => x.Bhyt == edit.Bhyt || x.PhoneNumber == edit.PhoneNumber)
               .FirstOrDefault();
            if (prePatient != null && prePatient.PhoneNumber == edit.PhoneNumber)
            {
                throw new Exception("Phone number is used.");
            }
            //check bảo hiểm exist
            if (prePatient != null && prePatient.Bhyt == edit.Bhyt)
            {
                throw new Exception("Health Insurance code is used.");
            }
            //edit
            if (edit.Bhyt != null)
            {
                user.Bhyt = edit.Bhyt;
            }
            if (edit.DateOfBirth != null)
            {
                user.DateOfBirth = (DateTime)edit.DateOfBirth;
            }
            if (edit.Gender != null)
            {
                user.Gender = (GenderEnum)edit.Gender;
            }
            if (edit.Name != null)
            {
                user.Name = edit.Name;
            }
            if (edit.PhoneNumber != null)
            {
                user.PhoneNumber = edit.PhoneNumber;
            }
            if (edit.Address != null)
            {
                user.Address = edit.Address;
            }
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeletePatient(long userId, long patientId)
        {
            //check user exist
            var user = _unitOfWork.PatientRepository
                .Get()
                .Where(x => x.Id == patientId)
                .Where(x => x.AccountId == userId)
                .Where(x=>x.Status==PatientStatus.HOAT_DONG)
                .FirstOrDefault();
            if (user == null)
            {
                throw new Exception("User doesn't exist");
            }
            user.Status = PatientStatus.DA_XOA;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
