using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Deposit_Iwanov_Egor
{
    public partial class MainWindow : Window
    {
        private List<Deposit> deposits = new List<Deposit>();

        public MainWindow()
        {
            InitializeComponent();
            InterestRateTypeComboBox.SelectedIndex = 0;
        }

        private readonly Regex decimalRegex = new Regex(@"^\d+(\.\d{1,2})?$");
        private readonly Regex positiveIntegerRegex = new Regex(@"^\d+$");

        private void AddDeposit(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = DepositNameTextBox.Text.Trim();
                string error = Validation.ValidateField(name, "Имя вклада", Validation.IsValidName);
                if (error != null) throw new Exception(error);

                string amountStr = DepositAmountTextBox.Text;
                string errorAmount = Validation.ValidateField(amountStr, "Сумма вклада", s => Validation.IsValidDecimal(s, Validation.MinAmount, Validation.MaxAmount));
                if (errorAmount != null) throw new Exception(errorAmount);
                decimal amount = decimal.Parse(amountStr, CultureInfo.InvariantCulture);

                DateTime expiryDate = DepositExpiryDatePicker.SelectedDate ?? throw new Exception("Выберите дату истечения!");
                if (!Validation.IsValidDate(expiryDate)) throw new Exception("Дата истечения не может быть в прошлом!");

                string termMonthsStr = DepositTermTextBox.Text;
                string errorTerm = Validation.ValidateField(termMonthsStr, "Срок вклада", s => Validation.IsValidPositiveInteger(s, Validation.MinTermMonths, Validation.MaxTermMonths));
                if (errorTerm != null) throw new Exception(errorTerm);
                int termMonths = int.Parse(termMonthsStr);

                decimal interestRate = 0;
                string interestRateType = ((ComboBoxItem)InterestRateTypeComboBox.SelectedItem)?.Content?.ToString() ?? "Вручную";

                if (interestRateType == "Вручную")
                {
                    string interestRateStr = DepositInterestRateTextBox.Text;
                    error = Validation.ValidateField(interestRateStr, "Процентная ставка", s => Validation.IsValidDecimal(s, 0, 1));
                    if (error != null) throw new Exception(error);
                    decimal.TryParse(interestRateStr, NumberStyles.Any, CultureInfo.InvariantCulture, out interestRate);
                }

                string cashbackStr = CashbackTextBox.Text;
                error = Validation.ValidateField(cashbackStr, "Кэшбек", s => Validation.IsValidDecimal(s, 0));
                if (error != null) throw new Exception(error);
                decimal.TryParse(cashbackStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal cashback);

                string nonRemovableBalanceStr = NonRemovableBalanceTextBox.Text;
                string errorNonRemovable = Validation.ValidateField(nonRemovableBalanceStr, "Неснимаемый остаток", s => Validation.IsValidDecimal(s, 0));
                if (errorNonRemovable != null) throw new Exception(errorNonRemovable);
                decimal nonRemovableBalance = decimal.Parse(nonRemovableBalanceStr, NumberStyles.Any, CultureInfo.InvariantCulture);


                Deposit newDeposit = new Deposit(name, amount, interestRate, expiryDate, termMonths, cashback, nonRemovableBalance, interestRateType);
                deposits.Add(newDeposit);
                UpdateListBox();
                ClearFields();
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Ошибка: {ex.Message}";
                ResultTextBlock.Foreground = Brushes.Red;
            }
        }

        private void ApplyInterests(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (Deposit deposit in deposits)
                {
                    if (deposit.ExpiryDate >= DateTime.Today)
                    {
                        deposit.CalculateInterest();
                    }
                }
                UpdateListBox();
            }
            catch (Exception ex)
            {
                ResultTextBlock.Text = $"Ошибка: {ex.Message}";
                ResultTextBlock.Foreground = Brushes.Red;
            }
        }


        private void ShowDeposits(object sender, RoutedEventArgs e)
        {
            UpdateListBox();
            CalculateTotalSum();
        }

        private void UpdateListBox()
        {
            DepositsListBox.Items.Clear();
            foreach (Deposit deposit in deposits.OrderBy(d => d.ExpiryDate))
            {
                ListBoxItem item = CreateDepositListBoxItem(deposit);
                DepositsListBox.Items.Add(item);
            }
        }

        private ListBoxItem CreateDepositListBoxItem(Deposit deposit)
        {
            ListBoxItem item = new ListBoxItem();
            StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            TextBlock depositInfo = new TextBlock
            {
                Text = $"{deposit.Name} - {deposit.Amount:C} (Истекает: {deposit.ExpiryDate.ToShortDateString()})",
                Margin = new Thickness(5)
            };

            Button addFundsButton = new Button { Content = "Добавить", Margin = new Thickness(5) };
            addFundsButton.Click += (sender, e) => AddFunds(deposit);
            Button withdrawFundsButton = new Button { Content = "Снять", Margin = new Thickness(5) };
            withdrawFundsButton.Click += (sender, e) => WithdrawFunds(deposit);

            stackPanel.Children.Add(depositInfo);
            stackPanel.Children.Add(addFundsButton);
            stackPanel.Children.Add(withdrawFundsButton);
            item.Content = stackPanel;


            TimeSpan timeSpan = deposit.ExpiryDate - DateTime.Today;
            if (timeSpan.Days <= 0)
            {
                item.Foreground = Brushes.Red;
            }
            else if (timeSpan.Days <= 5)
            {
                item.Foreground = Brushes.Orange;
            }
            return item;
        }

        private void CalculateTotalSum()
        {
            decimal totalSum = deposits.Sum(d => d.Amount);
            ResultTextBlock.Text = $"Общая сумма вкладов: {totalSum:C}";
        }

        private void AddFunds(Deposit deposit)
        {
            if (InputBox("Добавить средства", "Введите сумму:", out string amountStr))
            {
                if (Validation.IsValidDecimal(amountStr, 0.01m) && decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
                {
                    deposit.Amount += amount;
                    UpdateListBox();
                }
                else
                {
                    MessageBox.Show("Неверная сумма.");
                }
            }
        }


        private void WithdrawFunds(Deposit deposit)
        {
            if (InputBox("Снять средства", "Введите сумму:", out string amountStr))
            {
                if (Validation.IsValidDecimal(amountStr, 0.01m) && decimal.TryParse(amountStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal amount))
                {
                    if (deposit.Amount - amount >= deposit.NonRemovableBalance)
                    {
                        deposit.Amount -= amount;
                        UpdateListBox();
                    }
                    else
                    {
                        MessageBox.Show($"Нельзя снять сумму, ниже порога неснимаемого остатка, который вы указали как - ({deposit.NonRemovableBalance:C}).", "Операция приостановлена", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Неверная сумма.");
                }
            }
        }

        private void ClearFields()
        {
            DepositNameTextBox.Clear();
            DepositAmountTextBox.Clear();
            DepositInterestRateTextBox.Clear();
            DepositExpiryDatePicker.SelectedDate = null;
            DepositTermTextBox.Clear();
            CashbackTextBox.Clear();
            NonRemovableBalanceTextBox.Clear();
            InterestRateTypeComboBox.SelectedIndex = 0;
            ResultTextBlock.Text = "";
            ResultTextBlock.Foreground = Brushes.Black;
        }

        private bool InputBox(string title, string promptText, out string result)
        {
            InputBoxWindow inputBox = new InputBoxWindow(title, promptText);
            bool? dialogResult = inputBox.ShowDialog();
            result = inputBox.Result;
            return dialogResult.HasValue && dialogResult.Value;
        }
    }

    public class InputBoxWindow : Window
    {
        public string Result { get; private set; }

        public InputBoxWindow(string title, string promptText)
        {
            Title = title;
            Content = new InputBoxContent(promptText);
            Width = 300;
            Height = 170;
        }

        public void OnOkButtonClick(object sender, RoutedEventArgs e)
        {
            Result = ((InputBoxContent)Content).TextBox.Text;
            DialogResult = true;
            Close();
        }

        public void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class InputBoxContent : StackPanel
    {
        public TextBox TextBox { get; }

        public InputBoxContent(string promptText)
        {
            Orientation = Orientation.Vertical;
            Margin = new Thickness(10);

            TextBlock prompt = new TextBlock { Text = promptText, Margin = new Thickness(0, 0, 0, 10) };
            TextBox = new TextBox();
            Button okButton = new Button { Content = "OK", Margin = new Thickness(0, 10, 0, 0) };
            Button cancelButton = new Button { Content = "Отмена", Margin = new Thickness(0, 10, 0, 0) };

            okButton.Click += (sender, e) => ((InputBoxWindow)this.Parent).OnOkButtonClick(sender, e);
            cancelButton.Click += (sender, e) => ((InputBoxWindow)this.Parent).OnCancelButtonClick(sender, e);

            Children.Add(prompt);
            Children.Add(TextBox);
            Children.Add(okButton);
            Children.Add(cancelButton);
        }
    }
}