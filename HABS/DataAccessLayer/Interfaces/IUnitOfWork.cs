using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Account> AccountRepository { get; }
        IGenericRepository<Bill> BillRepository { get; }
        IGenericRepository<BillDetail> BillDetailRepository { get; }
        IGenericRepository<Cashier> CashierRepository { get; }
        IGenericRepository<CheckupRecord> CheckupRecordRepository { get; }
        IGenericRepository<Department> DepartmentRepository { get; }
        IGenericRepository<Doctor> DoctorRepository { get; }
        IGenericRepository<IcdDisease> IcdDiseaseRepository { get; }
        IGenericRepository<Medicine> MedicineRepository { get; }
        IGenericRepository<MedicineCategory> MedicineCategoryRepository { get; }
        IGenericRepository<Operation> OperationRepository { get; }
        IGenericRepository<Patient> PatientRepository { get; }
        IGenericRepository<Prescription> PrescriptionRepository { get; }
        IGenericRepository<PrescriptionDetail> PrescriptionDetailRepository { get; }
        IGenericRepository<Room> RoomRepository { get; }
        IGenericRepository<RoomType> RoomTypeRepository { get; }
        IGenericRepository<Schedule> ScheduleRepository { get; }
        IGenericRepository<TestRecord> TestRecordRepository { get; }
        HospitalAppointmentBookingContext Context();
        Task SaveChangesAsync();
    }
}
