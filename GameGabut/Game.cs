﻿using GameGabut.Interface;
using GameGabut.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut
{

    class Game
    {
        private Player player;
        private List<Enemy> enemies;
        private Shop shop;
        private GameData gameData;
        public Game()
        {

            string filePath = $"gamedata.json";
            gameData = GameData.LoadFromJson(filePath);


            Console.WriteLine("Masukkan nama karakter:");
            string name = Console.ReadLine();

            player = Player.LoadFromJson(name);
            if (player == null)
            {
                player = new Player(name, 100, 10, 5, 5, 5);
                Console.WriteLine("Karakter baru dibuat.");
            }
            else
            {
                Console.WriteLine("Karakter dimuat dari file.");
            }

            enemies = gameData.Enemies.Select(e => new Enemy(e.Name, e.Health, e.Attack, e.Defense, e.CriticalHit, e.CriticalChance, e.ExpReward, e.GoldReward)).ToList();
            shop = new Shop(gameData, player);
        }

        public void Start()
        {
            Console.WriteLine("Selamat datang di Console World Adventure!");

            while (true)
            {
                Console.WriteLine("\nPilih aksi:");
                Console.WriteLine("1. Bertarung");
                Console.WriteLine("2. Buka inventori");
                Console.WriteLine("3. Kunjungi toko");
                Console.WriteLine("4. Lihat status");
                Console.WriteLine("5. Simpan dan Keluar");
                Console.Write("Input : ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Battle();
                        break;
                    case "2":
                        player.OpenInventory();
                        break;
                    case "3":
                        shop.Visit();
                        break;
                    case "4":
                        player.DisplayStatus();
                        break;
                    case "5":
                        player.SaveToJson();
                        Console.WriteLine("Data karakter disimpan. Terima kasih telah bermain!");
                        return;
                    default:
                        Console.WriteLine("Pilihan tidak valid.");
                        break;
                }
            }
        }
        private Enemy CreateEnemy(Enemy enemy, int playerLevel)
        {
            // Berdasarkan level player, buat musuh dengan atribut yang disesuaikan
            // Contoh sederhana, Anda bisa mengalikan atribut musuh dengan level player
            // Sesuaikan dengan logika atau sistem skala kesulitan permainan Anda.
            var enemyHealth = playerLevel == 1 ? enemy.Health : enemy.Health * (playerLevel / 2);
            var enemyAttack = playerLevel == 1 ? enemy.Attack : enemy.Attack * (playerLevel / 2);
            var enemyDefense = playerLevel == 1 ? enemy.Defense : enemy.Defense * (playerLevel / 2);
            var enemyCriticalHit = playerLevel == 1 ? enemy.CriticalHit : enemy.CriticalHit * (playerLevel / 2);
            var enemyCriticalChance = playerLevel == 1 ? enemy.CriticalChance : enemy.CriticalChance * (playerLevel / 2);
            var expReward = playerLevel == 1 ? enemy.ExperienceReward : enemy.ExperienceReward * (playerLevel / 2);
            var goldReward = playerLevel == 1 ? enemy.GoldReward : enemy.GoldReward * (playerLevel / 2);

            return new Enemy(enemy.Name, enemyHealth, enemyAttack, enemyDefense, enemyCriticalHit, enemyCriticalChance, expReward, goldReward);
        }
        private void Battle()
        {
            Random random = new Random();
            var newEnemy = enemies[random.Next(enemies.Count)];
            Enemy enemy = CreateEnemy(newEnemy, player.Level);


            Console.WriteLine($"Kamu bertemu dengan {enemy.Name}!");

            bool playerEscaped = false;

            while (player.Health > 0 && enemy.Health > 0)
            {
                if (playerEscaped == true) break;
                Console.WriteLine($"\n{player.Name} HP: {player.Health} | {enemy.Name} HP: {enemy.Health}");
                Console.WriteLine("1. Serang");
                Console.WriteLine("2. Bertahan");
                Console.WriteLine("3. Gunakan Potion");
                Console.WriteLine("4. Coba Kabur");
                Console.Write("Pilih Aksi : ");

                string choice = Console.ReadLine();

                bool playerDefending = false;

                switch (choice)
                {
                    case "1":
                        int damage = player.PerformAttack();
                        enemy.TakeDamage(damage);
                        Console.WriteLine($"Kamu menyerang {enemy.Name} dan menyebabkan {damage} damage.");
                        break;
                    case "2":
                        playerDefending = true;
                        Console.WriteLine("Kamu bersiap untuk bertahan dari serangan berikutnya.");
                        break;
                    case "3":
                        player.UsePotion();
                        break;
                    case "4":
                        if (random.Next(100) < 50) // 50% chance to escape
                        {
                            Console.WriteLine("Kamu berhasil kabur dari pertarungan!");
                            playerEscaped = true;
                        }
                        else
                        {
                            Console.WriteLine("Kamu gagal kabur dari pertarungan!");
                        }
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid. Giliran dilewati.");
                        break;
                }

                if (playerEscaped == false && enemy.Health > 0)
                {
                    int enemyDamage = enemy.PerformAttack();
                    if (playerDefending)
                    {
                        enemyDamage /= 2; // Damage dikurangi setengah saat bertahan
                        Console.WriteLine($"Kamu bertahan dari serangan. Damage dikurangi menjadi {enemyDamage}.");
                    }
                    player.TakeDamage(enemyDamage);
                    Console.WriteLine($"{enemy.Name} menyerang dan menyebabkan {enemyDamage} damage.");
                }

            }

            if (player.Health > 0 && !playerEscaped)
            {
                Console.WriteLine($"Kamu mengalahkan {enemy.Name}!");
                player.GainExperience(enemy.ExperienceReward);
                player.GainGold(enemy.GoldReward);

                // Tambahkan kemungkinan mendapatkan item dari musuh
                if (random.Next(100) < 30) // 30% chance to get an item
                {
                    Item droppedItem = GetRandomItem();
                    player.Inventory.Add(droppedItem);
                    Console.WriteLine($"Kamu mendapatkan {droppedItem.Name} dari {enemy.Name}!");
                }
            }
            else if (player.Health <= 0)
            {
                Console.WriteLine("Kamu kalah dalam pertarungan.");
                player.Die();
            }
            else if (playerEscaped)
            {
                Console.WriteLine("Kamu berhasil kabur dari pertarungan.");
            }

        }

        private Item GetRandomItem()
        {
            Random random = new Random();
            int itemType = random.Next(3);
            switch (itemType)
            {
                case 0:
                    WeaponData weaponData = gameData.Weapons[random.Next(gameData.Weapons.Count)];
                    return new Weapon(weaponData.Name, weaponData.Price, weaponData.AttackBonus, weaponData.AttackBonus);
                case 1:
                    ArmorData armorData = gameData.Armors[random.Next(gameData.Armors.Count)];
                    return new Armor(armorData.Name, armorData.Price, armorData.DefenseBonus, armorData.DefenseBonus);
                default:
                    PotionData potionData = gameData.Potions[random.Next(gameData.Potions.Count)];
                    return new Potion(potionData.Name, potionData.Price, potionData.HealAmount, potionData.HealAmount);
            }
        }

    }

}
