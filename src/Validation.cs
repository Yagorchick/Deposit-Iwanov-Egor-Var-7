using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Deposit_Iwanov_Egor
{
    public static class Validation
    {
        public static bool IsValidName(string name)
        {
            string pattern = @"^[\p{L}\s]+$";
            return Regex.IsMatch(name, pattern);
        }

        public static bool IsValidDecimal(string input)
        {
            string pattern = @"^\d+(\.\d{1,2})?$";
            return Regex.IsMatch(input, pattern);
        }

        public static bool IsValidPercentage(string input)
        {
            string pattern = @"^(100(\.0{1,2})?|[0-9]{1,2}(\.[0-9]{1,2})?)$";
            return Regex.IsMatch(input, pattern);
        }

        public static bool IsValidCashback(string input)
        {
            string pattern = @"^(100(\.0{1,2})?|[0-9]{1,2}(\.[0-9]{1,2})?)$";
            return Regex.IsMatch(input, pattern);
        }
    }
}
