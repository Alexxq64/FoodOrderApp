using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FoodOrderApp.Services;
using FoodOrderApp.Helpers;
using FoodOrderApp.Models;

namespace FoodOrderApp.Forms
{
    public class UserOrdersForm : Form
    {
        private ListBox ordersListBox;
        private Button btnBack;
        private Button btnPay;
        private ComboBox paymentMethodComboBox;

        private List<Order> currentOrders;

        public UserOrdersForm()
        {
            this.Text = "Мои заказы";
            this.Width = 800;
            this.Height = 650;
            this.StartPosition = FormStartPosition.CenterScreen;

            ordersListBox = new ListBox
            {
                Top = 20,
                Left = 20,
                Width = 740,
                Height = 440
            };

            paymentMethodComboBox = new ComboBox
            {
                Top = 470,
                Left = 20,
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            paymentMethodComboBox.Items.AddRange(Enum.GetNames(typeof(PaymentType)));
            paymentMethodComboBox.SelectedIndex = 0;

            btnPay = new Button
            {
                Text = "Оплатить",
                Top = 470,
                Left = 240,
                Width = 100
            };
            btnPay.Click += BtnPay_Click;

            btnBack = new Button
            {
                Text = "Назад",
                Top = 520,
                Left = 20,
                Width = 100
            };
            btnBack.Click += (s, e) => this.Close();

            this.Controls.Add(ordersListBox);
            this.Controls.Add(paymentMethodComboBox);
            this.Controls.Add(btnPay);
            this.Controls.Add(btnBack);

            LoadOrders();
        }

        private void LoadOrders()
        {
            var user = Session.CurrentUser;
            if (user == null)
            {
                MessageBox.Show("Пользователь не авторизован");
                return;
            }

            ordersListBox.Items.Clear();

            currentOrders = OrderService.GetUserOrders(user.Id);

            if (currentOrders.Count == 0)
            {
                ordersListBox.Items.Add("Заказов не найдено.");
                ordersListBox.Enabled = false;
                btnPay.Enabled = false;
                paymentMethodComboBox.Enabled = false;
                return;
            }

            ordersListBox.Enabled = true;
            btnPay.Enabled = true;
            paymentMethodComboBox.Enabled = true;

            foreach (var order in currentOrders)
            {
                bool isPaid = PaymentService.IsOrderPaid(order.Id);
                string statusText = isPaid ? "Оплачен" : "Ожидает оплаты";

                string displayText = $"№{order.Id} от {order.OrderDateTime:dd.MM.yyyy HH:mm} - Статус: {order.Status} - Сумма: {order.TotalPrice}₽";

                if (!isPaid)
                    displayText += " [ОПЛАТИТЬ]";

                ordersListBox.Items.Add(displayText);
            }
        }

        private void BtnPay_Click(object sender, EventArgs e)
        {
            int index = ordersListBox.SelectedIndex;
            if (index < 0 || currentOrders == null || index >= currentOrders.Count)
            {
                MessageBox.Show("Выберите заказ для оплаты.");
                return;
            }

            var selectedOrder = currentOrders[index];

            // Проверка: уже оплачен?
            if (PaymentService.IsOrderPaid(selectedOrder.Id))
            {
                MessageBox.Show("Этот заказ уже оплачен.");
                return;
            }

            var paymentTypeStr = paymentMethodComboBox.SelectedItem.ToString();
            Enum.TryParse(paymentTypeStr, out PaymentType paymentType);

            var confirm = MessageBox.Show(
                $"Вы уверены, что хотите оплатить заказ №{selectedOrder.Id} на сумму {selectedOrder.TotalPrice}₽ методом: {paymentType}?",
                "Подтверждение оплаты",
                MessageBoxButtons.YesNo
            );

            if (confirm == DialogResult.Yes)
            {
                PaymentService.ProcessPayment(selectedOrder.Id, selectedOrder.TotalPrice, paymentType);
                MessageBox.Show("Оплата прошла успешно!");
                LoadOrders(); // Обновляем список
            }
        }
    }
}
