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
}
