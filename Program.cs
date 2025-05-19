using FoodOrderApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FoodOrderApp.Forms
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
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
                return; // Выход из программы
            }

            var startupForm = new StartupForm();
            startupForm.SetStatus("Соединение с БД успешно установлено.");

            DatabaseSeeder.CheckAndPromptDatabaseSeed();

            Application.Run(startupForm);
        }
    }
}
