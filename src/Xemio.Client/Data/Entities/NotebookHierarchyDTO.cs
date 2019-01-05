using System;
using System.Collections.Generic;
using System.Text;

namespace Xemio.Client.Data.Entities
{
    public class NotebookHierarchyDTO
    {
        public string UserId { get; set; }
        public List<NotebookHierarchyItemDTO> Notebooks { get; set; }
    }

    public class NotebookDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public string ParentNotebookId { get; set; }
    }
}
