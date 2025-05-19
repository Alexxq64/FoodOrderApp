using System.Windows.Forms;
using FoodOrderApp.Helpers;  // Для Session
using FoodOrderApp.Services; // Для NavigationService

namespace FoodOrderApp.Forms
{
    public class MainForm : Form
    {
        private NavigationService _navigation;

        public MainForm()
        {
            this.Text = "Главное меню";
            this.Width = 800;
            this.Height = 600;

            var user = Session.CurrentUser;
            if (user != null)
            {
                this.Text = $"Главное меню — {user.Names} ({user.Role})";

                var welcomeLabel = new Label
                {
                    Text = $"Добро пожаловать, {user.Names}!",
                    Left = 20,
                    Top = 20,
                    AutoSize = true
                };
                this.Controls.Add(welcomeLabel);

                _navigation = new NavigationService(this);

                var btnMenu = new Button
                {
                    Text = "Меню",
                    Left = 20,
                    Top = 60,
                    Width = 100
                };
                btnMenu.Click += (s, e) => _navigation.NavigateToMenu();
                this.Controls.Add(btnMenu);

                var btnOrders = new Button
                {
                    Text = "Заказы",
                    Left = 140,
                    Top = 60,
                    Width = 100
                };
                btnOrders.Click += (s, e) => _navigation.NavigateToOrders();
                this.Controls.Add(btnOrders);

                var btnPayment = new Button
                {
                    Text = "Оплата",
                    Left = 260,
                    Top = 60,
                    Width = 100
                };
                btnPayment.Click += (s, e) => _navigation.NavigateToPayment();
                this.Controls.Add(btnPayment);

                var btnStatistics = new Button
                {
                    Text = "Статистика",
                    Left = 380,
                    Top = 60,
                    Width = 100
                };
                btnStatistics.Click += (s, e) => _navigation.NavigateToStatistics();
                this.Controls.Add(btnStatistics);
            }
            else
            {
                this.Text = "Главное меню — пользователь не найден";
            }
        }
    }
}
