using System.Linq;
using Raven.Client.Documents.Indexes;
using Xemio.Logic.Database.Entities;

namespace Xemio.Logic.Database.Indexes
{
    public class Organizations_ByMemberUserIds : AbstractIndexCreationTask<Organization, Organizations_ByMemberUserIds.Result>
    {
        public class Result 
        {
            public string UserId { get; set; }
        }

        public Organizations_ByMemberUserIds()
        {
            this.Map = organizations =>
                from o in organizations
                from member in o.Members
                select new Result 
                {
                    UserId = member.UserId
                };
        }
    }
}