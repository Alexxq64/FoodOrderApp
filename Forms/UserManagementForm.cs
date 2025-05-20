using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using FoodOrderApp.Data;
using FoodOrderApp.Models;

namespace FoodOrderApp.Forms
{
    public class UserManagementForm : Form
    {
        private DataGridView usersGrid;
        private Button btnDelete;
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
                Height = 380,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            usersGrid.CellEndEdit += UsersGrid_CellEndEdit;
            usersGrid.RowValidating += UsersGrid_RowValidating;

            btnDelete = new Button
            {
                Text = "Удалить",
                Left = 20,
                Top = 420,
                Width = 100
            };
            btnDelete.Click += BtnDelete_Click;

            this.Controls.Add(usersGrid);
            this.Controls.Add(btnDelete);
        }

        private void LoadUsers()
        {
            var users = db.GetAllUsers();
            var table = new DataTable();

            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Names", typeof(string));
            table.Columns.Add("Login", typeof(string));
            table.Columns.Add("Role", typeof(string));
            table.Columns.Add("Password", typeof(string));

            foreach (var user in users)
            {
                table.Rows.Add(user.Id, user.Names, user.Login, user.Role.ToString(), user.Password);
            }

            usersGrid.DataSource = table;
            usersGrid.Columns["Id"].ReadOnly = true; // ID не редактируем
        }

        private void UsersGrid_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            var row = usersGrid.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            int id = row.Cells["Id"].Value is int val ? val : 0;
            string names = row.Cells["Names"].Value?.ToString();
            string login = row.Cells["Login"].Value?.ToString();
            string roleStr = row.Cells["Role"].Value?.ToString();
            string password = row.Cells["Password"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(names) ||
                string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(roleStr) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Все поля (включая пароль) должны быть заполнены.");
                e.Cancel = true;
                return;
            }

            if (!Enum.TryParse(roleStr, true, out UserRole parsedRole))
            {
                MessageBox.Show("Роль должна быть admin или user.");
                e.Cancel = true;
                return;
            }

            if (id == 0)
            {
                try
                {
                    // Добавляем в базу
                    db.AddUser(new User
                    {
                        Names = names,
                        Login = login,
                        Role = parsedRole,
                        Password = password
                    });

                    // Получаем ID из базы (опционально)
                    var newUser = db.GetUserByLogin(login);
                    row.Cells["Id"].Value = newUser.Id; // обновляем ID в таблице

                }
                catch (MySql.Data.MySqlClient.MySqlException ex) when (ex.Number == 1062)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует.");
                    e.Cancel = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка добавления пользователя: " + ex.Message);
                    e.Cancel = true;
                }
            }
        }


        private void UsersGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var row = usersGrid.Rows[e.RowIndex];
            if (row.IsNewRow) return;

            object idObj = row.Cells["Id"].Value;
            int id = (idObj != null && idObj != DBNull.Value) ? Convert.ToInt32(idObj) : 0;

            string names = row.Cells["Names"].Value?.ToString();
            string login = row.Cells["Login"].Value?.ToString();
            string roleStr = row.Cells["Role"].Value?.ToString();
            string password = row.Cells["Password"].Value?.ToString();

            if (string.IsNullOrWhiteSpace(names) ||
                string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(roleStr) ||
                string.IsNullOrWhiteSpace(password))
                return;

            if (!Enum.TryParse(roleStr, true, out UserRole parsedRole))
                return;

            if (id > 0)
            {
                db.UpdateUser(new User
                {
                    Id = id,
                    Names = names,
                    Login = login,
                    Role = parsedRole,
                    Password = password
                });
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (usersGrid.SelectedRows.Count == 0)
                return;

            var row = usersGrid.SelectedRows[0];
            if (row.IsNewRow || row.Cells["Id"].Value == null)
                return;

            var userId = Convert.ToInt32(row.Cells["Id"].Value);
            var result = MessageBox.Show("Удалить пользователя?", "Подтверждение", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                db.DeleteUser(userId);
                LoadUsers();
            }
        }
    }
}
