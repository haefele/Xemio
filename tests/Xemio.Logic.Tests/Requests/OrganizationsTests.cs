using System.Threading.Tasks;
using Xemio.Logic.Requests.Organizations;
using Xunit;

namespace Xemio.Logic.Tests.Requests
{
    public class OrganizationsTests : RequestTests
    {
        [Fact]
        public async Task CreateOrganizationQueryAndDeleteIt() 
        {
            var organizationName = "Test-Organization";
            string organizationId;

            var token = await this.CreateUserAndLogin();

            using (var context = this.RequestManager.StartRequestContext()) 
            {
                context.CurrentUser = token;

                await context.Send(new CreateOrganizationRequest 
                {
                    Name = organizationName,
                });

                await context.CommitAsync();
            }

            using (var context = this.RequestManager.StartRequestContext()) 
            {
                context.CurrentUser = token;

                var myOrganizations = await context.Send(new GetMyOrganizationsRequest());
                Assert.NotNull(myOrganizations);
                Assert.NotEmpty(myOrganizations);
                Assert.Single(myOrganizations);
                Assert.Equal(organizationName, myOrganizations[0].Name);

                organizationId = myOrganizations[0].Id;
            }

            using (var context = this.RequestManager.StartRequestContext()) 
            {
                context.CurrentUser = token;

                await context.Send(new DeleteOrganizationRequest 
                {
                    OrganizationId = organizationId
                });

                await context.CommitAsync();
            }

            using (var context = this.RequestManager.StartRequestContext()) 
            {
                context.CurrentUser = token;

                var myOrganizations = await context.Send(new GetMyOrganizationsRequest());
                Assert.Empty(myOrganizations);
            }
        }
    }
}