using System.Windows.Forms;

namespace FoodOrderApp.Forms
{
    public class MenuForm : Form
    {
        public MenuForm()
        {
            // Настройки формы
            this.Text = "Меню";
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Метка-заглушка
            var label = new Label
            {
                Text = "Это форма меню (MenuForm)",
                AutoSize = true,
                Left = 20,
                Top = 20
            };

            this.Controls.Add(label);
        }
    }
}
