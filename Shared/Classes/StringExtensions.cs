using System.Text.RegularExpressions;

namespace Tpk.DataServices.Shared.Classes
{
    public static class StringExtensions
    {
        public static string WordWithSpace(this string source)
        {
            var lists = Regex.Split(source, @"(?<!^)(?=[A-Z])");

            return string.Join(" ", lists);
        }
    }
}