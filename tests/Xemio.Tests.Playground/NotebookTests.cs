using System.Threading.Tasks;
using Xemio.Logic.Requests.Notebooks.CreateNotebook;
using Xemio.Logic.Services.JsonWebToken;
using Xunit;

namespace Xemio.Tests.Playground
{
    public class NotebookTests : PlaygroundTests
    {
        [Fact]
        public async Task CreateNotebook()
        {
            var authToken = await base.CreateUserAndLogin();

            using (var context = this.RequestManager.StartRequestContext())
            {
                context.CurrentUser = new AuthToken(authToken);

                var notebook = await context.Send(new CreateNotebookRequest
                {
                    Name = "Test-Notebook",
                    ParentNotebookId = null
                });

                var childNotebook = await context.Send(new CreateNotebookRequest
                {
                    Name = "Child-Notebook",
                    ParentNotebookId = notebook.Id
                });

                await context.CommitAsync();
            }
        }
    }
}