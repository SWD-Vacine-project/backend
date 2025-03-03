using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [EnableCors("MyPolicy")]
    [ApiController]
    [Route("api/customer")]
    public class CustomerController
    {
      
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;
        private readonly UnitOfWork _unitOfWork;
        public CustomerController(IConfiguration configuration, IHttpClientFactory httpClientFactory, UnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _client = httpClientFactory.CreateClient();
            _unitOfWork = unitOfWork;
        }
        
    }
}
