//using MilkStore.Repo.Repository;
//using Vaccine.Repo.Entities;

//namespace MilkStore.Repo.UnitOfWork
//{
//    public class UnitOfWork : IUnitOfWork
//    {
//        private VaccineDbContext _context;
//        private GenericRepository<Appointment> _appointment;
//        private GenericRepository<Child> _child;
//        private GenericRepository<Feedback> _feedback;
//        private GenericRepository<Notification> _Notification;
//        private GenericRepository<PackageDetail> _PackageDetail;
//        private GenericRepository<Payment> _Payment;
//        private GenericRepository<Role> _Role;
//        private GenericRepository<ServicePackage> _ServicePackage;
//        private GenericRepository<UserAccount> _UserAccount;
//        private GenericRepository<VaccinationRecord> _VaccinationRecord;
//        private GenericRepository<Vaccine> _Vaccine;
//        private GenericRepository<VaccineReaction> _VaccineReaction;



//        public UnitOfWork(VaccineDbContext context)
//        {
//            _context = context;
//        }




//        public void Save()
//        {
//            _context.SaveChanges();
//        }
//        public async Task<int> SaveAsync()
//        {
//            return await _context.SaveChangesAsync();
//        }
//        private bool disposed = false;

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!this.disposed)
//            {
//                if (disposing)
//                {
//                    _context.Dispose();
//                }
//            }
//            this.disposed = true;
//        }

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        public GenericRepository<Appointment> AppointmentRepository
//        {
//            get
//            {
//                if (_admin == null)
//                {
//                    this._admin = new GenericRepository<Admin>(_context);
//                }
//                return _admin;
//            }
//        }

//        public GenericRepository<AgeRange> AgeRangeRepository
//        {
//            get
//            {
//                if (_ageRange == null)
//                {
//                    this._ageRange = new GenericRepository<AgeRange>(_context);
//                }
//                return _ageRange;
//            }
//        }

//        public GenericRepository<BrandMilk> BrandMilkRepository
//        {
//            get
//            {
//                if (_brandMilk == null)
//                {
//                    this._brandMilk = new GenericRepository<BrandMilk>(_context);
//                }
//                return _brandMilk;
//            }
//        }
//        public GenericRepository<Company> CompanyRepository
//        {
//            get
//            {
//                if (_company == null)
//                {
//                    this._company = new GenericRepository<Company>(_context);
//                }
//                return _company;
//            }
//        }
//        public GenericRepository<Country> CountryRepository
//        {
//            get
//            {
//                if (_country == null)
//                {
//                    this._country = new GenericRepository<Country>(_context);
//                }
//                return _country;
//            }
//        }
//        public GenericRepository<Customer> CustomerRepository
//        {
//            get
//            {
//                if (_customer == null)
//                {
//                    this._customer = new GenericRepository<Customer>(_context);
//                }
//                return _customer;
//            }
//        }
//        public GenericRepository<DeliveryMan> DeliveryManRepository
//        {
//            get
//            {
//                if (_deliveryMan == null)
//                {
//                    this._deliveryMan = new GenericRepository<DeliveryMan>(_context);
//                }
//                return _deliveryMan;
//            }
//        }
//        public GenericRepository<Order> OrderRepository
//        {
//            get
//            {
//                if (_order == null)
//                {
//                    this._order = new GenericRepository<Order>(_context);
//                }
//                return _order;
//            }
//        }
//        public GenericRepository<OrderDetail> OrderDetailRepository
//        {
//            get
//            {
//                if (_orderDetail == null)
//                {
//                    this._orderDetail = new GenericRepository<OrderDetail>(_context);
//                }
//                return _orderDetail;
//            }
//        }
//        //public GenericRepository<Payment> PaymentRepository
//        //{
//        //    get
//        //    {
//        //        if (_payment == null)
//        //        {
//        //            this._payment = new GenericRepository<Payment>(_context);
//        //        }
//        //        return _payment;
//        //    }
//        //}
//        public GenericRepository<ProductItem> ProductItemRepository
//        {
//            get
//            {
//                if (_productItem == null)
//                {
//                    this._productItem = new GenericRepository<ProductItem>(_context);
//                }
//                return _productItem;
//            }
//        }
//        public GenericRepository<Storage> StorageRepository
//        {
//            get
//            {
//                if (_storage == null)
//                {
//                    this._storage = new GenericRepository<Storage>(_context);
//                }
//                return _storage;
//            }
//        }

//        GenericRepository<Admin> IUnitOfWork.AdminRepository => throw new NotImplementedException();

//        GenericRepository<AgeRange> IUnitOfWork.AgeRangeRepository => throw new NotImplementedException();

//        GenericRepository<BrandMilk> IUnitOfWork.BrandMilkRepository => throw new NotImplementedException();

//        GenericRepository<Company> IUnitOfWork.CompanyRepository => throw new NotImplementedException();

//        GenericRepository<Country> IUnitOfWork.CountryRepository => throw new NotImplementedException();

//        GenericRepository<Customer> IUnitOfWork.CustomerRepository => throw new NotImplementedException();

//        GenericRepository<DeliveryMan> IUnitOfWork.DeliveryManRepository => throw new NotImplementedException();

//        GenericRepository<Order> IUnitOfWork.OrderRepository => throw new NotImplementedException();

//        GenericRepository<OrderDetail> IUnitOfWork.OrderDetailRepository => throw new NotImplementedException();

//        //GenericRepository<Payment> IUnitOfWork.PaymentRepository => throw new NotImplementedException();

//        GenericRepository<ProductItem> IUnitOfWork.ProductItemRepository => throw new NotImplementedException();

//        GenericRepository<Storage> IUnitOfWork.StorageRepository => throw new NotImplementedException();
//    }
//}
