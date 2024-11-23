using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deposit_Iwanov_Egor
{
    public class Deposit
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int TermMonths { get; set; }
        public decimal Cashback { get; set; }
        public decimal NonRemovableBalance { get; set; }
        public string InterestRateType { get; set; }

        public Deposit(string name, decimal amount, decimal interestRate, DateTime expiryDate, int termMonths, decimal cashback, decimal nonRemovableBalance, string interestRateType)
        {
            Name = name;
            Amount = amount;
            InterestRate = interestRate;
            ExpiryDate = expiryDate;
            TermMonths = termMonths;
            Cashback = cashback;
            NonRemovableBalance = nonRemovableBalance;
            InterestRateType = interestRateType;
        }

        public void CalculateInterest()
        {
            if (InterestRateType == "Автоматически")
            {
                InterestRate = (decimal)(TermMonths * 0.005);
                if (InterestRate > 0.15m) InterestRate = 0.15m;
            }
            Amount += Amount * InterestRate;
        }

        public override string ToString()
        {
            return $"Название: {Name}, Сумма: {Amount:C}, Процент: {InterestRate:P}, Дата истечения: {ExpiryDate.ToShortDateString()}, Срок: {TermMonths} мес., Кэшбек: {Cashback:C}, Неснимаемый остаток: {NonRemovableBalance:C}";
        }
    }
}
