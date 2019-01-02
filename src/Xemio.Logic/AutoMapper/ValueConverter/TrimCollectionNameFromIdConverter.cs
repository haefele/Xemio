using AutoMapper;
using Xemio.Logic.Extensions;

namespace Xemio.Logic.AutoMapper.ValueConverter
{
    public class TrimCollectionNameFromIdConverter : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context)
        {
            return sourceMember.TrimCollectionNameFromId();
        }
    }
}