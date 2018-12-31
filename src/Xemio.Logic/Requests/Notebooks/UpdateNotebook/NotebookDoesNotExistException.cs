using System;

namespace Xemio.Logic.Requests.Notebooks.UpdateNotebook
{
    [Serializable]
    public class NotebookDoesNotExistException : XemioException
    {
        public string NotebookId { get; }

        public NotebookDoesNotExistException(string notebookId)
        {
            this.NotebookId = notebookId;
        }
    }
}