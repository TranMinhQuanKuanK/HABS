using DataAccessLayer.Models;
using DataAcessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcessLayer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HospitalAppointmentBookingContext _dbContext;

        public IGenericRepository<Account> AccountRepository { get; }
        public IGenericRepository<Config> ConfigRepository { get; }
        public IGenericRepository<Bill> BillRepository { get; }
        public IGenericRepository<BillDetail> BillDetailRepository { get; }
        public IGenericRepository<Cashier> CashierRepository { get; }
        public IGenericRepository<CheckupRecord> CheckupRecordRepository { get; }
        public IGenericRepository<Department> DepartmentRepository { get; }
        public IGenericRepository<Doctor> DoctorRepository { get; }
        public IGenericRepository<IcdDisease> IcdDiseaseRepository { get; }
        public IGenericRepository<Medicine> MedicineRepository { get; }
        public IGenericRepository<MedicineCategory> MedicineCategoryRepository { get; }
        public IGenericRepository<Operation> OperationRepository { get; }
        public IGenericRepository<Patient> PatientRepository { get; }
        public IGenericRepository<Prescription> PrescriptionRepository { get; }
        public IGenericRepository<PrescriptionDetail> PrescriptionDetailRepository { get; }
        public IGenericRepository<Room> RoomRepository { get; }
        public IGenericRepository<RoomType> RoomTypeRepository { get; }
        public IGenericRepository<Schedule> ScheduleRepository { get; }
        public IGenericRepository<TestRecord> TestRecordRepository { get; }
        public IGenericRepository<FcmTokenMobile> FcmTokenMobileRepository { get; }
        public UnitOfWork(HospitalAppointmentBookingContext dbContext,
            IGenericRepository<Account> accountRepository,
            IGenericRepository<Bill> billRepository,
            IGenericRepository<Config> configRepository,
            IGenericRepository<BillDetail> billDetailRepository,
            IGenericRepository<Cashier> cashierRepository,
            IGenericRepository<CheckupRecord> checkupRecordRepository,
            IGenericRepository<Department> departmentRepository,
            IGenericRepository<Doctor> doctorRepository,
            IGenericRepository<IcdDisease> icdDiseaseRepository,
            IGenericRepository<Medicine> medicineRepository,
            IGenericRepository<MedicineCategory> medicineCategoryRepository,
            IGenericRepository<Operation> operationRepository,
            IGenericRepository<Patient> patientRepository,
            IGenericRepository<Prescription> prescriptionRepository,
            IGenericRepository<PrescriptionDetail> prescriptionDetailRepository,
            IGenericRepository<Room> roomRepository,
            IGenericRepository<RoomType> roomTypeRepository,
            IGenericRepository<Schedule> scheduleRepository,
            IGenericRepository<TestRecord> testRecordRepository,
             IGenericRepository<FcmTokenMobile> fcmTokenMobileRepository
            )
        {
            _dbContext = dbContext;

            AccountRepository = accountRepository;
            BillRepository = billRepository;
            ConfigRepository = configRepository;
            BillDetailRepository = billDetailRepository;
            CashierRepository = cashierRepository;
            CheckupRecordRepository = checkupRecordRepository;
            DepartmentRepository = departmentRepository;
            DoctorRepository = doctorRepository;
            IcdDiseaseRepository = icdDiseaseRepository;
            MedicineRepository = medicineRepository;
            OperationRepository = operationRepository;
            PatientRepository = patientRepository;
            PrescriptionRepository = prescriptionRepository;
            PrescriptionDetailRepository = prescriptionDetailRepository;
            RoomRepository = roomRepository;
            ScheduleRepository = scheduleRepository;
            TestRecordRepository = testRecordRepository;
            FcmTokenMobileRepository = fcmTokenMobileRepository;
        }

        public HospitalAppointmentBookingContext Context()
        {
            return _dbContext;
        }

        //public DatabaseFacade Database()
        //{
        //    return _dbContext.Database;
        //}

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
