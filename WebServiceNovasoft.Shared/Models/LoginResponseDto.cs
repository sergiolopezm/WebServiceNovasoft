namespace WebServiceNovasoft.Shared.Models
{
    public class LoginResponseDto
    {
        public string token { get; set; } = string.Empty;
        public DateTime expiration { get; set; }
    }
}
