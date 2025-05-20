using System;
using System.Windows.Forms;
using FoodOrderApp.Helpers;
using FoodOrderApp.Services;

namespace FoodOrderApp.Forms
{
    public class AdminForm : Form
    {
        private Label welcomeLabel;
        private Button btnManageUsers;
        private Button btnManageMenu;
        private Button btnManageOrders;    // Добавлена кнопка управления заказами
        private Button btnStatistics;
        private NavigationService _navigation;

        public AdminForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"Админ-панель — {Session.CurrentUser.Names}";
            this.Width = 800;
            this.Height = 600;

            _navigation = new NavigationService(this);

            welcomeLabel = new Label
            {
                Text = $"Добро пожаловать, админ {Session.CurrentUser.Names}!",
                Left = 20,
                Top = 20,
                AutoSize = true
            };

            btnManageUsers = new Button
            {
                Text = "Управление пользователями",
                Left = 20,
                Top = 60,
                Width = 200
            };
            btnManageUsers.Click += BtnManageUsers_Click;

            btnManageMenu = new Button
            {
                Text = "Управление меню",
                Left = 20,
                Top = 100,
                Width = 200
            };
            btnManageMenu.Click += BtnManageMenu_Click;

            btnManageOrders = new Button
            {
                Text = "Управление заказами",
                Left = 20,
                Top = 140,
                Width = 200
            };
            btnManageOrders.Click += BtnManageOrders_Click;

            btnStatistics = new Button
            {
                Text = "Статистика",
                Left = 20,
                Top = 180,
                Width = 200
            };
            btnStatistics.Click += (s, e) => _navigation.NavigateToStatistics();

            this.Controls.Add(welcomeLabel);
            this.Controls.Add(btnManageUsers);
            this.Controls.Add(btnManageMenu);
            this.Controls.Add(btnManageOrders);
            this.Controls.Add(btnStatistics);
        }

        private void BtnManageUsers_Click(object sender, EventArgs e)
        {
            var form = new UserManagementForm();
            form.ShowDialog();
        }

        private void BtnManageMenu_Click(object sender, EventArgs e)
        {
            var menuForm = new MenuManagementForm();
            menuForm.ShowDialog();
        }

        private void BtnManageOrders_Click(object sender, EventArgs e)
        {
            var ordersForm = new OrderManagementForm();
            ordersForm.ShowDialog();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Здесь можно подгрузить данные, если нужно
        }
    }
}
