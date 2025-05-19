using System;
using System.Windows.Forms;
using FoodOrderApp.Models;
using FoodOrderApp.Services;

namespace FoodOrderApp.Forms
{
    public class AdminOrdersForm : Form
    {
        private ListBox ordersListBox;
        private Button backButton;

        public AdminOrdersForm()
        {
            InitializeComponents();
            LoadOrders();
        }

        private void InitializeComponents()
        {
            this.Text = "Управление заказами (Админ)";
            this.Width = 800;
            this.Height = 600;

            ordersListBox = new ListBox
            {
                Left = 20,
                Top = 20,
                Width = 740,
                Height = 460
            };

            backButton = new Button
            {
                Text = "Назад",
                Left = 20,
                Top = 500,
                Width = 100
            };
            backButton.Click += BackButton_Click;

            this.Controls.Add(ordersListBox);
            this.Controls.Add(backButton);
        }

        private void LoadOrders()
        {
            var orders = OrderService.GetAllOrders();
            ordersListBox.Items.Clear();

            foreach (var order in orders)
            {
                ordersListBox.Items.Add($"Заказ #{order.Id} — {order.Status} — {order.TotalPrice}₽ — {order.DeliveryAddress}");
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close(); // Возврат к предыдущей форме
        }
    }
}
