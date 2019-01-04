using System;

namespace Xemio.Logic.Requests.Notebooks
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