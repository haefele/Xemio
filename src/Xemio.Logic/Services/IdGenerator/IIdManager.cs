namespace Xemio.Logic.Services.IdGenerator
{
    public interface IIdManager
    {
        string GenerateNew<T>();
        string TrimCollectionNameFromId<T>(string idWithCollectionName);
        string AddCollectionName<T>(string idWithoutCollectionName);
    }
}