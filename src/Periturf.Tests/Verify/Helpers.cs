using System.Collections.Generic;

namespace Periturf.Tests.Verify
{
    static class Helpers
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<T> enumerable)
        {
            foreach (var item in enumerable)
                yield return item;
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
