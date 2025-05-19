using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;
using FoodOrderApp.Models;
using FoodOrderApp.Data;

namespace FoodOrderApp.Services
{
    public class AuthService
    {
        private readonly DbManager _dbManager;

        public AuthService()
        {
            _dbManager = new DbManager();
        }

        private const int Base64Sha256Length = 44; // Длина Base64 SHA256 хэша

        // Хеширование пароля (SHA256 + Base64)
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Получение пользователя из БД по логину
        private User GetUserByLogin(string login)
        {
            using (MySqlConnection conn = _dbManager.GetConnection())
            {
                conn.Open();
                string sql = "SELECT Id, Names, Login, Password, Role FROM users WHERE Login = @login";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32("Id"),
                                Names = reader.GetString("Names"),
                                Login = reader.GetString("Login"),
                                Password = reader.GetString("Password"), // Для проверки
                                Role = (UserRole)Enum.Parse(typeof(UserRole), reader.GetString("Role"), true)
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Обновление пароля в БД (для миграции с plain-text на хэш)
        private void UpdateUserPassword(int userId, string hashedPassword)
        {
            using (MySqlConnection conn = _dbManager.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE users SET Password = @password WHERE Id = @id";
                using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Логика авторизации с проверкой и обновлением пароля
        public User Login(string login, string password)
        {
            var user = GetUserByLogin(login);
            if (user == null)
                return null;

            // Проверяем длину пароля — если меньше 44, считаем plain-text
            if (user.Password.Length < Base64Sha256Length)
            {
                // Проверяем обычный пароль
                if (user.Password == password)
                {
                    // Обновляем пароль, сохраняя хэш
                    var hashedPassword = HashPassword(password);
                    UpdateUserPassword(user.Id, hashedPassword);
                    user.Password = hashedPassword;
                    return user;
                }
            }
            else
            {
                // Пароль в хэшированном виде, сравниваем хэши
                var hashedInput = HashPassword(password);
                if (hashedInput == user.Password)
                    return user;
            }

            // Если ни одно из условий не подошло — ошибка входа
            return null;
        }

        // Регистрация
        public bool Register(string name, string login, string password, out string error)
        {
            error = null;

            using (MySqlConnection conn = _dbManager.GetConnection())
            {
                conn.Open();

                string checkSql = "SELECT COUNT(*) FROM users WHERE Login = @login";
                using (MySqlCommand checkCmd = new MySqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@login", login);
                    var count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        error = "Пользователь с таким логином уже существует.";
                        return false;
                    }
                }

                string hashedPassword = HashPassword(password);
                string insertSql = "INSERT INTO users (Names, Login, Password, Role) VALUES (@name, @login, @password, 'user')";
                using (MySqlCommand insertCmd = new MySqlCommand(insertSql, conn))
                {
                    insertCmd.Parameters.AddWithValue("@name", name);
                    insertCmd.Parameters.AddWithValue("@login", login);
                    insertCmd.Parameters.AddWithValue("@password", hashedPassword);

                    int rows = insertCmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}
