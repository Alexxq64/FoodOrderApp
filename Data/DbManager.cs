using System;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Collections.Generic;
using FoodOrderApp.Models;

namespace FoodOrderApp.Data
{
    public class DbManager
    {
        private readonly string _connectionString;

        public DbManager()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MySqlConnection"].ConnectionString;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        // Метод для проверки соединения с БД
        public bool CheckConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return conn.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<string> GetTables()
        {
            var tables = new List<string>();
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SHOW TABLES;", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tables.Add(reader.GetString(0));
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Логика обработки ошибок, проброс или возвращение пустого списка
                throw;
            }
            return tables;
        }

        public string GetStatisticsSummary()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();

                    int orderCount = 0;
                    decimal totalPayments = 0;

                    // Количество заказов
                    using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM orders;", conn))
                    {
                        orderCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Общая сумма оплат
                    using (var cmd = new MySqlCommand("SELECT IFNULL(SUM(amount), 0) FROM payments;", conn))
                    {
                        totalPayments = Convert.ToDecimal(cmd.ExecuteScalar());
                    }

                    return $"Общее количество заказов: {orderCount}\r\n" +
                           $"Общая сумма оплат: {totalPayments:C}";
                }
            }
            catch (Exception ex)
            {
                return $"Ошибка при загрузке статистики: {ex.Message}";
            }
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT id, names, login, role, password FROM users", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32("id"),
                            Names = reader.GetString("names"),
                            Login = reader.GetString("login"),
                            Role = (UserRole)Enum.Parse(typeof(UserRole), reader.GetString("role"), true),
                            Password = reader.GetString("password")
                        });
                    }
                }
            }
            return users;
        }


        public void DeleteUser(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("DELETE FROM users WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddUser(User user)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("INSERT INTO users (names, login, role, password) VALUES (@names, @login, @role, @password)", conn))
                {
                    cmd.Parameters.AddWithValue("@names", user.Names);
                    cmd.Parameters.AddWithValue("@login", user.Login);
                    cmd.Parameters.AddWithValue("@role", user.Role.ToString());
                    cmd.Parameters.AddWithValue("@password", user.Password ?? "default");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateUser(User user)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("UPDATE users SET names=@names, login=@login, role=@role, password=@password WHERE id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", user.Id);
                    cmd.Parameters.AddWithValue("@names", user.Names);
                    cmd.Parameters.AddWithValue("@login", user.Login);
                    cmd.Parameters.AddWithValue("@role", user.Role.ToString());
                    cmd.Parameters.AddWithValue("@password", user.Password);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public User GetUserByLogin(string login)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT * FROM users WHERE login = @login", conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Names = reader["Names"].ToString(),
                                Login = reader["Login"].ToString(),
                                Password = reader["Password"].ToString(),
                                Role = (UserRole)Enum.Parse(typeof(UserRole), reader.GetString("role"), true),
                            };
                        }
                    }
                }
            }

            return null;
        }

        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT * FROM categories ORDER BY name", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            categories.Add(new Category
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString()
                            });
                        }
                    }
                }
            }
            return categories;
        }

        // Добавить новую категорию
        public void AddCategory(string categoryName)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("INSERT INTO categories (name) VALUES (@name)", conn))
                {
                    cmd.Parameters.AddWithValue("@name", categoryName);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Получить все пункты меню (Items) с категорией
        public List<Item> GetAllItems()
        {
            var items = new List<Item>();
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT * FROM items ORDER BY name", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                ImagePath = reader["ImagePath"].ToString()
                            });
                        }
                    }
                }
            }
            return items;
        }

        // Добавить новый пункт меню
        public void AddItem(Item item)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("INSERT INTO items (CategoryId, Name, Description, Price, ImagePath) VALUES (@catId, @name, @desc, @price, @img)", conn))
                {
                    cmd.Parameters.AddWithValue("@catId", item.CategoryId);
                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@desc", item.Description);
                    cmd.Parameters.AddWithValue("@price", item.Price);
                    cmd.Parameters.AddWithValue("@img", item.ImagePath ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Обновить пункт меню
        public void UpdateItem(Item item)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("UPDATE items SET CategoryId=@catId, Name=@name, Description=@desc, Price=@price, ImagePath=@img WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", item.Id);
                    cmd.Parameters.AddWithValue("@catId", item.CategoryId);
                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@desc", item.Description);
                    cmd.Parameters.AddWithValue("@price", item.Price);
                    cmd.Parameters.AddWithValue("@img", item.ImagePath ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Удалить пункт меню
        public void DeleteItem(int id)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("DELETE FROM items WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateOrderStatus(int orderId, string newStatus)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new MySqlCommand("UPDATE orders SET Status = @status WHERE Id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@status", newStatus);
                    cmd.Parameters.AddWithValue("@id", orderId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Order> GetAllOrders()
        {
            var orders = new List<Order>();
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"
            SELECT o.Id, o.ClientId, o.OrderDateTime, o.Status, o.TotalPrice, o.DeliveryAddress,
                   u.Names as ClientName,
                   p.Status as PaymentStatus
            FROM orders o
            LEFT JOIN users u ON o.ClientId = u.Id
            LEFT JOIN payments p ON p.OrderId = o.Id
            ORDER BY o.OrderDateTime DESC";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var order = new Order
                        {
                            Id = reader.GetInt32("Id"),
                            ClientId = reader.GetInt32("ClientId"),
                            OrderDateTime = reader.GetDateTime("OrderDateTime"),
                            Status = reader.GetString("Status"),
                            TotalPrice = reader.GetDecimal("TotalPrice"),
                            DeliveryAddress = reader.GetString("DeliveryAddress"),
                            Client = new User
                            {
                                Names = reader.IsDBNull(reader.GetOrdinal("ClientName")) ? "" : reader.GetString("ClientName")
                            },
                            Payments = new List<Payment>()
                            // Для простоты Payment заполним только статус первого платежа, можно расширить при необходимости
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("PaymentStatus")))
                        {
                            var paymentStatusStr = reader.GetString("PaymentStatus");
                            if (Enum.TryParse(paymentStatusStr, out PaymentStatus paymentStatus))
                            {
                                order.Payments.Add(new Payment { Status = paymentStatus });
                            }
                        }

                        orders.Add(order);
                    }
                }
            }
            return orders;
        }



    }
}
