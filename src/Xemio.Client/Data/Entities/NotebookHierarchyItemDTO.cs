using System.Collections.Generic;

namespace Xemio.Client.Data.Entities
{
    public class NotebookHierarchyItemDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<NotebookHierarchyItemDTO> Notebooks { get; set; }
    }
}