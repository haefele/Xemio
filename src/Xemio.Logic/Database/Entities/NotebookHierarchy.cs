using System.Collections.Generic;
using System.Linq;
using Xemio.Logic.Extensions;

namespace Xemio.Logic.Database.Entities
{
    public class NotebookHierarchy : AggregateRoot
    {
        public NotebookHierarchy()
        {
            this.Notebooks = new List<NotebookHierarchyItem>();
        }

        public string UserId { get; set; }
        public List<NotebookHierarchyItem> Notebooks { get; set; }

        public void AddNewNotebook(Notebook notebook)
        {
            var item = new NotebookHierarchyItem
            {
                NotebookId = notebook.Id,
            };

            this.Update(notebook, item);
        }
        public NotebookHierarchyItem RemoveNotebook(Notebook notebook)
        {
            var parent = this.Notebooks.Flatten(f => f.Notebooks).FirstOrDefault(f => f.Notebooks.Any(d => d.NotebookId == notebook.Id));
            var item = this.Notebooks.Flatten(f => f.Notebooks).FirstOrDefault(f => f.NotebookId == notebook.Id);

            if (parent != null)
            {
                parent.Notebooks.Remove(item);
            }
            else
            {
                this.Notebooks.Remove(item);
            }

            return item;
        }
        public void UpdateNotebook(Notebook notebook)
        {
            var removedItem = this.RemoveNotebook(notebook);
            this.Update(notebook, removedItem);
        }

        private void Update(Notebook notebook, NotebookHierarchyItem item)
        {
            item.NotebookName = notebook.Name;

            if (notebook.ParentNotebookId != null)
            {
                var parentNotebook = this.Notebooks.Flatten(f => f.Notebooks).First(f => f.NotebookId == notebook.ParentNotebookId);
                parentNotebook.Notebooks.Add(item);
            }
            else
            {
                this.Notebooks.Add(item);
            }
        }
    }
}