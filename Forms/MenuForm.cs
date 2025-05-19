using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FoodOrderApp.Models;
using FoodOrderApp.Services;

namespace FoodOrderApp.Forms
{
    public class MenuForm : Form
    {
        private List<Category> _categories;
        private List<OrderDetail> _currentOrderDetails = new List<OrderDetail>();
        private FlowLayoutPanel _panelCategories;
        private Button btnPlaceOrder;

        public MenuForm()
        {
            InitializeComponent();
            LoadMenu();
        }

        private void InitializeComponent()
        {
            this.Text = "Меню";
            this.Width = 900;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;

            _panelCategories = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoScroll = true,
                Width = this.ClientSize.Width,
                Height = 600,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            this.Controls.Add(_panelCategories);

            btnPlaceOrder = new Button
            {
                Text = "Оформить заказ",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            btnPlaceOrder.Click += BtnPlaceOrder_Click;
            this.Controls.Add(btnPlaceOrder);
        }

        private void LoadMenu()
        {
            _categories = MenuService.GetAllCategoriesWithItems();

            _panelCategories.Controls.Clear();

            foreach (var category in _categories)
            {
                var groupBox = new GroupBox
                {
                    Text = category.Name,
                    Width = 850,
                    Height = 150,
                    Padding = new Padding(10),
                    Margin = new Padding(10)
                };

                var panelItems = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false
                };

                foreach (var item in category.Items)
                {
                    var itemPanel = CreateItemPanel(item);
                    panelItems.Controls.Add(itemPanel);
                }

                groupBox.Controls.Add(panelItems);
                _panelCategories.Controls.Add(groupBox);
            }
        }

        private Panel CreateItemPanel(Item item)
        {
            var panel = new Panel
            {
                Width = 800,
                Height = 50,
                Margin = new Padding(5)
            };

            var lblName = new Label
            {
                Text = item.Name,
                Location = new Point(5, 5),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            panel.Controls.Add(lblName);

            var lblDesc = new Label
            {
                Text = item.Description,
                Location = new Point(150, 7),
                AutoSize = true,
                Width = 300
            };
            panel.Controls.Add(lblDesc);

            var lblPrice = new Label
            {
                Text = $"{item.Price} ₽",
                Location = new Point(460, 5),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };
            panel.Controls.Add(lblPrice);

            var numericQty = new NumericUpDown
            {
                Location = new Point(520, 5),
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Width = 50
            };
            panel.Controls.Add(numericQty);

            var btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(580, 2),
                Width = 80,
                Height = 30
            };
            btnAdd.Click += (s, e) =>
            {
                int quantity = (int)numericQty.Value;
                AddToOrder(item, quantity);
            };
            panel.Controls.Add(btnAdd);

            return panel;
        }

        private void AddToOrder(Item item, int quantity)
        {
            // Проверяем, есть ли уже такой товар в заказе
            var existing = _currentOrderDetails.Find(od => od.Item.Id == item.Id);
            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                _currentOrderDetails.Add(new OrderDetail
                {
                    Item = item,
                    ItemId = item.Id,
                    Quantity = quantity,
                    Price = item.Price
                });
            }

            MessageBox.Show($"Добавлено: {item.Name} x{quantity}", "В заказ добавлено", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (_currentOrderDetails.Count == 0)
            {
                MessageBox.Show("Корзина пуста. Добавьте блюда перед оформлением.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Открываем форму оформления заказа, передавая детали
            var orderForm = new OrderForm(_currentOrderDetails);
            orderForm.ShowDialog();

            // После оформления можно очистить корзину
            _currentOrderDetails.Clear();
        }
    }
}
