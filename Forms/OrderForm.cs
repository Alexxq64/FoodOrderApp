using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FoodOrderApp.Helpers;
using FoodOrderApp.Models;
using FoodOrderApp.Services;

namespace FoodOrderApp.Forms
{
    public class OrderForm : Form
    {
        private List<OrderDetail> _orderDetails;
        private ListBox listBoxOrderDetails;
        private Button btnConfirm;
        private Button btnCancel;

        public OrderForm(List<OrderDetail> orderDetails)
        {
            _orderDetails = orderDetails;
            InitializeComponent();
            LoadOrderDetails();
        }

        private void InitializeComponent()
        {
            this.Text = "Оформление заказа";
            this.Width = 600;
            this.Height = 400;

            listBoxOrderDetails = new ListBox
            {
                Left = 20,
                Top = 20,
                Width = 540,
                Height = 280
            };

            btnConfirm = new Button
            {
                Text = "Подтвердить заказ",
                Left = 20,
                Top = 320,
                Width = 150
            };
            btnConfirm.Click += BtnConfirm_Click;

            btnCancel = new Button
            {
                Text = "Отмена",
                Left = 200,
                Top = 320,
                Width = 150
            };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(listBoxOrderDetails);
            this.Controls.Add(btnConfirm);
            this.Controls.Add(btnCancel);
        }

        private void LoadOrderDetails()
        {
            listBoxOrderDetails.Items.Clear();
            foreach (var detail in _orderDetails)
            {
                listBoxOrderDetails.Items.Add($"{detail.Item.Name} x{detail.Quantity} - {detail.Price * detail.Quantity} руб.");
            }
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (_orderDetails.Count == 0)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = Session.CurrentUser;
            if (user == null)
            {
                MessageBox.Show("Пользователь не авторизован!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool success = OrderService.CreateOrder(user.Id, _orderDetails);
            if (success)
            {
                MessageBox.Show("Заказ оформлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

    }
}
