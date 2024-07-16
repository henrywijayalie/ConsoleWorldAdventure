using GameGabut.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut.Model
{
    class Enemy : Character
    {
        public int ExperienceReward { get; private set; }
        public int GoldReward { get; private set; }

        public Enemy(string name, int health, int attack, int defense, double criticalHit, double criticalChance, int expReward, int goldReward) : base(name, health, attack, defense, criticalHit, criticalChance)
        {
            ExperienceReward = expReward;
            GoldReward = goldReward;
        }
    }
}
