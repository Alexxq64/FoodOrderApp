using System;
using System.Windows.Forms;
using FoodOrderApp.Models;
using FoodOrderApp.Services;
using FoodOrderApp.Helpers;

namespace FoodOrderApp.Forms
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;

        // 👇 ЭТИ поля должны быть частью класса
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;

        public LoginForm()
        {
            InitializeComponent(); // 👈 Вызов метода, который добавит контролы
            _authService = new AuthService();
        }

        private void InitializeComponent()
        {
            this.Text = "Вход";
            this.Width = 320;
            this.Height = 220;

            int labelLeft = 20;
            int controlLeft = 100;
            int verticalSpacing = 40;

            var lblLogin = new Label { Text = "Логин:", Left = labelLeft, Top = 20, AutoSize = true };
            txtLogin = new TextBox { Left = controlLeft, Top = 20, Width = 180 };

            var lblPassword = new Label { Text = "Пароль:", Left = labelLeft, Top = 20 + verticalSpacing, AutoSize = true };
            txtPassword = new TextBox { Left = controlLeft, Top = 20 + verticalSpacing, Width = 180, PasswordChar = '*' };

            btnLogin = new Button { Text = "Войти", Left = controlLeft, Top = 20 + 2 * verticalSpacing + 10, Width = 80 };
            btnRegister = new Button { Text = "Регистрация", Left = controlLeft + 90, Top = 20 + 2 * verticalSpacing + 10, Width = 100 };

            btnLogin.Click += btnLogin_Click;
            btnRegister.Click += btnRegister_Click;

            this.Controls.Add(lblLogin);
            this.Controls.Add(txtLogin);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnRegister);
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            User user = _authService.Login(login, password);
            if (user != null)
            {
                MessageBox.Show($"Добро пожаловать, {user.Names}!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Сохраняем пользователя в сессии
                Session.CurrentUser = user;

                Session.CurrentUser = user;
                this.DialogResult = DialogResult.OK; // Закрываем форму и возвращаем OK
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnRegister_Click(object sender, EventArgs e)
        {
            var registerForm = new RegisterForm();
            registerForm.ShowDialog();
        }
    }
}
