using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using FoodOrderApp.Data;
using FoodOrderApp.Models;

public class OrderManagementForm : Form
{
    private DataGridView dgvOrders;
    private Button btnSave;
    private DbManager db;
    private List<Order> orders;

    private readonly string[] orderStatusOptions = new[] { "Новый", "Выполняется", "Доставка", "Завершен" };

    public OrderManagementForm()
    {
        db = new DbManager();
        InitializeComponent();
        LoadData();
    }

    private void InitializeComponent()
    {
        this.Text = "Управление заказами";
        this.Width = 900;
        this.Height = 600;

        dgvOrders = new DataGridView
        {
            Left = 20,
            Top = 20,
            Width = 840,
            Height = 480,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            EditMode = DataGridViewEditMode.EditOnEnter
        };

        btnSave = new Button
        {
            Text = "Сохранить изменения",
            Left = 20,
            Top = 510,
            Width = 150
        };

        btnSave.Click += BtnSave_Click;

        Controls.Add(dgvOrders);
        Controls.Add(btnSave);

        dgvOrders.DataError += (s, e) => { e.ThrowException = false; };
    }

    private void LoadData()
    {
        try
        {
            // 1. Получаем данные из базы
            orders = db.GetAllOrders();

            // 2. Создаем временную таблицу с колонкой "Status"
            var tempTable = new DataTable();
            tempTable.Columns.Add("Id", typeof(int));
            tempTable.Columns.Add("Client", typeof(string));
            tempTable.Columns.Add("OrderDate", typeof(DateTime));
            tempTable.Columns.Add("Address", typeof(string));
            tempTable.Columns.Add("Status", typeof(string)); // Добавляем колонку для статуса
            tempTable.Columns.Add("PaymentStatus", typeof(string));
            tempTable.Columns.Add("Total", typeof(decimal));

            // 3. Заполняем данными
            foreach (var order in orders)
            {
                tempTable.Rows.Add(
                    order.Id,
                    order.Client?.Names ?? "Неизвестно",
                    order.OrderDateTime,
                    order.DeliveryAddress,
                    order.Status, // Заполняем статус здесь
                    order.Payments?.FirstOrDefault()?.Status.ToString() ?? "Не оплачено",
                    order.TotalPrice
                );
            }

            // 4. Настраиваем DataGridView
            dgvOrders.AutoGenerateColumns = false;
            dgvOrders.DataSource = tempTable;

            // 5. Очищаем существующие колонки и добавляем заново
            dgvOrders.Columns.Clear();

            // 6. Добавляем колонки в правильном порядке
            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                ReadOnly = true
            });

            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Client",
                HeaderText = "Клиент",
                DataPropertyName = "Client",
                ReadOnly = true
            });

            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "OrderDate",
                HeaderText = "Дата заказа",
                DataPropertyName = "OrderDate",
                ReadOnly = true
            });

            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Address",
                HeaderText = "Адрес",
                DataPropertyName = "Address",
                ReadOnly = true
            });

            // 7. Добавляем ComboBox для статуса
            var statusCol = new DataGridViewComboBoxColumn
            {
                Name = "Status",
                HeaderText = "Статус заказа",
                DataPropertyName = "Status",
                DataSource = new List<string>(orderStatusOptions),
                ValueType = typeof(string),
                DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                FlatStyle = FlatStyle.Flat,
                ReadOnly = false
            };
            dgvOrders.Columns.Add(statusCol);

            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PaymentStatus",
                HeaderText = "Оплата",
                DataPropertyName = "PaymentStatus",
                ReadOnly = true
            });

            dgvOrders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Сумма",
                DataPropertyName = "Total",
                ReadOnly = true
            });

            // 8. Значения статусов уже привязаны по DataPropertyName "Status" из tempTable, поэтому дополнительно присваивать не нужно
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                          MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    private void BtnSave_Click(object sender, EventArgs e)
    {
        foreach (DataGridViewRow row in dgvOrders.Rows)
        {
            if (row.IsNewRow) continue;

            int orderId = Convert.ToInt32(row.Cells["Id"].Value);
            string newStatus = Convert.ToString(row.Cells["Status"].Value); // Здесь используем "Status"

            var order = orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null && order.Status != newStatus)
            {
                db.UpdateOrderStatus(orderId, newStatus);
            }
        }

        MessageBox.Show("Статусы заказов обновлены.");
        LoadData();
    }
}
