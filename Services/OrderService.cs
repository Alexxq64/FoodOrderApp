using System;
using System.Collections.Generic;
using FoodOrderApp.Models;
using FoodOrderApp.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace FoodOrderApp.Services
{
    public static class OrderService
    {
        private static DbManager _dbManager = new DbManager();

        public static List<Order> GetAllOrders()
        {
            var orders = new List<Order>();
            using (var conn = _dbManager.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM Orders", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            Id = reader.GetInt32("Id"),
                            ClientId = reader.GetInt32("ClientId"),
                            Status = reader.GetString("Status"),
                            TotalPrice = reader.GetDecimal("TotalPrice"),
                            DeliveryAddress = reader.GetString("DeliveryAddress"),
                            OrderDateTime = reader.GetDateTime("OrderDateTime")
                        });
                    }
                }
            }
            return orders;
        }

        public static List<Order> GetUserOrders(int clientId)
        {
            var orders = new List<Order>();
            using (var conn = _dbManager.GetConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM Orders WHERE ClientId = @clientId", conn);
                cmd.Parameters.AddWithValue("@clientId", clientId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            Id = reader.GetInt32("Id"),
                            ClientId = reader.GetInt32("ClientId"),
                            Status = reader.GetString("Status"),
                            TotalPrice = reader.GetDecimal("TotalPrice"),
                            DeliveryAddress = reader.GetString("DeliveryAddress"),
                            OrderDateTime = reader.GetDateTime("OrderDateTime")
                        });
                    }
                }
            }
            return orders;
        }

        public static bool CreateOrder(int userId, List<OrderDetail> orderDetails, string deliveryAddress = "Адрес по умолчанию")
        {
            using (var conn = _dbManager.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Считаем итоговую цену
                        decimal totalPrice = 0;
                        foreach (var detail in orderDetails)
                        {
                            totalPrice += detail.Price * detail.Quantity;
                        }

                        var orderCmd = new MySqlCommand(
                            "INSERT INTO Orders (ClientId, OrderDateTime, Status, TotalPrice, DeliveryAddress) " +
                            "VALUES (@ClientId, @OrderDateTime, @Status, @TotalPrice, @DeliveryAddress); SELECT LAST_INSERT_ID();",
                            conn, transaction);

                        orderCmd.Parameters.AddWithValue("@ClientId", userId);
                        orderCmd.Parameters.AddWithValue("@OrderDateTime", DateTime.Now);
                        orderCmd.Parameters.AddWithValue("@Status", "Ожидает");
                        orderCmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                        orderCmd.Parameters.AddWithValue("@DeliveryAddress", deliveryAddress);

                        int orderId = Convert.ToInt32(orderCmd.ExecuteScalar());

                        foreach (var detail in orderDetails)
                        {
                            var detailCmd = new MySqlCommand(
                                "INSERT INTO OrderDetails (OrderId, ItemId, Quantity, Price) " +
                                "VALUES (@OrderId, @ItemId, @Quantity, @Price)", conn, transaction);

                            detailCmd.Parameters.AddWithValue("@OrderId", orderId);
                            detailCmd.Parameters.AddWithValue("@ItemId", detail.Item.Id);
                            detailCmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                            detailCmd.Parameters.AddWithValue("@Price", detail.Price);

                            detailCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка при сохранении заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }
    }
}
