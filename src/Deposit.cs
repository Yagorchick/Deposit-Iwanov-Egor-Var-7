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
        public DateTime ExpirationDate { get; set; }

        public bool IsExpired => DateTime.Now > ExpirationDate;

        public bool IsExpiringSoon => !IsExpired && (ExpirationDate - DateTime.Now).TotalDays < 5;

        public void AddAmount(decimal amount)
        {
            Amount += amount;
        }

        public bool WithdrawAmount(decimal amount)
        {
            if (amount <= Amount)
            {
                Amount -= amount;
                return true;
            }
            return false;
        }

        public void ApplyInterest()
        {
            if (!IsExpired)
            {
                Amount += Amount * InterestRate / 100;
            }
        }
    }
}
