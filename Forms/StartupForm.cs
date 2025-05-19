using FoodOrderApp.Data;
using FoodOrderApp.Forms;
using System;
using System.Windows.Forms;

namespace FoodOrderApp.Forms
{
    public class StartupForm : Form
    {
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        public StartupForm()
        {
            this.Text = "Food Order App";
            this.Width = 800;
            this.Height = 600;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel
            {
                Text = "Готов"
            };
            statusStrip.Items.Add(statusLabel);
            statusStrip.Dock = DockStyle.Bottom;

            this.Controls.Add(statusStrip);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                var dbManager = new DbManager();
                if (!dbManager.CheckConnection())
                {
                    MessageBox.Show("Не удалось подключиться к базе данных.",
                                    "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                    return;
                }

                SetStatus("Подключение успешно.");

                // Проверка и предложение заполнить тестовыми данными
                DatabaseSeeder.CheckAndPromptDatabaseSeed();

                // Показать окно логина (модально)
                var loginForm = new LoginForm();
                var result = loginForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    // После логина открыть главное окно
                    var mainForm = new MainForm(); // или MainForm
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    // Пользователь закрыл или отказался от входа
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при инициализации:\n" + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        public void SetStatus(string message)
        {
            statusLabel.Text = message;
        }
    }
}
