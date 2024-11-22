using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Deposit_Iwanov_Egor
{
    public partial class MainWindow : Window
    {
        private List<Deposit> deposits = new List<Deposit>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddDeposit(object sender, RoutedEventArgs e)
        {
            string name = DepositNameTextBox.Text;
            if (decimal.TryParse(DepositAmountTextBox.Text, out decimal amount) &&
                decimal.TryParse(DepositInterestRateTextBox.Text, out decimal interestRate) &&
                DateTime.TryParse(DepositExpiryDateTextBox.Text, out DateTime expirationDate))
            {
                deposits.Add(new Deposit
                {
                    Name = name,
                    Amount = amount,
                    InterestRate = interestRate,
                    ExpirationDate = expirationDate
                });
                ResultTextBlock.Text = "Вклад добавлен!";
                ClearInputFields();
            }
            else
            {
                ResultTextBlock.Text = "Введите корректные данные!";
            }
        }

        private void ApplyInterests(object sender, RoutedEventArgs e)
        {
            foreach (var deposit in deposits)
            {
                deposit.ApplyInterest();
            }
            ResultTextBlock.Text = "Проценты начислены!";
        }

        private void ShowDeposits(object sender, RoutedEventArgs e)
        {
            DepositsListBox.Items.Clear();
            foreach (var deposit in deposits)
            {
                var displayText = $"{deposit.Name} - Сумма: {deposit.Amount}, Процент: {deposit.InterestRate}, Дата истечения: {deposit.ExpirationDate.ToShortDateString()}";

                if (deposit.IsExpired)
                {
                    DepositsListBox.Items.Add(new ListBoxItem
                    {
                        Content = displayText,
                        Background = Brushes.Red
                    });
                }
                else if (deposit.IsExpiringSoon)
                {
                    DepositsListBox.Items.Add(new ListBoxItem
                    {
                        Content = displayText,
                        Background = Brushes.Yellow
                    });
                }
                else
                {
                    DepositsListBox.Items.Add(new ListBoxItem
                    {
                        Content = displayText,
                        Background = Brushes.Green
                    });
                }
            }
        }

        private void ClearInputFields()
        {
            DepositNameTextBox.Clear();
            DepositAmountTextBox.Clear();
            DepositInterestRateTextBox.Clear();
            DepositExpiryDateTextBox.Clear();
        }
    }
}