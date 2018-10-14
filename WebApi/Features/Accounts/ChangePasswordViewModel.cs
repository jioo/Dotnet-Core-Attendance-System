namespace WebApi.Features.Accounts
{
    public class ChangePasswordViewModel 
    {
        public string UserName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}