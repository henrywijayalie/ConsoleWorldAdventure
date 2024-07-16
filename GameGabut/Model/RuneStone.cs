using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut.Model
{
    public class RuneStone
    {
        public string Name { get; set; }
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }
        public double CriticalChanceBonus { get; set; }
        public int Duration { get; set; }
    }
}
