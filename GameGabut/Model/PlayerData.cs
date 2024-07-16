using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut.Model
{
    class PlayerData
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int Gold { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public double CriticalHit { get; set; }
        public double CriticalChance { get; set; }
        public int Defense { get; set; }
        public int Deaths { get; set; }
        public string EquippedWeaponName { get; set; }
        public int EquippedWeaponAttackBonus { get; set; }
        public string EquippedArmorName { get; set; }
        public int EquippedArmorDefenseBonus { get; set; }
        public List<ItemData> Inventory { get; set; }
    }
}
