using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vaccine.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;
        string accessCode ="https://accounts.google.com/o/oauth2/auth?client_id=1006543489483-mrg7qa1pas18ulb0hvnadiagh8jajghs.apps.googleusercontent.com&response_type=code&approval_prompt=force&access_type=offline&redirect_uri=https://localhost:7090/signin-google&scope=openid email profile https://mail.google.com/ ";

        public AuthController(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClient = httpClientFactory.CreateClient();
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
