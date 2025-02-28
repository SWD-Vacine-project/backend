namespace MilkStore.API.Models.CustomerModel
{
    public class ResponseCreateCustomerModel
    {
        public string CustomerName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

    }
}