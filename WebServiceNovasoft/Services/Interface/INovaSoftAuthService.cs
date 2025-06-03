using WebServiceNovasoft.Shared.Models;

namespace WebServiceNovasoft.Services.Interface
{
    public interface INovaSoftAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<string> GetTokenAsync();
        bool IsTokenValid();
        void ClearToken();
    }
}
