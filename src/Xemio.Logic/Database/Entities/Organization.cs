using System.Collections.Generic;

namespace Xemio.Logic.Database.Entities
{
    public class Organization : AggregateRoot 
    {
        public Organization()
        {
            this.Members = new List<Member>();
        }

        public string Name { get; set; }
        public List<Member> Members { get; set; }
    }

    public class Member : Entity
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
    }
}
