using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using WebServiceNovasoft.Services.Interface;
using WebServiceNovasoft.Shared.Models;

namespace WebServiceNovasoft.Services.Api
{
    public class NovaSoftAccountService : INovaSoftAccountService
    {
        private readonly HttpClient _httpClient;
        private readonly INovaSoftAuthService _authService;

        public NovaSoftAccountService(HttpClient httpClient, INovaSoftAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<bool> CreateAccountAsync(CreateAccountRequestDto createAccountRequest)
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("api/NovaSoftProxy/accounts", createAccountRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Create Account Response: {response.StatusCode} - {responseContent}");

                // VERIFICAR EL STATUS CODE CORRECTAMENTE
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    // Parsear el error para mostrar mensaje específico
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (errorResponse?.Errors != null && errorResponse.Errors.Any())
                        {
                            var errorMessage = string.Join(", ", errorResponse.Errors.Select(e => e.Mensaje));
                            Console.WriteLine($"Error específico: {errorMessage}");
                            throw new InvalidOperationException(errorMessage);
                        }
                    }
                    catch (JsonException)
                    {
                        // Si no se puede parsear el error, usar mensaje genérico
                    }

                    throw new InvalidOperationException($"Error al crear cuenta. Status: {response.StatusCode}. Response: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateAccount Exception: {ex.Message}");
                throw; // Re-lanzar la excepción para que se muestre en el UI
            }
        }

        public async Task<List<AccountDto>> GetAllAccountsAsync()
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("api/NovaSoftProxy/accounts");
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Get Accounts Response: {response.StatusCode} - {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var accounts = JsonSerializer.Deserialize<List<AccountDto>>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return accounts ?? new List<AccountDto>();
                }
                else
                {
                    Console.WriteLine($"Error al obtener cuentas: {response.StatusCode}");
                    return new List<AccountDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllAccounts Exception: {ex.Message}");
                return new List<AccountDto>();
            }
        }

        public async Task<AccountDto?> GetAccountByIdAsync(string accountId)
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"api/NovaSoftProxy/accounts/{accountId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Get Account By ID Response: {response.StatusCode} - {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var account = JsonSerializer.Deserialize<AccountDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return account;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAccountById Exception: {ex.Message}");
                return null;
            }
        }
    }

    // Clase para manejar los errores de la API
    public class ErrorResponse
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public string TraceId { get; set; } = string.Empty;
        public List<ErrorDetail> Errors { get; set; } = new();
    }

    public class ErrorDetail
    {
        public string Codigo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}
