using Vaccine.Repo.Repository;
using Vaccine.Repo.Entities;

namespace Vaccine.Repo.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
        Task<int> SaveAsync();


        public GenericRepository<Appointment> AppointmentRepository{ get; }
        public GenericRepository<Child> ChildRepository           { get; }
        public GenericRepository<Feedback> FeedbackRepository     { get; }
        public GenericRepository<Notification> NotificationRepository { get; }
        public GenericRepository<PackageDetail> PackageDetailRepository { get; }
        public GenericRepository<Payment> PaymentRepository { get; }
        public GenericRepository<Role> RoleRepository { get; }
        public GenericRepository<ServicePackage> ServicePackageRepository { get; }
        public GenericRepository<UserAccount> UserAccountRepository { get; }
        public GenericRepository<VaccinationRecord> VaccinationRecordRepository { get; }
        public GenericRepository<VaccineReaction> VaccineReactionRepository { get; }

    }
}