using System;
using System.Windows.Forms;
using FoodOrderApp.Helpers;

namespace FoodOrderApp.Forms
{
    public class AdminForm : Form
    {
        private Label welcomeLabel;
        private Button btnManageUsers;
        private Button btnManageMenu;
        private Button btnLogout;

        public AdminForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"Админ-панель — {Session.CurrentUser.Names}";
            this.Width = 800;
            this.Height = 600;

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

            btnLogout = new Button
            {
                Text = "Выход",
                Left = 20,
                Top = 150,
                Width = 200
            };
            btnLogout.Click += BtnLogout_Click;

            this.Controls.Add(welcomeLabel);
            this.Controls.Add(btnManageUsers);
            this.Controls.Add(btnManageMenu);
            this.Controls.Add(btnLogout);
        }

        private void BtnManageUsers_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Здесь будет управление пользователями.", "Управление пользователями");
        }

        private void BtnManageMenu_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Здесь будет управление меню.", "Управление меню");
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            // Очистить сессию и вернуться к логину
            Session.CurrentUser = null;

            var loginForm = new LoginForm();
            this.Hide();

            var result = loginForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                Form nextForm;
                if (Session.CurrentUser.Role == Models.UserRole.admin)
                    nextForm = new AdminForm();
                else
                    nextForm = new MainForm();

                nextForm.Show();
            }

            this.Close(); // Закрываем текущую форму
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // Здесь можно подгрузить данные, если нужно
        }
    }
}
