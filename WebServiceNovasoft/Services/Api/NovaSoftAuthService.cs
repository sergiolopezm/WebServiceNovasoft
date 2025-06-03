using System.Net.Http.Json;
using System.Text.Json;
using WebServiceNovasoft.Services.Interface;
using WebServiceNovasoft.Shared.Models;

namespace WebServiceNovasoft.Services.Api
{
    public class NovaSoftAuthService : INovaSoftAuthService
    {
        private readonly HttpClient _httpClient;
        private string? _currentToken;
        private DateTime? _tokenExpiration;

        public NovaSoftAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                // CAMBIO: Ahora llama al proxy local en lugar de la API externa
                var response = await _httpClient.PostAsJsonAsync("api/NovaSoftProxy/login", loginRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Proxy Response Status: {response.StatusCode}");
                Console.WriteLine($"Proxy Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.token))
                    {
                        _currentToken = loginResponse.token;
                        _tokenExpiration = loginResponse.expiration;
                        return loginResponse;
                    }
                }

                throw new InvalidOperationException($"Login failed: {response.StatusCode} - {responseContent}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HttpRequestException: {ex.Message}");
                throw new InvalidOperationException($"Error de conexión con el proxy: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login Exception: {ex.Message}");
                throw new InvalidOperationException($"Login error: {ex.Message}");
            }
        }

        public async Task<string> GetTokenAsync()
        {
            if (IsTokenValid())
            {
                return _currentToken!;
            }

            var loginRequest = new LoginRequestDto();
            var loginResponse = await LoginAsync(loginRequest);

            if (!string.IsNullOrEmpty(loginResponse.token))
            {
                return loginResponse.token;
            }

            throw new InvalidOperationException("No se pudo obtener token válido");
        }

        public bool IsTokenValid()
        {
            return !string.IsNullOrEmpty(_currentToken) &&
                   _tokenExpiration.HasValue &&
                   _tokenExpiration.Value > DateTime.Now.AddMinutes(-5);
        }

        public void ClearToken()
        {
            _currentToken = null;
            _tokenExpiration = null;
        }
    }
}