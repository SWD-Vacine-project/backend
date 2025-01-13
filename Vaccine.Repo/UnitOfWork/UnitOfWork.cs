using MilkStore.Repo.Repository;
using Vaccine.Repo.Entities;

namespace MilkStore.Repo.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private VaccineDbContext _context;
        private GenericRepository<Appointment> _appointment;
        private GenericRepository<Child> _child;
        private GenericRepository<Feedback> _feedback;
        private GenericRepository<Notification> _notification;
        private GenericRepository<PackageDetail> _packageDetail;
        private GenericRepository<Payment> _payment;
        private GenericRepository<Role> _role;
        private GenericRepository<ServicePackage> _servicePackage;
        private GenericRepository<UserAccount> _userAccount;
        private GenericRepository<VaccinationRecord> _vaccinationRecord;
        private GenericRepository<VaccineReaction> _vaccineReaction;

        public UnitOfWork(VaccineDbContext context)
        {
            _context = context;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // property IUnitOfWork ket noi voi repo thong qua UnitOfWork
        public GenericRepository<Appointment> AppointmentRepository
        {
            get
            {
                if (_appointment == null)
                {
                    _appointment = new GenericRepository<Appointment>(_context);
                }
                return _appointment;
            }
        }

        public GenericRepository<Child> ChildRepository
        {
            get
            {
                if (_child == null)
                {
                    _child = new GenericRepository<Child>(_context);
                }
                return _child;
            }
        }

        public GenericRepository<Feedback> FeedbackRepository
        {
            get
            {
                if (_feedback == null)
                {
                    _feedback = new GenericRepository<Feedback>(_context);
                }
                return _feedback;
            }
        }

        public GenericRepository<Notification> NotificationRepository
        {
            get
            {
                if (_notification == null)
                {
                    _notification = new GenericRepository<Notification>(_context);
                }
                return _notification;
            }
        }

        public GenericRepository<PackageDetail> PackageDetailRepository
        {
            get
            {
                if (_packageDetail == null)
                {
                    _packageDetail = new GenericRepository<PackageDetail>(_context);
                }
                return _packageDetail;
            }
        }

        public GenericRepository<Payment> PaymentRepository
        {
            get
            {
                if (_payment == null)
                {
                    _payment = new GenericRepository<Payment>(_context);
                }
                return _payment;
            }
        }

        public GenericRepository<Role> RoleRepository
        {
            get
            {
                if (_role == null)
                {
                    _role = new GenericRepository<Role>(_context);
                }
                return _role;
            }
        }

        public GenericRepository<ServicePackage> ServicePackageRepository
        {
            get
            {
                if (_servicePackage == null)
                {
                    _servicePackage = new GenericRepository<ServicePackage>(_context);
                }
                return _servicePackage;
            }
        }

        public GenericRepository<UserAccount> UserAccountRepository
        {
            get
            {
                if (_userAccount == null)
                {
                    _userAccount = new GenericRepository<UserAccount>(_context);
                }
                return _userAccount;
            }
        }

        public GenericRepository<VaccinationRecord> VaccinationRecordRepository
        {
            get
            {
                if (_vaccinationRecord == null)
                {
                    _vaccinationRecord = new GenericRepository<VaccinationRecord>(_context);
                }
                return _vaccinationRecord;
            }
        }

        public GenericRepository<VaccineReaction> VaccineReactionRepository
        {
            get
            {
                if (_vaccineReaction == null)
                {
                    _vaccineReaction = new GenericRepository<VaccineReaction>(_context);
                }
                return _vaccineReaction;
            }
        }
    }
}
