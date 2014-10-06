using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nigrimmist.Modules.Helpers
{
    public static class NigrimmistUtilityExtensions
    {
        public static string RemoveAllTags(this string content)
        {
           return Regex.Replace(content, @"<[^>]*>", String.Empty);
        }
    }
}
