using GameGabut.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut.Model
{
    class MaterialItem : Item
    {
        public double EnhancementChance { get; private set; }

        public MaterialItem(string name, int price, int status, int enhancementChance) : base(name, price, status)
        {
            EnhancementChance = enhancementChance;
            Status = enhancementChance;
        }
    }
}
