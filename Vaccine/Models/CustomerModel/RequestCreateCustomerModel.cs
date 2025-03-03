using Swashbuckle.AspNetCore.Filters;
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

    /// <summary>
    /// Example request model for Swagger documentation
    /// </summary>
    public class ExampleCreateCustomerModel : IExamplesProvider<RequestCreateCustomerModel>
    {
        public RequestCreateCustomerModel GetExamples()
        {
            return new RequestCreateCustomerModel
            {
                Name = "John Doe",
                Email = "johndoe@gmail.com",
                Password = "ggid"
            };
        }
    }


}
