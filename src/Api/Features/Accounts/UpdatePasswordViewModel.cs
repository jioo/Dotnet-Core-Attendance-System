using System.ComponentModel.DataAnnotations;

namespace WebApi.Features.Accounts
{
    public class UpdatePasswordViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }
    }
}