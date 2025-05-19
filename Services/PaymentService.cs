using System;
using System.Collections.Generic;
using System.Linq;
using FoodOrderApp.Models;
using FoodOrderApp.Helpers;

namespace FoodOrderApp.Services
{
    public static class PaymentService
    {
        // Временное хранилище (заглушка). В реальном приложении — база данных.
        private static List<Payment> AllPayments = new List<Payment>
        {
            new Payment
            {
                Id = 1,
                OrderId = 1,
                Amount = 1200,
                PaidDate = DateTime.Now.AddDays(-2),
                PaymentType = PaymentType.Карта,
                Status = PaymentStatus.Paid,
                Order = new Order { ClientId = 1 }
            },
            new Payment
            {
                Id = 2,
                OrderId = 2,
                Amount = 800,
                PaidDate = DateTime.Now.AddDays(-1),
                PaymentType = PaymentType.Наличные,
                Status = PaymentStatus.Paid,
                Order = new Order { ClientId = 2 }
            },
            new Payment
            {
                Id = 3,
                OrderId = 3,
                Amount = 550,
                PaidDate = DateTime.Now.AddHours(-6),
                PaymentType = PaymentType.Онлайн,
                Status = PaymentStatus.Pending,
                Order = new Order { ClientId = 1 }
            }
        };

        public static List<Payment> GetUserPayments()
        {
            var user = Session.CurrentUser;
            if (user == null)
                return new List<Payment>();

            return AllPayments
                .Where(p => p.Order.ClientId == user.Id)
                .ToList();
        }
    }
}
