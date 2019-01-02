namespace Xemio.Logic.Extensions
{
    public static class StringExtensions
    {
        public static string TrimCollectionNameFromId(this string self)
        {
            Guard.NotNull(self, nameof(self));

            var slashIndex = self.LastIndexOf('/');

            if (slashIndex == -1)
                return self;

            return self.Substring(slashIndex + 1);
        }
    }
}