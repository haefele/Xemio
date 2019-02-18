namespace Xemio.Logic.Services.EntityId
{
    public interface IEntityIdManager
    {
        string GenerateNew<T>();
        string TrimCollectionNameFromId<T>(string idWithCollectionName);
        string AddCollectionName<T>(string idWithoutCollectionName);
    }
}