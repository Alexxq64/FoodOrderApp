using System.Collections.Generic;
using System.Linq;
using FoodOrderApp.Models;

namespace FoodOrderApp.Services
{
    public static class OrderService
    {
        private static List<Order> _orders = new List<Order>
        {
            new Order { Id = 1, ClientId = 1, Status = "Ожидает", TotalPrice = 1200, DeliveryAddress = "ул. Пушкина" },
            new Order { Id = 2, ClientId = 2, Status = "В пути", TotalPrice = 850, DeliveryAddress = "пр. Ленина" },
            new Order { Id = 3, ClientId = 1, Status = "Доставлен", TotalPrice = 560, DeliveryAddress = "ул. Мира" }
        };

        public static List<Order> GetAllOrders()
        {
            // Для администратора — вернуть все заказы
            return _orders;
        }

        public static List<Order> GetUserOrders(int clientId)
        {
            // Для обычного пользователя — только свои заказы
            return _orders.Where(o => o.ClientId == clientId).ToList();
        }
    }
}
