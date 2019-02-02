using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Accounts
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string CardNo { get; set; }
        public string Position { get; set; } 
    }
}