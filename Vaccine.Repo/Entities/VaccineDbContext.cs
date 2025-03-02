using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Vaccine.Repo.Entities;

namespace Vaccine.Repo.Entities;
public partial class VaccineDbContext : DbContext
{
    public VaccineDbContext()
    {
    }

    public VaccineDbContext(DbContextOptions<VaccineDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Child> Children { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<HealthRecord> HealthRecords { get; set; }

    public virtual DbSet<Holiday> Holidays { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<Vaccine> Vaccines { get; set; }

    public virtual DbSet<VaccineBatch> VaccineBatches { get; set; }

    public virtual DbSet<VaccineBatchDetail> VaccineBatchDetails { get; set; }

    public virtual DbSet<VaccineCombo> VaccineCombos { get; set; }

    public virtual DbSet<VaccineComboDetail> VaccineComboDetails { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        //=> optionsBuilder.UseSqlServer("Server=ALIEN\\SQLEXPRESS;Database=VaccinationSystem;Trusted_Connection=True;TrustServerCertificate=True;");
    //        => optionsBuilder.UseSqlServer("Server=tcp:vaccinationsystem.database.windows.net,1433;Initial Catalog=VaccinationSystem;Persist Security Info=False;User ID=baohd;Password=StrongPass123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    public static string GetConnectionString(string connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        string connectionString = config.GetConnectionString(connectionStringName);
        return connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString("DefaultConnection")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__43AA4141D507E51E");

            entity.ToTable("Admin");

            entity.HasIndex(e => e.UserName, "UQ__Admin__7C9273C4A5C05285").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Admin__AB6E61643BE6D2B2").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.BloodType)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("blood_type");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__A50828FCCAF8FD8B");

            entity.ToTable("Appointment");

            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.AppointmentDate)
                .HasColumnType("datetime")
                .HasColumnName("appointment_date");
            entity.Property(e => e.ChildId).HasColumnName("child_id");
            entity.Property(e => e.ComboId).HasColumnName("combo_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");
            entity.Property(e => e.VaccineType)
                .HasMaxLength(10)
                .HasColumnName("vaccine_type");

            entity.HasOne(d => d.Child).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("FK__Appointme__child__6E01572D");

            entity.HasOne(d => d.Customer).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Appointme__custo__6D0D32F4");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK__Appointme__docto__6FE99F9F");

            entity.HasOne(d => d.Staff).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK__Appointme__staff__6EF57B66");
        });

        modelBuilder.Entity<Child>(entity =>
        {
            entity.HasKey(e => e.ChildId).HasName("PK__Child__015ADC05D6123B49");

            entity.ToTable("Child");

            entity.Property(e => e.ChildId).HasColumnName("child_id");
            entity.Property(e => e.BloodType)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("blood_type");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");

            entity.HasOne(d => d.Customer).WithMany(p => p.Children)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Child__customer___3D5E1FD2");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB8556D2BA61");

            entity.ToTable("Customer");

            entity.HasIndex(e => e.UserName, "UQ__Customer__7C9273C49DCBDD5D").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Customer__AB6E6164FB0A8D82").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__Customer__B43B145F32BA75DA").IsUnique();

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.BloodType)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("blood_type");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctor__F39935640AC18A1B");

            entity.ToTable("Doctor");

            entity.HasIndex(e => e.Phone, "UQ__Doctor__B43B145FD55A76B1").IsUnique();

            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.Degree)
                .HasMaxLength(255)
                .HasColumnName("degree");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.ExperienceYears).HasColumnName("experience_years");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Feedback__60883D90A2C3DA46");

            entity.ToTable("Feedback");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");

            entity.HasOne(d => d.Appointment).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Feedback__appoin__7D439ABD");

            entity.HasOne(d => d.Customer).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Feedback__custom__7B5B524B");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Feedback__doctor__7C4F7684");
        });

        modelBuilder.Entity<HealthRecord>(entity =>
        {
            entity.HasKey(e => e.RecordId).HasName("PK__HealthRe__BFCFB4DD288E913D");

            entity.ToTable("HealthRecord");

            entity.Property(e => e.RecordId).HasColumnName("record_id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.AteBeforeVaccine).HasColumnName("ate_before_vaccine");
            entity.Property(e => e.BloodPressure)
                .HasMaxLength(50)
                .HasColumnName("blood_pressure");
            entity.Property(e => e.ConditionOk).HasColumnName("condition_ok");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.HeartRate)
                .HasMaxLength(50)
                .HasColumnName("heart_rate");
            entity.Property(e => e.Height)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("height");
            entity.Property(e => e.ReactionNotes).HasColumnName("reaction_notes");
            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Temperature)
                .HasColumnType("decimal(4, 2)")
                .HasColumnName("temperature");
            entity.Property(e => e.Weight)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("weight");

            entity.HasOne(d => d.Appointment).WithMany(p => p.HealthRecords)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__HealthRec__appoi__76969D2E");

            entity.HasOne(d => d.Doctor).WithMany(p => p.HealthRecords)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__HealthRec__docto__778AC167");

            entity.HasOne(d => d.Staff).WithMany(p => p.HealthRecords)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__HealthRec__staff__75A278F5");
        });

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.HasKey(e => e.HolidayId).HasName("PK__Holiday__253884EA055747A3");

            entity.ToTable("Holiday");

            entity.Property(e => e.HolidayId).HasColumnName("holiday_id");
            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.DateFrom).HasColumnName("date_from");
            entity.Property(e => e.DateTo).HasColumnName("date_to");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .HasColumnName("reason");

            entity.HasOne(d => d.Admin).WithMany(p => p.Holidays)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Holiday__admin_i__02FC7413");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__F58DFD49FCB28C43");

            entity.ToTable("Invoice");

            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Invoice__custome__4E88ABD4");
        });

        modelBuilder.Entity<InvoiceDetail>(entity =>
        {
            entity.HasKey(e => e.DetailId).HasName("PK__InvoiceD__38E9A22452EA4BDD");

            entity.ToTable("InvoiceDetail");

            entity.Property(e => e.DetailId).HasColumnName("detail_id");
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.ComboId).HasColumnName("combo_id");
            entity.Property(e => e.InvoiceId).HasColumnName("invoice_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");

            entity.HasOne(d => d.Invoice).WithMany(p => p.InvoiceDetails)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__InvoiceDe__invoi__5535A963");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__1963DD9C0479EEB4");

            entity.HasIndex(e => e.UserName, "UQ__Staff__7C9273C4E5EC4089").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Staff__AB6E616430E163F2").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__Staff__B43B145F69B2F366").IsUnique();

            entity.Property(e => e.StaffId).HasColumnName("staff_id");
            entity.Property(e => e.Dob).HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("user_name");
        });

        modelBuilder.Entity<Vaccine>(entity =>
        {
            entity.HasKey(e => e.VaccineId).HasName("PK__Vaccine__B593EFB3F29BBCAF");

            entity.ToTable("Vaccine");

            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .HasColumnName("description");
            entity.Property(e => e.InternalDurationDoses).HasColumnName("internal_duration_doses");
            entity.Property(e => e.MaxLateDate).HasColumnName("max_late_date");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<VaccineBatch>(entity =>
        {
            entity.HasKey(e => e.BatchNumber).HasName("PK__VaccineB__56E3783691982155");

            entity.ToTable("VaccineBatch");

            entity.Property(e => e.BatchNumber)
                .HasMaxLength(50)
                .HasColumnName("batch_number");
            entity.Property(e => e.Country)
                .HasMaxLength(255)
                .HasColumnName("country");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.ManufactureDate).HasColumnName("manufacture_date");
            entity.Property(e => e.Manufacturer)
                .HasMaxLength(255)
                .HasColumnName("manufacturer");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        modelBuilder.Entity<VaccineBatchDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("VaccineBatchDetail");

            entity.Property(e => e.BatchNumber)
                .HasMaxLength(50)
                .HasColumnName("batch_number");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");

            entity.HasOne(d => d.BatchNumberNavigation).WithMany()
                .HasForeignKey(d => d.BatchNumber)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__VaccineBa__batch__5EBF139D");

            entity.HasOne(d => d.Vaccine).WithMany()
                .HasForeignKey(d => d.VaccineId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__VaccineBa__vacci__5FB337D6");
        });

        modelBuilder.Entity<VaccineCombo>(entity =>
        {
            entity.HasKey(e => e.ComboId).HasName("PK__VaccineC__18F74AA37256A032");

            entity.ToTable("VaccineCombo");

            entity.Property(e => e.ComboId).HasColumnName("combo_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
        });

        modelBuilder.Entity<VaccineComboDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("VaccineComboDetail");

            entity.Property(e => e.ComboId).HasColumnName("combo_id");
            entity.Property(e => e.VaccineId).HasColumnName("vaccine_id");

            entity.HasOne(d => d.Combo).WithMany()
                .HasForeignKey(d => d.ComboId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__VaccineCo__combo__6477ECF3");

            entity.HasOne(d => d.Vaccine).WithMany()
                .HasForeignKey(d => d.VaccineId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__VaccineCo__vacci__656C112C");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
