using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Logs
{
    public class LogInOutViewModel 
    {
        [Required]
        public string CardNo { get; set; }
        [Required]
        public string Password { get; set; }
    }
}