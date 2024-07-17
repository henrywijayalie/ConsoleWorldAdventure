using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut.Interface
{
    class Character
    {
        public string Name { get; protected set; }
        public int Health { get; protected set; }
        public int Attack { get; protected set; }
        public int Defense { get; protected set; }
        public double CriticalHit { get; protected set; } // New attribute for critical hit multiplier
        public double CriticalChance { get; protected set; } // New attribute for critical hit chance in percentage

        public Character(string name, int health, int attack, int defense, double criticalHit, double criticalChance)
        {
            Name = name;
            Health = health;
            Attack = attack;
            Defense = defense;
            CriticalHit = criticalHit;
            CriticalChance = criticalChance;
        }

        public virtual int PerformAttack()
        {
            Random random = new Random();
            int baseDamage = random.Next(Attack - 2, Attack + 3);
            var randDouble = random.NextDouble();
            // Calculate if the attack is a critical hit
            if (randDouble * 100 < CriticalChance)
            {
                Console.WriteLine("Critical Hit!");
                return (int)(baseDamage * CriticalHit);
            }

            return baseDamage;
        }

        public int TakeDamage(int damage)
        {
            int actualDamage = Math.Max(damage - Defense, 0);
            Health -= actualDamage;
            if (Health < 0) { 
                Health = 0; 
                return actualDamage; 
            }
            return actualDamage;
        }
    }

}
