using System.ComponentModel.DataAnnotations;

namespace WebServiceNovasoft.Shared.Models
{
    public class CreateAccountRequestDto
    {
        [Required]
        public string codCli { get; set; } = string.Empty;

        [Required]
        public string nomCli { get; set; } = string.Empty;

        [Required]
        public string nitCli { get; set; } = string.Empty;

        [Required]
        public string codCiu { get; set; } = string.Empty;

        [Required]
        public string codDep { get; set; } = string.Empty;

        [Required]
        public string codPai { get; set; } = string.Empty;

        public string di2Cli { get; set; } = string.Empty;
        public string te1Cli { get; set; } = string.Empty;

        [Required]
        public int tipCli { get; set; }

        [Required]
        public string fecIng { get; set; } = string.Empty;

        public string eMail { get; set; } = string.Empty;
        public string tipIde { get; set; } = string.Empty;
        public string ap1Cli { get; set; } = string.Empty;
        public string nom1Cli { get; set; } = string.Empty;

        [Required]
        public int tipPer { get; set; }

        [Required]
        public string estCli { get; set; } = string.Empty;

        public string codCliExtr { get; set; } = string.Empty;
        public string pagWeb { get; set; } = string.Empty;
    }
}
