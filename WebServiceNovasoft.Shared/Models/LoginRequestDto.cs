using System.ComponentModel.DataAnnotations;

namespace WebServiceNovasoft.Shared.Models
{
    public class LoginRequestDto
    {
        [Required]
        public string userLogin { get; set; } = "pruebaTecnica";

        [Required]
        public string password { get; set; } = "P@ssw0rd";

        [Required]
        public string connectionName { get; set; } = "DataPower";
    }
}
