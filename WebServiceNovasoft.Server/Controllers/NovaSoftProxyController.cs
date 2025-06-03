using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceNovasoft.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NovaSoftProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NovaSoftProxyController> _logger;

        public NovaSoftProxyController(IHttpClientFactory httpClientFactory, ILogger<NovaSoftProxyController> logger)
        {
            _httpClient = httpClientFactory.CreateClient("NovaSoftAPI");
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] object loginRequest)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation("Proxying login request to NovaSoft API");

                var response = await _httpClient.PostAsync("WebAPI/api/Cuenta/Login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"NovaSoft API response: {response.StatusCode}");

                // MANTENER EL STATUS CODE ORIGINAL
                Response.StatusCode = (int)response.StatusCode;
                return Content(responseContent, "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error proxying login request");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("accounts")]
        public async Task<IActionResult> CreateAccount([FromBody] object createAccountRequest)
        {
            try
            {
                var token = GetBearerToken();
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Bearer token required");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var jsonContent = JsonSerializer.Serialize(createAccountRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("WebAPI/api/CXC/Senior/Accounts", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Create Account Response: {response.StatusCode} - {responseContent}");

                // MANTENER EL STATUS CODE ORIGINAL - MUY IMPORTANTE
                Response.StatusCode = (int)response.StatusCode;
                return Content(responseContent, "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("accounts")]
        public async Task<IActionResult> GetAllAccounts()
        {
            return await ProxyGetRequest("WebAPI/api/CXC/Senior/Accounts");
        }

        [HttpGet("accounts/{id}")]
        public async Task<IActionResult> GetAccountById(string id)
        {
            return await ProxyGetRequest($"WebAPI/api/CXC/Senior/Accounts/{id}");
        }

        private async Task<IActionResult> ProxyGetRequest(string endpoint)
        {
            try
            {
                var token = GetBearerToken();
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized("Bearer token required");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync(endpoint);
                var responseContent = await response.Content.ReadAsStringAsync();

                // MANTENER EL STATUS CODE ORIGINAL
                Response.StatusCode = (int)response.StatusCode;
                return Content(responseContent, "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error proxying GET request to {endpoint}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private string? GetBearerToken()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }
            return null;
        }
    }
}
