using NUlid;
using Raven.Client.Documents;

namespace Xemio.Logic.Services.EntityId
{
    public class EntityIdManager : IEntityIdManager
    {
        private readonly IDocumentStore _documentStore;

        public EntityIdManager(IDocumentStore documentStore)
        {
            Guard.NotNull(documentStore, nameof(documentStore));

            this._documentStore = documentStore;
        }

        public string GenerateNew<T>()
        {
            var id = Ulid.NewUlid().ToString();
            return this.AddCollectionName<T>(id);
        }

        public string TrimCollectionNameFromId<T>(string idWithCollectionName)
        {
            var collectionName = this._documentStore.Conventions.GetCollectionName(typeof(T));
            var idPrefix = this._documentStore.Conventions.TransformTypeCollectionNameToDocumentIdPrefix(collectionName);

            if (idWithCollectionName.StartsWith(idPrefix))
                return idWithCollectionName.Substring(idPrefix.Length + 1); // +1 because of the slash between collection name and id

            return idWithCollectionName;
        }

        public string AddCollectionName<T>(string idWithoutCollectionName)
        {
            var collectionName = this._documentStore.Conventions.GetCollectionName(typeof(T));
            var idPrefix = this._documentStore.Conventions.TransformTypeCollectionNameToDocumentIdPrefix(collectionName);

            return idPrefix + "/" + idWithoutCollectionName;
        }

        public string GenerateNotebookHierarchyId(string userId)
        {
            return userId + "/notebookHierarchy";
        }
    }
}