using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using FoodOrderApp.Data;
using FoodOrderApp.Models;

public class MenuManagementForm : Form
{
    private DataGridView dgvMenu;
    private Button btnDelete;
    private Button btnSave;
    private DbManager db;
    private List<Category> categories;
    private List<Item> items;

    public MenuManagementForm()
    {
        db = new DbManager();
        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        this.Text = "Управление меню";
        this.Width = 900;
        this.Height = 600;

        dgvMenu = new DataGridView()
        {
            Left = 20,
            Top = 20,
            Width = 840,
            Height = 480,
            AllowUserToAddRows = true,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        btnDelete = new Button()
        {
            Text = "Удалить выбранное",
            Left = 20,
            Top = 510,
            Width = 150
        };

        btnSave = new Button()
        {
            Text = "Сохранить изменения",
            Left = 200,
            Top = 510,
            Width = 150
        };

        btnDelete.Click += BtnDelete_Click;
        btnSave.Click += BtnSave_Click;

        this.Controls.Add(dgvMenu);
        this.Controls.Add(btnDelete);
        this.Controls.Add(btnSave);

        dgvMenu.EditingControlShowing += DgvMenu_EditingControlShowing;
        dgvMenu.CellValueChanged += DgvMenu_CellValueChanged;
        dgvMenu.DataError += (s, e) => { e.ThrowException = false; };
    }

    private void LoadData()
    {
        categories = db.GetAllCategories();
        items = db.GetAllItems();

        var dt = new DataTable();

        dt.Columns.Add("Id", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        dt.Columns.Add("Description", typeof(string));
        dt.Columns.Add("Price", typeof(decimal));
        dt.Columns.Add("CategoryId", typeof(int));

        foreach (var item in items)
        {
            dt.Rows.Add(item.Id, item.Name, item.Description, item.Price, item.CategoryId);
        }

        dgvMenu.DataSource = dt;

        dgvMenu.Columns["Id"].ReadOnly = true;

        // Удаляем CategoryId колонку и заменяем на ComboBoxColumn
        int categoryIndex = dgvMenu.Columns["CategoryId"].Index;
        dgvMenu.Columns.Remove("CategoryId");

        var comboColumn = new DataGridViewComboBoxColumn
        {
            Name = "CategoryId",
            HeaderText = "Категория",
            DataPropertyName = "CategoryId",
            DataSource = GetCategoryBindingSource(),
            DisplayMember = "Name",
            ValueMember = "Id",
            DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        };

        dgvMenu.Columns.Insert(categoryIndex, comboColumn);
    }

    private BindingSource GetCategoryBindingSource()
    {
        var catList = new List<Category>(categories);
        catList.Add(new Category { Id = -1, Name = "<Добавить новую категорию>" });
        return new BindingSource(catList, null);
    }

    private void DgvMenu_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
        if (dgvMenu.CurrentCell.OwningColumn.Name == "CategoryId" && e.Control is ComboBox combo)
        {
            combo.SelectedIndexChanged -= Combo_SelectedIndexChanged;
            combo.SelectedIndexChanged += Combo_SelectedIndexChanged;
        }
    }

    private void Combo_SelectedIndexChanged(object sender, EventArgs e)
    {
        var combo = sender as ComboBox;
        if (combo?.SelectedItem is Category selected && selected.Id == -1)
        {
            var inputForm = new CategoryInputForm();
            if (inputForm.ShowDialog() == DialogResult.OK)
            {
                string newName = inputForm.CategoryName?.Trim();

                if (!string.IsNullOrEmpty(newName) && !categories.Any(c => c.Name.Equals(newName, StringComparison.OrdinalIgnoreCase)))
                {
                    db.AddCategory(newName);
                    categories = db.GetAllCategories();

                    // Обновляем источник данных ComboBox'а
                    var catCol = (DataGridViewComboBoxColumn)dgvMenu.Columns["CategoryId"];
                    catCol.DataSource = GetCategoryBindingSource();

                    dgvMenu.CurrentCell.Value = categories.First(c => c.Name == newName).Id;
                }
                else
                {
                    MessageBox.Show("Категория пустая или уже существует.", "Ошибка");
                    dgvMenu.CurrentCell.Value = DBNull.Value;
                }
            }
            else
            {
                dgvMenu.CurrentCell.Value = DBNull.Value;
            }
        }
    }

    private void DgvMenu_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
        // Дополнительная логика по изменению ячеек (по желанию)
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        if (dgvMenu.SelectedRows.Count == 0)
        {
            MessageBox.Show("Выберите строку для удаления.");
            return;
        }

        var row = dgvMenu.SelectedRows[0];
        if (row.Cells["Id"].Value == DBNull.Value || Convert.ToInt32(row.Cells["Id"].Value) == 0)
        {
            dgvMenu.Rows.Remove(row);
            return;
        }

        int id = Convert.ToInt32(row.Cells["Id"].Value);
        var confirm = MessageBox.Show("Удалить выбранный пункт меню?", "Подтверждение", MessageBoxButtons.YesNo);
        if (confirm == DialogResult.Yes)
        {
            db.DeleteItem(id);
            dgvMenu.Rows.Remove(row);
        }
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        foreach (DataGridViewRow row in dgvMenu.Rows)
        {
            if (row.IsNewRow) continue;

            int id = row.Cells["Id"].Value == DBNull.Value ? 0 : Convert.ToInt32(row.Cells["Id"].Value);
            string name = Convert.ToString(row.Cells["Name"].Value)?.Trim();
            string desc = Convert.ToString(row.Cells["Description"].Value)?.Trim();
            decimal price = decimal.TryParse(Convert.ToString(row.Cells["Price"].Value), out var p) ? p : 0;
            int catId = row.Cells["CategoryId"].Value == DBNull.Value ? 0 : Convert.ToInt32(row.Cells["CategoryId"].Value);

            if (string.IsNullOrEmpty(name) || catId <= 0)
                continue;

            var item = new Item
            {
                Id = id,
                Name = name,
                Description = desc,
                Price = price,
                CategoryId = catId,
                ImagePath = ""
            };

            if (id == 0)
                db.AddItem(item);
            else
                db.UpdateItem(item);
        }

        MessageBox.Show("Изменения сохранены.");
        LoadData();
    }
}
