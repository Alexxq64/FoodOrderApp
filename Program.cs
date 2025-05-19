using System;
using System.Windows.Forms;
using FoodOrderApp.Data;
using FoodOrderApp.Forms;
using FoodOrderApp.Helpers;

namespace FoodOrderApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var db = new DbManager();
            if (!db.CheckConnection())
            {
                MessageBox.Show("Ошибка подключения к базе данных. Приложение будет закрыто.",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DatabaseSeeder.CheckAndPromptDatabaseSeed();

            var loginForm = new LoginForm();
            var result = loginForm.ShowDialog();

            if (result != DialogResult.OK || Session.CurrentUser == null)
            {
                return;
            }

            Form mainForm;

            if (Session.CurrentUser.Role == Models.UserRole.admin)
            {
                mainForm = new AdminForm();
            }
            else
            {
                mainForm = new MainForm();
            }

            Application.Run(mainForm);

        }
    }
}
