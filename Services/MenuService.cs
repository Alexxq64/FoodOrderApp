using System.Collections.Generic;
using FoodOrderApp.Models;
using FoodOrderApp.Data;
using MySql.Data.MySqlClient;

namespace FoodOrderApp.Services
{
    public static class MenuService
    {
        public static List<Category> GetAllCategoriesWithItems()
        {
            var categories = new List<Category>();
            var dbManager = new DbManager();

            using (var conn = dbManager.GetConnection())
            {
                conn.Open();

                // Загружаем категории
                var cmdCategories = new MySqlCommand("SELECT Id, Name FROM Categories", conn);
                using (var reader = cmdCategories.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            Id = reader.GetInt32("Id"),
                            Name = reader.GetString("Name")
                        });
                    }
                }

                // Загружаем элементы для каждой категории
                foreach (var category in categories)
                {
                    var cmdItems = new MySqlCommand("SELECT Id, CategoryId, Name, Description, Price, ImagePath FROM Items WHERE CategoryId = @categoryId", conn);
                    cmdItems.Parameters.AddWithValue("@categoryId", category.Id);

                    using (var reader = cmdItems.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var item = new Item
                            {
                                Id = reader.GetInt32("Id"),
                                CategoryId = reader.GetInt32("CategoryId"),
                                Name = reader.GetString("Name"),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString("Description"),
                                Price = reader.GetDecimal("Price"),
                                ImagePath = reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? null : reader.GetString("ImagePath")
                            };
                            category.Items.Add(item);
                        }
                    }
                }
            }

            return categories;
        }
    }
}
