using GameGabut.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut.Model
{
    class Armor : Item
    {
        public int DefenseBonus { get; private set; }

        public Armor(string name, int price, int status, int defenseBonus) : base(name, price, status)
        {
            DefenseBonus = defenseBonus;
            status = defenseBonus;
        }
    }
}
