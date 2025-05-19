using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Связь: Один ко многим (Category -> Items)
        public List<Item> Items { get; set; } = new List<Item>();
    }
}
