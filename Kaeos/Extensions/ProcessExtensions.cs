using System.Diagnostics;
using System.Linq;

namespace Kaeos.Extensions
{
    public static class ProcessExtensions
    {
        public static bool ProcessExists(int id)
        {
            return Process.GetProcesses().Any(x => x.Id == id);
        }
    }
}
