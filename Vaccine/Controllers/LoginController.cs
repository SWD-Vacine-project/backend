//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Vaccine.Repo.UnitOfWork;

//namespace Vaccine.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class LoginController : ControllerBase
//    {
//        private readonly UnitOfWork _unitOfWork;
//        private readonly TokenService _tokenService;

//        public LoginController(UnitOfWork unitOfWork, TokenService tokenService)
//        {
//            _unitOfWork = unitOfWork;
//            _tokenService = tokenService;
//        }

//        [HttpPost("loginadmin")]
//        public IActionResult LoginAdmin([FromBody] LoginModelAdmin loginModel)
//        {
//            // Kiểm tra trong bảng Admin
//            var admin = _unitOfWork.AdminRepository.Get(x => x.Username == loginModel.Username && x.Password == loginModel.Password && x.Delete == 1).FirstOrDefault();
//            if (admin != null)
//            {
//                var token = _tokenService.GenerateToken(admin.Username, admin.Role);
//                return Ok(new { token, id = admin.AdminID, Username = admin.Username, Role = admin.Role });
//            }

//            return Unauthorized("Invalid username or password.");
//        }

//        [HttpPost("logincustomer")]
//        public IActionResult LoginCustomer([FromBody] LoginModelCustomer loginModel)
//        {
//            // Kiểm tra trong bảng Customer
//            var customer = _unitOfWork.CustomerRepository.Get(x => x.Email == loginModel.Username && x.Password == loginModel.Password && x.Delete == 1).FirstOrDefault();
//            if (customer != null)
//            {
//                var token = _tokenService.GenerateToken(customer.CustomerName, "Customer");
//                return Ok(new { token, customerName = customer.CustomerName, customerId = customer.CustomerID });
//            }

//            return Unauthorized("Invalid username or password.");
//        }
//    }
//}
