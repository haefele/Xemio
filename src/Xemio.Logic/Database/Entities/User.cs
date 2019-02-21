namespace Xemio.Logic.Database.Entities
{
    public class User : AggregateRoot
    {
        public string EmailAddress { get; set; }
        public string PasswordHash { get; set; }
        public string DisplayName { get; set; }
    }
}