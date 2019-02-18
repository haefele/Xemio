namespace Xemio.Client.Data.Endpoints.Notebooks
{
    public class UpdateNotebookAction
    {
        public bool UpdateName { get; set; }
        public string Name { get; set; }
        
        public bool UpdateParentNotebookId { get; set; }
        public string ParentNotebookId { get; set; }
    }
}
