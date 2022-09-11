using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DataAccessLayer.Models
{
    public partial class HospitalAppointmentBookingContext : DbContext
    {
        public HospitalAppointmentBookingContext()
        {
        }

        public HospitalAppointmentBookingContext(DbContextOptions<HospitalAppointmentBookingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<BillDetail> BillDetails { get; set; }
        public virtual DbSet<Cashier> Cashiers { get; set; }
        public virtual DbSet<CheckupRecord> CheckupRecords { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<FcmTokenMobile> FcmTokenMobiles { get; set; }
        public virtual DbSet<IcdDisease> IcdDiseases { get; set; }
        public virtual DbSet<Medicine> Medicines { get; set; }
        public virtual DbSet<MedicineCategory> MedicineCategories { get; set; }
        public virtual DbSet<Operation> Operations { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<Prescription> Prescriptions { get; set; }
        public virtual DbSet<PrescriptionDetail> PrescriptionDetails { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<RoomType> RoomTypes { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<TestRecord> TestRecords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=DESKTOP-V4RQDKU;Initial Catalog=HospitalAppointmentBooking;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasIndex(e => e.PhoneNumber, "UQ__Account__85FB4E386A59CCCD")
                    .IsUnique();

                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bill");

                entity.Property(e => e.PayDate).HasColumnType("datetime");

                entity.Property(e => e.TimeCreated).HasColumnType("datetime");

                entity.HasOne(d => d.Cashier)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.CashierId)
                    .HasConstraintName("FK__Bill__CashierId__47DBAE45");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.Bills)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__Bill__PatientId__619B8048");
            });

            modelBuilder.Entity<BillDetail>(entity =>
            {
                entity.ToTable("BillDetail");

                entity.HasOne(d => d.Bill)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.BillId)
                    .HasConstraintName("FK__BillDetai__BillI__4BAC3F29");

                entity.HasOne(d => d.CheckupRecord)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.CheckupRecordId)
                    .HasConstraintName("FK__BillDetai__Check__48CFD27E");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.OperationId)
                    .HasConstraintName("FK__BillDetai__Opera__49C3F6B7");

                entity.HasOne(d => d.TestRecord)
                    .WithMany(p => p.BillDetails)
                    .HasForeignKey(d => d.TestRecordId)
                    .HasConstraintName("FK__BillDetai__TestR__4AB81AF0");
            });

            modelBuilder.Entity<Cashier>(entity =>
            {
                entity.ToTable("Cashier");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<CheckupRecord>(entity =>
            {
                entity.ToTable("CheckupRecord");

                entity.HasIndex(e => e.EstimatedDate, "EstimatedDateCheckupRecordIndex");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.EstimatedDate).HasColumnType("datetime");

                entity.Property(e => e.EstimatedStartTime).HasColumnType("datetime");

                entity.Property(e => e.IcdDiseaseCode).HasMaxLength(20);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.CheckupRecords)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK__CheckupRe__Depar__4CA06362");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.CheckupRecords)
                    .HasForeignKey(d => d.DoctorId)
                    .HasConstraintName("FK__CheckupRe__Docto__4D94879B");

                entity.HasOne(d => d.IcdDisease)
                    .WithMany(p => p.CheckupRecords)
                    .HasForeignKey(d => d.IcdDiseaseId)
                    .HasConstraintName("FK__CheckupRe__IcdDi__4F7CD00D");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.CheckupRecords)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__CheckupRe__Patie__5070F446");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.CheckupRecords)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__CheckupRe__RoomI__4E88ABD4");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.ToTable("Doctor");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Username).IsRequired();
            });

            modelBuilder.Entity<FcmTokenMobile>(entity =>
            {
                entity.ToTable("FcmTokenMobile");

                entity.Property(e => e.TokenId).IsRequired();

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.FcmTokenMobiles)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FcmTokenM__Accou__5165187F");
            });

            modelBuilder.Entity<IcdDisease>(entity =>
            {
                entity.ToTable("IcdDisease");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Medicine>(entity =>
            {
                entity.ToTable("Medicine");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Unit).IsRequired();

                entity.HasOne(d => d.MedicineCategory)
                    .WithMany(p => p.Medicines)
                    .HasForeignKey(d => d.MedicineCategoryId)
                    .HasConstraintName("FK__Medicine__Medici__52593CB8");
            });

            modelBuilder.Entity<MedicineCategory>(entity =>
            {
                entity.ToTable("MedicineCategory");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Operation>(entity =>
            {
                entity.ToTable("Operation");

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Operations)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK__Operation__Depar__534D60F1");

                entity.HasOne(d => d.RoomType)
                    .WithMany(p => p.Operations)
                    .HasForeignKey(d => d.RoomTypeId)
                    .HasConstraintName("FK__Operation__RoomT__5441852A");
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patient");

                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.Bhyt).HasColumnName("BHYT");

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.Name).IsRequired();

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Patients)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Patient__Account__5535A963");
            });

            modelBuilder.Entity<Prescription>(entity =>
            {
                entity.ToTable("Prescription");

                entity.Property(e => e.TimeCreated).HasColumnType("datetime");

                entity.HasOne(d => d.CheckupRecord)
                    .WithMany(p => p.Prescriptions)
                    .HasForeignKey(d => d.CheckupRecordId)
                    .HasConstraintName("FK__Prescript__Check__5629CD9C");
            });

            modelBuilder.Entity<PrescriptionDetail>(entity =>
            {
                entity.ToTable("PrescriptionDetail");

                entity.HasOne(d => d.Medicine)
                    .WithMany(p => p.PrescriptionDetails)
                    .HasForeignKey(d => d.MedicineId)
                    .HasConstraintName("FK__Prescript__Medic__571DF1D5");

                entity.HasOne(d => d.Prescription)
                    .WithMany(p => p.PrescriptionDetails)
                    .HasForeignKey(d => d.PrescriptionId)
                    .HasConstraintName("FK__Prescript__Presc__5812160E");
            });

            modelBuilder.Entity<Room>(entity =>
            {
                entity.ToTable("Room");

                entity.Property(e => e.Floor).IsRequired();

                entity.Property(e => e.RoomNumber).IsRequired();

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK__Room__Department__59063A47");

                entity.HasOne(d => d.RoomType)
                    .WithMany(p => p.Rooms)
                    .HasForeignKey(d => d.RoomTypeId)
                    .HasConstraintName("FK__Room__RoomTypeId__59FA5E80");
            });

            modelBuilder.Entity<RoomType>(entity =>
            {
                entity.ToTable("RoomType");

                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.DoctorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Schedule__Doctor__5AEE82B9");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.Schedules)
                    .HasForeignKey(d => d.RoomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Schedule__RoomId__5BE2A6F2");
            });

            modelBuilder.Entity<TestRecord>(entity =>
            {
                entity.ToTable("TestRecord");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.EstimatedDate).HasColumnType("datetime");

                entity.HasOne(d => d.CheckupRecord)
                    .WithMany(p => p.TestRecords)
                    .HasForeignKey(d => d.CheckupRecordId)
                    .HasConstraintName("FK__TestRecor__Check__5CD6CB2B");

                entity.HasOne(d => d.Doctor)
                    .WithMany(p => p.TestRecords)
                    .HasForeignKey(d => d.DoctorId)
                    .HasConstraintName("FK__TestRecor__Docto__60A75C0F");

                entity.HasOne(d => d.Operation)
                    .WithMany(p => p.TestRecords)
                    .HasForeignKey(d => d.OperationId)
                    .HasConstraintName("FK__TestRecor__Opera__5DCAEF64");

                entity.HasOne(d => d.Patient)
                    .WithMany(p => p.TestRecords)
                    .HasForeignKey(d => d.PatientId)
                    .HasConstraintName("FK__TestRecor__Patie__5EBF139D");

                entity.HasOne(d => d.Room)
                    .WithMany(p => p.TestRecords)
                    .HasForeignKey(d => d.RoomId)
                    .HasConstraintName("FK__TestRecor__RoomI__5FB337D6");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
