namespace Xemio.Client.Data.Actions
{
    public class RegisterUserAction
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    public class LoginUserAction
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }
}