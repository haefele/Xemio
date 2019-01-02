using AutoMapper;

namespace Xemio.Logic.AutoMapper.ValueConverter
{
    public class TrimCollectionNameFromIdConverter : IValueConverter<string, string>
    {
        public string Convert(string sourceMember, ResolutionContext context)
        {
            var slashIndex = sourceMember.LastIndexOf('/');

            if (slashIndex == -1)
                return sourceMember;

            return sourceMember.Substring(slashIndex + 1);
        }
    }
}