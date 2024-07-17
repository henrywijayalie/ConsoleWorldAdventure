using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut.Interface
{
    class Item
    {
        public string Name { get; protected set; }
        public int Price { get; set; }
        public int Status { get; set; }
        public bool IsEquipped { get; set; }

        public Item(string name, int price, int status)
        {
            Name = name;
            Price = price;
            Status = status;
            IsEquipped = false;
        }
    }
}
