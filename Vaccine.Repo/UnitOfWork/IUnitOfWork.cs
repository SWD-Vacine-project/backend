//using MilkStore.Repo.Repository;

//namespace MilkStore.Repo.UnitOfWork
//{
//    public interface IUnitOfWork : IDisposable
//    {
//        void Save();
//        Task<int> SaveAsync();

//        public GenericRepository<Admin> AdminRepository { get; }
//        public GenericRepository<AgeRange> AgeRangeRepository { get; }
//        public GenericRepository<BrandMilk> BrandMilkRepository { get; }
//        public GenericRepository<Company> CompanyRepository { get; }
//        public GenericRepository<Country> CountryRepository { get; }
//        public GenericRepository<Customer> CustomerRepository { get; }
//        public GenericRepository<DeliveryMan> DeliveryManRepository { get; }

//        public GenericRepository<Order> OrderRepository { get; }
//        public GenericRepository<OrderDetail> OrderDetailRepository { get; }
//        //public GenericRepository<Payment> PaymentRepository { get; }
//        public GenericRepository<ProductItem> ProductItemRepository { get; }
//        public GenericRepository<Storage> StorageRepository { get; }
//    }
//}