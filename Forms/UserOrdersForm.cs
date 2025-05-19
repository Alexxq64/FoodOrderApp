using System;
using System.Windows.Forms;
using FoodOrderApp.Services;
using FoodOrderApp.Helpers;
using FoodOrderApp.Models;
using System.Collections.Generic;

namespace FoodOrderApp.Forms
{
    public class UserOrdersForm : Form
    {
        private ListBox ordersListBox;
        private Button btnBack;

        public UserOrdersForm()
        {
            this.Text = "Мои заказы";
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;

            ordersListBox = new ListBox
            {
                Top = 20,
                Left = 20,
                Width = 740,
                Height = 480
            };

            btnBack = new Button
            {
                Text = "Назад",
                Top = 520,
                Left = 20,
                Width = 100
            };
            btnBack.Click += (s, e) => this.Close();

            this.Controls.Add(ordersListBox);
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

            var orders = OrderService.GetUserOrders(user.Id);

            if (orders.Count == 0)
            {
                ordersListBox.Items.Add("Заказов не найдено.");
                ordersListBox.Enabled = false;
                return;
            }

            ordersListBox.Enabled = true;

            // Добавляем в ListBox строки с информацией о заказах
            foreach (var order in orders)
            {
                // Формируем строку, например: "№1 от 19.05.2025 - Статус: Ожидает - Сумма: 1200₽"
                string displayText = $"№{order.Id} от {order.OrderDateTime.ToString("dd.MM.yyyy HH:mm")} - Статус: {order.Status} - Сумма: {order.TotalPrice}₽";
                ordersListBox.Items.Add(displayText);
            }
        }
    }
}
