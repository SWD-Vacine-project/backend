using Vaccine.Repo.Entities;

namespace Vaccine.API.Models.CustomerModel
{
    public class RequestCreateCustomerModel
    {
        public string Name { get; set; } = null!;

        public string? Email { get; set; }

        public string Password { get; set; } = null!;

        //public string? Phone { get; set; } = null!; // Ensure it's required

    }
}
