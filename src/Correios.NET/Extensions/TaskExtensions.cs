using Correios.NET.Helpers;
using System.Threading.Tasks;

namespace Correios.NET.Extensions
{
    public static class TaskExtensions
    {
        public static void RunSync(this Task task)
        {
            AsyncHelpers.RunSync(() => task);
        }

        public static T RunSync<T>(this Task<T> task)
        {
            return AsyncHelpers.RunSync(() => task);
        }
    }
}
