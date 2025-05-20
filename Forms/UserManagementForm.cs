using System;
using System.Windows.Forms;
using System.Data;
using FoodOrderApp.Data;

namespace FoodOrderApp.Forms
{
    public class UserManagementForm : Form
    {
        private DataGridView usersGrid;
        private Button btnAdd, btnEdit, btnDelete;
        private DbManager db;

        public UserManagementForm()
        {
            db = new DbManager();
            InitializeComponent();
            LoadUsers();
        }

        private void InitializeComponent()
        {
            this.Text = "Управление пользователями";
            this.Width = 800;
            this.Height = 500;

            usersGrid = new DataGridView
            {
                Left = 20,
                Top = 20,
                Width = 740,
                Height = 350,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            btnAdd = new Button
            {
                Text = "Добавить",
                Left = 20,
                Top = 390,
                Width = 100
            };
            btnAdd.Click += BtnAdd_Click;

            btnEdit = new Button
            {
                Text = "Редактировать",
                Left = 140,
                Top = 390,
                Width = 100
            };
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button
            {
                Text = "Удалить",
                Left = 260,
                Top = 390,
                Width = 100
            };
            btnDelete.Click += BtnDelete_Click;

            this.Controls.Add(usersGrid);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnDelete);
        }

        private void LoadUsers()
        {
            var dt = db.GetAllUsers(); // метод добавим ниже
            usersGrid.DataSource = dt;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Форма добавления пользователя (в разработке)");
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (usersGrid.SelectedRows.Count == 0)
                return;

            var userId = usersGrid.SelectedRows[0].Cells["Id"].Value;
            MessageBox.Show($"Редактирование пользователя ID: {userId} (в разработке)");
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (usersGrid.SelectedRows.Count == 0)
                return;

            var userId = usersGrid.SelectedRows[0].Cells["Id"].Value;
            var result = MessageBox.Show("Удалить пользователя?", "Подтверждение", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                db.DeleteUser(Convert.ToInt32(userId)); // метод добавим ниже
                LoadUsers();
            }
        }
    }
}
