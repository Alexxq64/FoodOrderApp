using FoodOrderApp.Data;
using System;
using System.Windows.Forms;

namespace FoodOrderApp.Forms
{
    public class MainForm : Form
    {
        private Button helloButton;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;

        public MainForm()
        {
            this.Text = "Food Order App";
            this.Width = 800;
            this.Height = 600;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            helloButton = new Button
            {
                Text = "Показать таблицы",
                Left = 100,
                Top = 100,
                Width = 150,
            };

            // Обработчик кнопки вызывает метод получения таблиц и выводит их
            helloButton.Click += (sender, e) =>
            {
                try
                {
                    var dbManager = new DbManager();
                    var tables = dbManager.GetTables();

                    if (tables.Count == 0)
                        MessageBox.Show("Таблицы в базе отсутствуют.", "Информация");
                    else
                        MessageBox.Show(string.Join(Environment.NewLine, tables), "Список таблиц");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении таблиц:\n{ex.Message}", "Ошибка");
                }
            };

            // Создаем статус-бар и метку статуса
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel
            {
                Text = "Готов"
            };
            statusStrip.Items.Add(statusLabel);
            statusStrip.Dock = DockStyle.Bottom;

            // Добавляем контролы на форму
            this.Controls.Add(helloButton);
            this.Controls.Add(statusStrip);
        }

        // Метод для обновления текста в статус-баре
        public void SetStatus(string message)
        {
            statusLabel.Text = message;
        }
    }
}
