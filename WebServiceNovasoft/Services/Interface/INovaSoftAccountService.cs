using WebServiceNovasoft.Shared.Models;

namespace WebServiceNovasoft.Services.Interface
{
    public interface INovaSoftAccountService
    {
        Task<bool> CreateAccountAsync(CreateAccountRequestDto createAccountRequest);
        Task<List<AccountDto>> GetAllAccountsAsync();
        Task<AccountDto?> GetAccountByIdAsync(string accountId);
    }
}
