using AutoMapper;
using Xemio.Logic.Extensions;
using Xemio.Logic.Services.EntityId;

namespace Xemio.Logic.AutoMapper.ValueConverter
{
    public class TrimCollectionNameFromIdConverter<T> : IValueConverter<string, string>
    {
        private readonly IEntityIdManager _entityIdManager;

        public TrimCollectionNameFromIdConverter(IEntityIdManager entityIdManager)
        {
            this._entityIdManager = entityIdManager;
        }

        public string Convert(string sourceMember, ResolutionContext context)
        {
            return this._entityIdManager.TrimCollectionNameFromId<T>(sourceMember);
        }
    }
}