namespace Xemio.Logic.Database.Entities
{
    public class Notebook : AggregateRoot
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public string ParentNotebookId { get; set; }
    }
}