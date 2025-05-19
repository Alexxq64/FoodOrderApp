using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public PaymentType PaymentType { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidDate { get; set; }
        public PaymentStatus Status { get; set; }

        // Связь
        public Order Order { get; set; }
    }

    public enum PaymentType
    {
        Карта,
        Наличные,
        Онлайн
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Canceled,
        Failed
    }

}
