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
                using (var cmd = new MySqlCommand("SELECT id, names, login, role FROM users", conn))
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


    }
}
