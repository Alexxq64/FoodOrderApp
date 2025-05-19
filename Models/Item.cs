using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Models
{
    public class Item
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }

        // Связь: Один ко многим (Item -> Category)
        public Category Category { get; set; }

        // Связь: Один ко многим (Item -> OrderDetails)
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
