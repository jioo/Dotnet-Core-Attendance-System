using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels
{
    public class LogInOutViewModel 
    {
        [Required]
        public string CardNo { get; set; }
        [Required]
        public string Password { get; set; }
    }
}