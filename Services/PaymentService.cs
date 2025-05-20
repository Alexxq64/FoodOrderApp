using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FoodOrderApp.Models;
using FoodOrderApp.Helpers;
using FoodOrderApp.Data;
using MySql.Data.MySqlClient;

namespace FoodOrderApp.Services
{
    public static class PaymentService
    {
        private static readonly DbManager dbManager = new DbManager();

        /// <summary>
        /// Получение платежей текущего пользователя
        /// </summary>
        public static List<Payment> GetUserPayments()
        {
            var payments = new List<Payment>();
            var user = Session.CurrentUser;
            if (user == null)
                return payments;

            try
            {
                using (var conn = dbManager.GetConnection())
                {
                    conn.Open();

                    string query = @"
                        SELECT p.Id, p.OrderId, p.Amount, p.PaidDate, p.PaymentType, p.Status
                        FROM payments p
                        JOIN orders o ON o.Id = p.OrderId
                        WHERE o.ClientId = @UserId AND p.Status = 'Paid'
                        ORDER BY p.PaidDate DESC;";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", user.Id);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                payments.Add(new Payment
                                {
                                    Id = reader.GetInt32("Id"),
                                    OrderId = reader.GetInt32("OrderId"),
                                    Amount = reader.GetDecimal("Amount"),
                                    PaidDate = reader.GetDateTime("PaidDate"),
                                    PaymentType = (PaymentType)Enum.Parse(typeof(PaymentType), reader.GetString("PaymentType")),
                                    Status = (PaymentStatus)Enum.Parse(typeof(PaymentStatus), reader.GetString("Status"))
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении платежей: {ex.Message}");
            }

            return payments;
        }

        /// <summary>
        /// Имитация оплаты: запись в базу + обновление заказа
        /// </summary>
        public static void ProcessPayment(int orderId, decimal amount, PaymentType paymentType)
        {
            using (var conn = dbManager.GetConnection())
            {
                conn.Open();

                // 1. Проверка: есть ли уже оплаченный платёж
                using (var cmd = new MySqlCommand(
                    "SELECT COUNT(*) FROM payments WHERE OrderId = @orderId AND Status = 'Paid'", conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    var count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                        throw new Exception("Заказ уже оплачен.");
                }

                // 2. Вставка платежа
                using (var cmd = new MySqlCommand(@"
                    INSERT INTO payments (OrderId, PaymentType, Amount, PaidDate, Status)
                    VALUES (@orderId, @type, @amount, @date, 'Paid')", conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    cmd.Parameters.AddWithValue("@type", paymentType.ToString());
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool IsOrderPaid(int orderId)
        {
            using (var conn = dbManager.GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM payments WHERE OrderId = @orderId AND Status = 'Paid'", conn))
                {
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }
    }
}
