using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FoodOrderApp.Data
{
    public static class DatabaseSeeder
    {
        public static void CheckAndPromptDatabaseSeed()
        {
            try
            {
                var db = new DbManager();
                var tables = db.GetTables();

                if (tables.Count == 0)
                {
                    MessageBox.Show(
                        "В базе данных отсутствуют таблицы. Возможно, база не инициализирована.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                if (IsDatabaseEmpty())
                {
                    var result = MessageBox.Show(
                        "База данных пуста. Хотите добавить тестовые данные?",
                        "Инициализация БД",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        SeedTestData();
                        MessageBox.Show("Тестовые данные будут добавлены (метод пока пустой).");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при проверке БД: " + ex.Message,
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private static bool IsDatabaseEmpty()
        {
            try
            {
                var db = new DbManager();
                using (var conn = db.GetConnection())
                {
                    conn.Open();

                    string sql = "SELECT COUNT(*) FROM users;";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        var count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count == 0;
                    }
                }
            }
            catch
            {
                // Если таблица users отсутствует — считаем что не пустая, чтобы не запустить сидинг ошибочно
                return false;
            }
        }

        public static void SeedTestData()
        {
            var db = new DbManager();

            using (var conn = db.GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var cmd = conn.CreateCommand();
                        cmd.Transaction = transaction;

                        // 1. Users
                        cmd.CommandText = @"
                    INSERT INTO users (Names, Login, Password, Role) VALUES
                    ('Иван Иванов', 'ivan', 'password123', 'user'),
                    ('Админ Админов', 'admin', 'adminpass', 'admin');";
                        cmd.ExecuteNonQuery();

                        // 2. Categories
                        cmd.CommandText = @"
                    INSERT INTO categories (Name) VALUES
                    ('Пицца'),
                    ('Суши'),
                    ('Напитки');";
                        cmd.ExecuteNonQuery();

                        // 3. Items
                        cmd.CommandText = @"
                    INSERT INTO items (CategoryId, Name, Description, Price, ImagePath) VALUES
                    (1, 'Маргарита', 'Классическая пицца с сыром и томатами', 450.00, NULL),
                    (1, 'Пепперони', 'Пицца с пепперони и сыром моцарелла', 550.00, NULL),
                    (2, 'Ролл Филадельфия', 'Ролл с лососем и сливочным сыром', 400.00, NULL),
                    (3, 'Кока-Кола 0.5л', 'Газированный напиток', 100.00, NULL);";
                        cmd.ExecuteNonQuery();

                        // 4. Orders
                        cmd.CommandText = @"
                    INSERT INTO orders (ClientId, OrderDateTime, Status, TotalPrice, DeliveryAddress) VALUES
                    (1, NOW(), 'новый', 1050.00, 'ул. Ленина, д. 10');";
                        cmd.ExecuteNonQuery();

                        // 5. OrderDetails
                        cmd.CommandText = @"
                    INSERT INTO orderdetails (OrderId, ItemId, Quantity, Price) VALUES
                    (1, 1, 1, 450.00),
                    (1, 2, 1, 550.00),
                    (1, 4, 1, 100.00);";
                        cmd.ExecuteNonQuery();

                        // 6. Payments
                        cmd.CommandText = @"
                    INSERT INTO payments (OrderId, PaymentType, Amount, Status) VALUES
                    (1, 'Карта', 1100.00, 'Paid');";
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Ошибка при добавлении тестовых данных: " + ex.Message,
                                        "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
