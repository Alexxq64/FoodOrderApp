using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryAddress { get; set; }

        // Связь с пользователем
        public User Client { get; set; }

        // Связь с деталями заказа и платежами
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public List<Payment> Payments { get; set; } = new List<Payment>();
    }

}
