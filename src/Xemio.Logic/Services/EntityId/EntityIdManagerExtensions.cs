namespace Xemio.Logic.Services.EntityId
{
    public static class EntityIdManagerExtensions
    {
        public static string TryAddCollectionName<T>(this IEntityIdManager self, string idWithoutCollectionName)
        {
            if (string.IsNullOrWhiteSpace(idWithoutCollectionName))
                return null;

            return self.AddCollectionName<T>(idWithoutCollectionName);
        }
    }
}