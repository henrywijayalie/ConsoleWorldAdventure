using GameGabut.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut.Model
{
    class RuneStone : Item
    {
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }
        public double CriticalChanceBonus { get; set; }
        public RuneStone(string name, int price, int status, int attackBonus, int defenseBonus, double criticalChanceBonus) : base(name, price, status)
        {
            AttackBonus = attackBonus;
            DefenseBonus = defenseBonus;
            CriticalChanceBonus = criticalChanceBonus;
            status = 0;
        }
    }
}
