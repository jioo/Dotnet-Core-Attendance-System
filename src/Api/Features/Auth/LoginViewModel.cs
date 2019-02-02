using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Auth
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}