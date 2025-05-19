using System;
using System.Windows.Forms;
using FoodOrderApp.Services;

namespace FoodOrderApp.Forms
{
    public partial class RegisterForm : Form
    {
        private readonly AuthService _authService;

        private TextBox txtName;
        private TextBox txtLogin;
        private TextBox txtPassword;
        private Button btnRegister;

        public RegisterForm()
        {
            InitializeComponent();
            _authService = new AuthService();
        }

        private void InitializeComponent()
        {
            this.Text = "Регистрация";
            this.Width = 350;
            this.Height = 250;

            txtName = new TextBox { Left = 120, Top = 20, Width = 180 };
            txtLogin = new TextBox { Left = 120, Top = 60, Width = 180 };
            txtPassword = new TextBox { Left = 120, Top = 100, Width = 180, PasswordChar = '*' };

            btnRegister = new Button { Text = "Зарегистрироваться", Left = 120, Top = 140, Width = 180 };

            btnRegister.Click += BtnRegister_Click;

            this.Controls.Add(new Label { Text = "Имя:", Left = 20, Top = 20, AutoSize = true });
            this.Controls.Add(txtName);
            this.Controls.Add(new Label { Text = "Логин:", Left = 20, Top = 60, AutoSize = true });
            this.Controls.Add(txtLogin);
            this.Controls.Add(new Label { Text = "Пароль:", Left = 20, Top = 100, AutoSize = true });
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnRegister);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_authService.Register(name, login, password, out string error))
            {
                MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close(); // Закрыть форму регистрации
            }
            else
            {
                MessageBox.Show($"Ошибка регистрации: {error}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
