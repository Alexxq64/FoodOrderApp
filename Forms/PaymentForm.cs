using System;
using System.Windows.Forms;
using FoodOrderApp.Services;

namespace FoodOrderApp.Forms
{
    public class PaymentForm : Form
    {
        private ListBox paymentsListBox;
        private Button backButton;

        public PaymentForm()
        {
            InitializeComponents();
            LoadPayments();
        }

        private void InitializeComponents()
        {
            this.Text = "Мои платежи";
            this.Width = 600;
            this.Height = 500;

            paymentsListBox = new ListBox
            {
                Left = 20,
                Top = 20,
                Width = 540,
                Height = 380
            };

            backButton = new Button
            {
                Text = "Назад",
                Left = 20,
                Top = 420,
                Width = 100
            };
            backButton.Click += BackButton_Click;

            this.Controls.Add(paymentsListBox);
            this.Controls.Add(backButton);
        }

        private void LoadPayments()
        {
            var payments = PaymentService.GetUserPayments();

            paymentsListBox.Items.Clear();

            foreach (var payment in payments)
            {
                paymentsListBox.Items.Add($"#{payment.Id} — {payment.Amount}₽ — {payment.PaidDate:dd.MM.yyyy HH:mm}");
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close(); // Возврат к предыдущей форме
        }
    }
}
