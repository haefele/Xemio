using FluentValidation;

namespace Xemio.Logic.Extensions
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> NoSurroundingWhitespace<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            Guard.NotNull(ruleBuilder, nameof(ruleBuilder));

            return ruleBuilder.Must(f => f?.Trim().Length == f?.Length);
        }
    }
}