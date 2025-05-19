using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrderApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Names { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }

        // Связь: Один ко многим (User -> Orders)
        public List<Order> Orders { get; set; } = new List<Order>();
    }

    public enum UserRole
    {
        user,
        admin
    }

}
