namespace Xemio.Logic.Entities
{
    public class User : AggregateRoot
    {
        public string EmailAddress { get; set; }
        public string PasswordHash { get; set; }
    }
}