using GameGabut.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut.Model
{
    class Weapon : Item
    {
        public int AttackBonus { get; private set; }

        public Weapon(string name, int price, int status, int attackBonus) : base(name, price, status)
        {
            AttackBonus = attackBonus;
            status = attackBonus;
        }
    }
}
