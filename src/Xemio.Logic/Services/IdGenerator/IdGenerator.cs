using NUlid;
using Raven.Client.Documents;

namespace Xemio.Logic.Services.IdGenerator
{
    public class IdGenerator : IIdGenerator
    {
        private readonly IDocumentStore _documentStore;

        public IdGenerator(IDocumentStore documentStore)
        {
            Guard.NotNull(documentStore, nameof(documentStore));

            this._documentStore = documentStore;
        }

        public string Generate<T>()
        {
            var collectionName = this._documentStore.Conventions.GetCollectionName(typeof(T));
            var idPrefix = this._documentStore.Conventions.TransformTypeCollectionNameToDocumentIdPrefix(collectionName);

            return idPrefix + "/" + Ulid.NewUlid();
        }
    }
}