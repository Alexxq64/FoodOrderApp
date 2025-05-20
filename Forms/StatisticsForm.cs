using System;
using System.Windows.Forms;
using FoodOrderApp.Helpers;
using FoodOrderApp.Data;  // добавлено

namespace FoodOrderApp.Forms
{
    public class StatisticsForm : Form
    {
        private Label titleLabel;
        private TextBox statsTextBox;

        public StatisticsForm()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void InitializeComponent()
        {
            this.Text = "Статистика";
            this.Width = 600;
            this.Height = 400;

            titleLabel = new Label()
            {
                Text = "Статистика заказов и платежей",
                Left = 20,
                Top = 20,
                AutoSize = true,
                Font = new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold)
            };

            statsTextBox = new TextBox()
            {
                Left = 20,
                Top = 60,
                Width = 540,
                Height = 280,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(statsTextBox);
        }

        private void LoadStatistics()
        {
            if (Session.CurrentUser?.Role != Models.UserRole.admin)
            {
                MessageBox.Show("Доступ запрещён", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            var db = new DbManager();
            string stats = db.GetStatisticsSummary();
            statsTextBox.Text = stats;
        }
    }
}
