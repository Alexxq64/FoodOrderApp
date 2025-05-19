using System.Windows.Forms;
using FoodOrderApp.Helpers;  // Для Session

namespace FoodOrderApp.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            var user = Session.CurrentUser;
            if (user != null)
            {
                Text = $"Главное меню — {user.Names} ({user.Role})";

                var welcomeLabel = new Label
                {
                    Text = $"Добро пожаловать, {user.Names}!",
                    Left = 20,
                    Top = 20,
                    AutoSize = true
                };

                this.Controls.Add(welcomeLabel);
            }
            else
            {
                Text = "Главное меню — пользователь не найден";
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Главное меню";
            this.Width = 800;
            this.Height = 600;

            // Контролы будут добавлены в конструкторе
        }
    }
}
