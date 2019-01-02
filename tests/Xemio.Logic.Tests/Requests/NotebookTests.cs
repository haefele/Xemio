using System.Threading.Tasks;
using Xemio.Client.Data.Entities;
using Xemio.Logic.Database.Entities;
using Xemio.Logic.Requests.Notebooks.CreateNotebook;
using Xemio.Logic.Requests.Notebooks.GetNotebookHierarchy;
using Xemio.Logic.Requests.Notebooks.UpdateNotebook;
using Xunit;

namespace Xemio.Logic.Tests.Requests
{
    public class NotebookTests : RequestTests
    {
        [Fact]
        public async Task CanCreateUpdateDeleteAndGetNotebookHierarchy()
        {
            var authToken = await base.CreateUserAndLogin();

            string notebookName = "Programming";
            string notebookId;

            string childNotebookName = ".NET";
            string childNotebookId;

            string secondChildNotebookName = "JavaScript";
            string secondChildNotebookId;

            using (var context = this.RequestManager.StartRequestContext())
            {
                context.CurrentUser = authToken;

                var notebook = await context.Send(new CreateNotebookRequest
                {
                    Name = notebookName,
                    ParentNotebookId = null
                });

                notebookId = notebook.Id;

                await context.CommitAsync();
            }

            using (var context = this.RequestManager.StartRequestContext())
            {
                context.CurrentUser = authToken;

                var childNotebook = await context.Send(new CreateNotebookRequest
                {
                    Name = childNotebookName,
                    ParentNotebookId = notebookId
                });

                childNotebookId = childNotebook.Id;

                await context.CommitAsync();
            }

            using (var context = this.RequestManager.StartRequestContext())
            {
                context.CurrentUser = authToken;

                var childNotebook = await context.Send(new CreateNotebookRequest
                {
                    Name = secondChildNotebookName,
                    ParentNotebookId = notebookId
                });

                secondChildNotebookId = childNotebook.Id;

                await context.CommitAsync();
            }

            using (var context = this.RequestManager.StartRequestContext())
            {
                context.CurrentUser = authToken;

                var updatedChildNotebook = await context.Send(new UpdateNotebookRequest
                {
                    NotebookId = childNotebookId,
                    UpdateParentNotebookId = true,
                    ParentNotebookId = null
                });

                await context.CommitAsync();
            }

            using (var context = this.RequestManager.StartRequestContext())
            {
                context.CurrentUser = authToken;

                var hierarchy = await context.Send(new GetNotebookHierarchyRequest());

                var dto = this.Mapper.Map<NotebookHierarchy, NotebookHierarchyDTO>(hierarchy);
            }

            await this.WaitForUserToContinueTheTest();
        }
    }
}