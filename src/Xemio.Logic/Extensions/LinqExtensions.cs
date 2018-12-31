using System;
using System.Collections.Generic;
using System.Linq;

namespace Xemio.Logic.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> self, Func<T, IEnumerable<T>> selector)
        {
            return self.SelectMany(c => selector(c).Flatten(selector)).Concat(self);
        }
    }
}