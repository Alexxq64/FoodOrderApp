using System;
using System.Windows.Forms;
using FoodOrderApp.Services;
using FoodOrderApp.Helpers;

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

            var orders = OrderService.GetUserOrders(user.Id);
            foreach (var order in orders)
            {
                ordersListBox.Items.Add(order);
            }
        }
    }
}
