using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Vaccine.Repo.Entities;
using Vaccine.Repo.UnitOfWork;

namespace Vaccine.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        string accessCode ="https://accounts.google.com/o/oauth2/auth?client_id=1006543489483-mrg7qa1pas18ulb0hvnadiagh8jajghs.apps.googleusercontent.com&response_type=code&approval_prompt=force&access_type=offline&redirect_uri=https://localhost:7090/signin-google&scope=openid email profile https://mail.google.com/ ";
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IConfiguration config, IHttpClientFactory httpClientFactory, IUnitOfWork unitOfWork)
        {
            _config = config;
            _httpClient = httpClientFactory.CreateClient();
            _unitOfWork = unitOfWork; // Store as IUnitOfWork directly
        }

        [HttpGet]
        public IActionResult GetLink()
        {
            return Ok(accessCode);
        }



        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleSignIn([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest("Authorization code is required.");

            var tokenRequest = new Dictionary<string, string>
            {
                { "code", code },
                { "client_id", _config["Google:ClientId"] },
                { "client_secret", _config["Google:ClientSecret"] },
                { "redirect_uri", _config["Google:RedirectUri"] },
                { "grant_type", "authorization_code" }
            };

            var response = await _httpClient.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(tokenRequest));

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Google Response Status Code: {response.StatusCode}");
            Console.WriteLine($"Google Response Headers: {string.Join(", ", response.Headers)}");
            Console.WriteLine($"Google Response: {responseContent}");

            if (!response.IsSuccessStatusCode)
                return BadRequest($"Error getting access token: {responseContent}");

            try
            {
                var tokenResponse = JsonSerializer.Deserialize<GoogleTokenResponse>(
                    responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return Ok(tokenResponse);
            }
            catch (JsonException)
            {
                return BadRequest("Invalid JSON response from Google.");
            }
        }

        // -----------------------Login Customer------------------------
        [HttpPost("login")]
        public IActionResult Login(LoginRequest login)
        {

            if (login == null || String.IsNullOrEmpty(login.UserName) || String.IsNullOrEmpty(login.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }
            dynamic user = null;
            string userRole;
            string preFix = login.UserName.Substring(0, 3);
            if (preFix == "ST_")
            {
                user = _unitOfWork.StaffRepository.
                    Get(s => s.UserName == login.UserName).
                    FirstOrDefault();
                userRole = "Staff";
            }
            else if (preFix == "AD_")
            {
                user = _unitOfWork.AdminRepository.
                    Get(a => a.UserName == login.UserName).
                    FirstOrDefault();
                userRole = "Admin";
            }
            else
            {
                user = _unitOfWork.CustomerRepository.
                    Get(u => u.UserName == login.UserName).
                    FirstOrDefault();
                userRole = "Customer";
            }


            if (user == null)
            {
                return Unauthorized(new { message = "Account does not exist" });
            }
            if (user.Password != login.Password)
            {
                return Unauthorized(new { message = "Password is incorrect" });
            }
            //return Ok(new 
            //{ 
            //    user.Email,
            //    user.Name, 
            //    user.Phone, 
            //    user.Address,
            //    Role = userRole,
            //    Children = user.Children ?? new List<Child>()
            //});
            var response = new
            {
                user.Email,
                user.Name,
                user.Phone,
                Role = userRole
            };

            if (userRole == "Customer")
            {
                return Ok(new
                {
                    response.Email,
                    response.Name,
                    response.Phone,
                    response.Role,
                    Address = user.Address,
                    Children = user.Children ?? new List<Child>()
                });
            }

            return Ok(response);

        }
        public class LoginRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

    

        //=========== Signup Customer =====================
        [HttpPost("signup")]

        public IActionResult Signup(SignupRequest newCustomer)
        {
            if (newCustomer == null || string.IsNullOrEmpty(newCustomer.UserName) ||
                string.IsNullOrEmpty(newCustomer.Password) || string.IsNullOrEmpty(newCustomer.Name) ||
                string.IsNullOrEmpty(newCustomer.Phone))
            {
                return BadRequest(new { message = "Missing required fields." });
            }

            var existingUser = _unitOfWork.CustomerRepository.Get(c => c.UserName == newCustomer.UserName).FirstOrDefault();
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists." });
            }

            var customer = new Customer
            {
                UserName = newCustomer.UserName,
                Password = newCustomer.Password, 
                Name = newCustomer.Name,
                Dob = newCustomer.Dob,
                Gender = newCustomer.Gender,
                Phone = newCustomer.Phone,
                Email = newCustomer.Email,
                Address = newCustomer.Address,
                BloodType = newCustomer.BloodType
            };

            _unitOfWork.CustomerRepository.Insert(customer);
            _unitOfWork.Save();

            return Ok(new { message = "Signup successful", customerId = customer.CustomerId });
        }


        public class SignupRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public DateOnly Dob { get; set; }
            public string? Gender { get; set; }
            public string Phone { get; set; }
            public string? Email { get; set; }
            public string? Address { get; set; }
            public string? BloodType { get; set; }
        }

        //=============================================================================

        [HttpPost("verify-token")]
        public async Task<IActionResult> VerifyGoogleToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
                return BadRequest("ID token is required.");

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
                return Ok(new { payload.Email, payload.Name, payload.Picture });
            }
            catch
            {
                return BadRequest("Invalid token.");
            }
        }

        public class GoogleTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; }

            [JsonPropertyName("id_token")]
            public string IdToken { get; set; }

            [JsonPropertyName("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }
        }

        public class TokenRequest
        {
            public string IdToken { get; set; }
        }
    }
}
