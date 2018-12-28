namespace Xemio.Logic.Services.IdGenerator
{
    public interface IIdGenerator
    {
        string Generate<T>();
    }
}