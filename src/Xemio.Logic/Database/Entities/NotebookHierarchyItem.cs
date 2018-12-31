using System.Collections.Generic;

namespace Xemio.Logic.Database.Entities
{
    public class NotebookHierarchyItem
    {
        public NotebookHierarchyItem()
        {
            this.Notebooks = new List<NotebookHierarchyItem>();
        }

        public string NotebookId { get; set; }
        public string NotebookName { get; set; }
        public List<NotebookHierarchyItem> Notebooks { get; set; }
    }
}