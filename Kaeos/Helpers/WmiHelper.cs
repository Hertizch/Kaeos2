using System.Linq;
using System.Management;

namespace Kaeos.Helpers
{
    public static class WmiHelper
    {
        public static string GetValueFromWmi(string scope, string queryString, string objectName)
        {
            string results = null;

            using (var searcher = new ManagementObjectSearcher(scope, queryString))
            {
                foreach (var queryObj in searcher.Get().Cast<ManagementObject>())
                {
                    results = queryObj[objectName].ToString().Trim();
                    break;
                }
            }

            return results;
        }
    }
}
