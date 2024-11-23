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
        private static readonly Regex decimalRegex = new Regex(@"^\d+(\.\d{1,2})?$");
        private static readonly Regex positiveIntegerRegex = new Regex(@"^\d+$");
        private static readonly Regex nameRegex = new Regex(@"^[a-zA-Zа-яА-Я0-9\s-]+$");

        public const int MaxNameLength = 50;
        public const decimal MinAmount = 0.01m;
        public const decimal MaxAmount = 100000000m;
        public const int MinTermMonths = 1;
        public const int MaxTermMonths = 120;


        public static bool IsValidDecimal(string input, decimal min = 0, decimal max = decimal.MaxValue)
        {
            if (!decimalRegex.IsMatch(input)) return false;
            return decimal.TryParse(input, out decimal value) && value >= min && value <= max;
        }

        public static bool IsValidPositiveInteger(string input, int min = 1, int max = int.MaxValue)
        {
            if (!positiveIntegerRegex.IsMatch(input)) return false;
            return int.TryParse(input, out int value) && value >= min && value <= max;
        }

        public static bool IsValidName(string input)
        {
            return nameRegex.IsMatch(input) && input.Length <= MaxNameLength;
        }

        public static bool IsValidDate(DateTime? date)
        {
            return date.HasValue && date.Value >= DateTime.Today;
        }

        public static string ValidateField(string input, string fieldName, Func<string, bool> validationFunc, int maxLength = 50, decimal minValue = 0, decimal maxValue = decimal.MaxValue)
        {
            if (string.IsNullOrEmpty(input)) return $"Поле {fieldName} не может быть пустым.";
            if (input.Length > maxLength) return $"Поле {fieldName} не должно превышать {maxLength} символов.";
            if (!validationFunc(input)) return $"Неверный формат поля {fieldName}.";

            if (decimal.TryParse(input, out decimal value))
            {
                if (value < minValue) return $"Значение поля {fieldName} должно быть не меньше {minValue}.";
                if (value > maxValue) return $"Значение поля {fieldName} должно быть не больше {maxValue}.";
            }

            return null;
        }

        public static string ValidateAmount(string amountStr)
        {
            return ValidateField(amountStr, "Сумма вклада", s => IsValidDecimal(s, MinAmount, MaxAmount));
        }

        public static string ValidateTerm(string termStr)
        {
            return ValidateField(termStr, "Срок вклада", s => IsValidPositiveInteger(s, MinTermMonths, MaxTermMonths));
        }
    }
}