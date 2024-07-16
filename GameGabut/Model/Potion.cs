using GameGabut.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut.Model
{
    class Potion : Item
    {
        public int HealAmount { get; private set; }

        public Potion(string name, int price, int status, int healAmount) : base(name, price, status)
        {
            HealAmount = healAmount;
            status = healAmount;
        }
    }
}
