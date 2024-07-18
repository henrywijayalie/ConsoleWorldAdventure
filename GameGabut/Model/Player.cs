using GameGabut.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GameGabut.Model
{
    class Player : Character
    {
        public int Level { get; private set; }
        public int Experience { get; private set; }
        public int Gold { get; private set; }
        public int Deaths { get; private set; }
        public double CriticalHit { get; protected set; } // New attribute for critical hit multiplier
        public double CriticalChance { get; protected set; } // New attribute for critical hit chance in percentage
        public List<Item> Inventory { get; private set; }
        public Weapon EquippedWeapon { get; private set; }
        public Armor EquippedArmor { get; private set; }
        public static List<RuneStone> RuneStones { get; private set; }

        public Player(string name, int health, int attack, int defense, double criticalHit, double criticalChance, GameData gameData) : base(name, health, attack, defense, criticalHit, criticalChance)
        {
            Level = 1;
            Experience = 0;
            Gold = 20;
            Deaths = 0;
            Inventory = new List<Item>();

            var dataRunes = gameData.RuneStones.Select(e => new RuneStone(e.Name, e.Price, 0, e.AttackBonus, e.DefenseBonus, e.CriticalChanceBonus)).ToList();
            RuneStones = new List<RuneStone>();
            RuneStones.AddRange(dataRunes);
        }
        public int Sell(Item item)
        {
            if (Inventory.Contains(item))
            {
                Inventory.Remove(item);
                GainGold(item.Price);
                return item.Price;
            }
            else
            {
                Console.WriteLine("Item tidak ditemukan di dalam inventori.");
                return 0;
            }
        }
        public bool IsItemEquipped(Item item)
        {
            if (item is Weapon && item == EquippedWeapon)
                return true;
            else if (item is Armor && item == EquippedArmor)
                return true;
            else
                return false;
        }

        public void Die()
        {
            Random rand = new Random();
            Deaths++;
            int expLoss = 50 * Level;
            Experience = Math.Max(0, Experience - expLoss);
            int goldLoss = rand.Next(1, Level);
            Gold -= goldLoss;
            var (gold, silver, copper) = ConvertCopper(goldLoss);
            var newAmount = "";
            if (gold != 0)
            {
                newAmount += ($"{gold} Gold ");
            }
            if (silver != 0)
            {
                newAmount += ($"{silver} Silver ");
            }
            if (copper != 0)
            {
                newAmount += ($"{copper} Copper");
            }

            Console.WriteLine($"Kamu mati! Total kematian: {Deaths}");
            Console.WriteLine($"Kamu kehilangan {expLoss} EXP.");
            Console.WriteLine($"Kamu menjatuhkan {newAmount}.");
            Health = 100 + (Level - 1) * 20; // Reset HP
        }

        public void GainExperience(int amount)
        {
            Experience += amount;
            Health += 10;
            Console.WriteLine($"Kamu mendapatkan {amount} EXP dan Recovery 10 HP.");
            CheckLevelUp();
        }

        public void GainGold(int amount)
        {
            Gold += amount;

            var (gold, silver, copper) = ConvertCopper(amount);
            var newAmount = "";
            if (gold != 0)
            {
                newAmount += ($"{gold} Gold ");
            }
            if (silver != 0)
            {
                newAmount += ($"{silver} Silver ");
            }
            if (copper != 0)
            {
                newAmount += ($"{copper} Copper");
            }
            Console.WriteLine($"Kamu mendapatkan {newAmount}.");
        }

        public (int Gold, int Silver, int Copper) ConvertCopper(int copper)
        {
            int gold = copper / 10000;
            copper %= 10000;
            int silver = copper / 100;
            copper %= 100;
            return (gold, silver, copper);
        }

        private void CheckLevelUp()
        {
            if (Experience >= ((Level * 100) + (Health * Level / 2)))
            {
                Level++;
                Experience -= ((Level - 1) * 100) + (Health * Level / 2);
                Health = Health + ((Health * 15 / 100) + Level);
                Attack += 2;
                Defense += 1;
                CriticalChance = Level % 2 == 1 ? CriticalChance + 0.1 : CriticalChance;
                CriticalHit = Level % 2 == 0 ? CriticalHit + 0.2 : CriticalHit;
                Console.WriteLine($"Level Up! Kamu sekarang level {Level}.");
            }
        }

        public void UsePotion()
        {
            Item potion = Inventory.Find(item => item is Potion);
            if (potion != null)
            {
                int healAmount = ((Potion)potion).HealAmount;
                Health = Health + healAmount;
                Inventory.Remove(potion);
                Console.WriteLine($"Kamu menggunakan potion dan memulihkan {healAmount} HP.");
            }
            else
            {
                Console.WriteLine("Kamu tidak memiliki potion.");
            }
        }
        public void UseRuneStone()
        {
            Item runeStone = Inventory.Find(item => item is RuneStone);
            if (runeStone != null)
            {
                var attackBonus = ((RuneStone)runeStone).AttackBonus;
                var defenseBonus = ((RuneStone)runeStone).DefenseBonus;
                var criticalChanceBonus = ((RuneStone)runeStone).CriticalChanceBonus;
                Attack = Attack + attackBonus;
                Defense = Defense + defenseBonus;
                CriticalChance = CriticalChance + criticalChanceBonus;
                var improvement = string.Empty;
                if (attackBonus != 0)
                {
                    improvement += $"Attack +{attackBonus} ";
                }
                if (defenseBonus != 0)
                {
                    improvement += $"Defense +{defenseBonus} ";
                }
                if (criticalChanceBonus != 0)
                {
                    improvement += $"Critical Chance +{criticalChanceBonus}%";
                }
                Inventory.Remove(runeStone);
                Console.WriteLine($"Kamu menggunakan {((RuneStone)runeStone).Name} dan meningkatkan {improvement}");
            }
            else
            {
                Console.WriteLine("Kamu tidak memiliki Rune Stone.");
            }
        }

        public void SaveToJson()
        {
            PlayerData data = new PlayerData
            {
                Name = Name,
                Level = Level,
                Experience = Experience,
                Gold = Gold,
                Health = Health,
                Attack = Attack,
                Defense = Defense,
                Deaths = Deaths,
                CriticalHit = CriticalHit,
                CriticalChance = CriticalChance,
                Inventory = Inventory.Select(item => new ItemData
                {
                    Name = item.Name,
                    Price = item.Price,
                    Status = item.Status,
                    IsEquipped = item.IsEquipped,
                    Type = item.GetType().Name
                }).ToList()
            };

            string fileName = $"{Name}.json";
            string jsonString = JsonSerializer.Serialize(data);
            File.WriteAllText(fileName, jsonString);
        }

        public static Player LoadFromJson(string name, GameData gameData)
        {
            string fileName = $"{name}.json";
            if (File.Exists(fileName))
            {
                string jsonString = File.ReadAllText(fileName);
                PlayerData data = JsonSerializer.Deserialize<PlayerData>(jsonString);
                Player player = new Player(data.Name, data.Health, data.Attack, data.Defense, data.CriticalHit, data.CriticalChance, gameData)
                {
                    Level = data.Level,
                    Experience = data.Experience,
                    Gold = data.Gold,
                    Deaths = data.Deaths,
                    Attack = data.Attack,
                    Defense = data.Defense,
                    CriticalChance = data.CriticalChance,
                    CriticalHit = data.CriticalHit
                };

                foreach (var itemData in data.Inventory)
                {
                    Item item = null;
                    switch (itemData.Type)
                    {
                        case "Weapon":
                            item = new Weapon(itemData.Name, itemData.Price, itemData.Status, itemData.Status); // Assume AttackBonus
                            break;
                        case "Armor":
                            item = new Armor(itemData.Name, itemData.Price, itemData.Status, itemData.Status); // Assume DefenseBonus
                            break;
                        case "Potion":
                            item = new Potion(itemData.Name, itemData.Price, itemData.Status, itemData.Status); // Assume HealAmount
                            break;
                        case "RuneStone":
                            int AtkBonus = 0;
                            int DefBonus = 0;
                            double CritChanceBonus = 0;
                            foreach (var dataRunestone in RuneStones)
                            {
                                if (dataRunestone.Name == itemData.Name)
                                {
                                    AtkBonus = dataRunestone.AttackBonus;
                                    DefBonus = dataRunestone.DefenseBonus;
                                    CritChanceBonus = dataRunestone.CriticalChanceBonus;

                                }
                            }
                            item = new RuneStone(itemData.Name, itemData.Price, 0, AtkBonus, DefBonus, CritChanceBonus); // Assume Rune Stone Bonus
                            break;
                    }

                    if (item != null)
                    {
                        item.IsEquipped = itemData.IsEquipped;
                        player.Inventory.Add(item);
                        if (item.IsEquipped)
                        {
                            if (item is Weapon weapon)
                            {
                                player.EquippedWeapon = weapon;
                                player.Attack += weapon.AttackBonus;
                            }
                            else if (item is Armor armor)
                            {
                                player.EquippedArmor = armor;
                                player.Defense += armor.DefenseBonus;
                            }
                        }
                    }
                }

                return player;
            }
            return null;
        }
        //public void OpenInventory()
        //{
        //    Console.WriteLine("\nInventori:");
        //    for (int i = 0; i < Inventory.Count; i++)
        //    {
        //        string equippedStatus = Inventory[i].IsEquipped ? " (Digunakan)" : "";
        //        Console.WriteLine($"{i + 1}. {Inventory[i].Name}{equippedStatus}");
        //    }

        //    Console.WriteLine("Pilih item untuk digunakan (0 untuk kembali):");
        //    if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= Inventory.Count)
        //    {
        //        UseItem(Inventory[choice - 1]);
        //    }
        //}
        public static int GetIntegerInput(string prompt)
        {
            int value;
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out value))
                {
                    return value;
                }
                else
                {
                    Console.WriteLine("Inputan harus angka");
                }
            }
        }

        public void OpenInventory()
        {
            Console.WriteLine("Menu Inventory:");
            Console.WriteLine("1. Equip/Unequip");
            Console.WriteLine("2. Enhancement");

            int choice = GetIntegerInput("Masukkan pilihan Anda: ");

            switch (choice)
            {
                case 1:
                    DisplayEquipUnequipMenu();
                    break;
                case 2:
                    DisplayRefineMenu();
                    break;
                default:
                    Console.WriteLine("Opsi tidak valid.");
                    break;
            }
        }


        public void DisplayEquipUnequipMenu()
        {
            Console.WriteLine("\nInventori:");
            for (int i = 0; i < Inventory.Count; i++)
            {
                string statusInfo = GetStatusInfo(Inventory[i]);
                string equippedStatus = Inventory[i].IsEquipped ? " (Digunakan)" : "";
                Console.WriteLine($"{i + 1}. {Inventory[i].Name} [{statusInfo}] {equippedStatus}");
            }

            Console.WriteLine("Pilih item untuk digunakan atau dilepas (0 untuk kembali):");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= Inventory.Count)
            {
                if (Inventory[choice - 1].IsEquipped)
                {
                    UnequipItem(Inventory[choice - 1]);
                }
                else
                {
                    UseItem(Inventory[choice - 1]);
                }
            }
        }
        private void DisplayRefineMenu()
        {
            var (gold, silver, copper) = ConvertCopper(Gold);
            var newAmount = "";
            if (gold != 0)
            {
                newAmount += ($"{gold} Gold ");
            }
            if (silver != 0)
            {
                newAmount += $"{silver} Silver ";
            }
            if (copper != 0)
            {
                newAmount += ($"{copper} Copper");
            }
            Console.WriteLine($"Uang : {newAmount}.");
            Console.WriteLine("Enhancement Menu:");

            int index = 1;
            foreach (var item in Inventory)
            {
                if (item is Weapon weapon)
                {
                    int refineCost = weapon.Status * 25; // Biaya perbaikan berdasarkan statu
                    var (refineGold, refineSilver, refineCopper) = ConvertCopper(refineCost);
                    var amountRefineWeapon = "";
                    if (refineGold != 0)
                    {
                        amountRefineWeapon += ($"{refineGold} Gold ");
                    }
                    if (refineSilver != 0)
                    {
                        amountRefineWeapon += ($"{refineSilver} Silver ");
                    }
                    if (refineCopper != 0)
                    {
                        amountRefineWeapon += ($"{refineCopper} Copper");
                    }

                    var isEquiped = item.IsEquipped ? " (Digunakan)" : "";

                    Console.WriteLine($"{index}. {item.Name}{isEquiped} | Current Atk +{item.Status} Refine Cost : {amountRefineWeapon}.");
                    index++;
                }

                if (item is Armor armor)
                {
                    int refineCost = armor.Status * 25; // Biaya perbaikan berdasarkan status
                    var (refineGold, refineSilver, refineCopper) = ConvertCopper(refineCost);
                    var amountRefineArmor = "";
                    if (refineGold != 0)
                    {
                        amountRefineArmor += ($"{refineGold} Gold ");
                    }
                    if (refineSilver != 0)
                    {
                        amountRefineArmor += ($"{refineSilver} Silver ");
                    }
                    if (refineCopper != 0)
                    {
                        amountRefineArmor += ($"{refineCopper} Copper");
                    }

                    var isEquiped = item.IsEquipped ? " (Digunakan)" : "";
                    Console.WriteLine($"{index}. {item.Name}{isEquiped} | Current Def +{item.Status} Refine Cost : {amountRefineArmor}.");
                    index++;
                }
            }

            Console.Write("Pilih item untuk Refine (nomor): ");
            int selectedIndex = int.Parse(Console.ReadLine()) - 1;

            if (selectedIndex >= 0 && selectedIndex < Inventory.Count)
            {
                Item selected = Inventory[selectedIndex];
                // Panggil metode RefineWeapon atau RefineArmor sesuai dengan tipe item
                if (selected is Weapon)
                {
                    RefineWeapon((Weapon)selected);
                }
                else if (selected is Armor)
                {
                    RefineArmor((Armor)selected);
                }
            }
            else
            {
                Console.WriteLine("Pilihan tidak valid.");
            }
        }


        private string GetStatusInfo(Item item)
        {
            if (item is Weapon weapon)
            {
                return $"Attack +{weapon.AttackBonus}";
            }
            else if (item is Armor armor)
            {
                return $"Defense +{armor.DefenseBonus}";
            }
            else if (item is Potion potion)
            {
                return $"Healing +{potion.HealAmount}";
            }
            else if (item is RuneStone runeStone)
            {
                var improvement = string.Empty;
                if (runeStone.AttackBonus != 0)
                {
                    improvement += $"Attack +{runeStone.AttackBonus} ";
                }
                if (runeStone.DefenseBonus != 0)
                {
                    improvement += $"Defense +{runeStone.DefenseBonus} ";
                }
                if (runeStone.CriticalChanceBonus != 0)
                {
                    improvement += $"Critical Chance +{runeStone.CriticalChanceBonus}";
                }
                return improvement;
            }
            return "";
        }

        private void UnequipItem(Item item)
        {
            if (item is Weapon weapon && EquippedWeapon == weapon)
            {
                Attack -= weapon.AttackBonus;
                EquippedWeapon.IsEquipped = false;
                EquippedWeapon = null;
                Console.WriteLine($"Kamu melepas {item.Name}. Attack -{weapon.AttackBonus}");
            }
            else if (item is Armor armor && EquippedArmor == armor)
            {
                Defense -= armor.DefenseBonus;
                EquippedArmor.IsEquipped = false;
                EquippedArmor = null;
                Console.WriteLine($"Kamu melepas {item.Name}. Defense -{armor.DefenseBonus}");
            }
        }


        private void UseItem(Item item)
        {
            if (item is Weapon weapon)
            {
                if (EquippedWeapon != null)
                {
                    Attack -= EquippedWeapon.AttackBonus;
                    EquippedWeapon.IsEquipped = false;
                }
                EquippedWeapon = weapon;
                Attack += weapon.AttackBonus;
                weapon.IsEquipped = true;
                Console.WriteLine($"Kamu menggunakan {item.Name}. Attack +{weapon.AttackBonus}");
            }
            else if (item is Armor armor)
            {
                if (EquippedArmor != null)
                {
                    Defense -= EquippedArmor.DefenseBonus;
                    EquippedArmor.IsEquipped = false;
                }
                EquippedArmor = armor;
                Defense += armor.DefenseBonus;
                armor.IsEquipped = true;
                Console.WriteLine($"Kamu menggunakan {item.Name}. Defense +{armor.DefenseBonus}");
            }
            else if (item is Potion)
            {
                UsePotion();
            }
            else if (item is RuneStone)
            {
                UseRuneStone();
            }
        }

        public void DisplayStatus()
        {
            Console.WriteLine($"\nStatus {Name}:");
            Console.WriteLine($"Level: {Level}");
            Console.WriteLine($"HP: {Health}");

            int baseAttack = Attack - (EquippedWeapon?.AttackBonus ?? 0);
            Console.WriteLine($"Attack: {Attack} ({baseAttack} +" + (EquippedWeapon?.AttackBonus ?? 0) + ")");

            int baseDefense = Defense - (EquippedArmor?.DefenseBonus ?? 0);
            Console.WriteLine($"Defense: {Defense} ({baseDefense} +" + (EquippedArmor?.DefenseBonus ?? 0) + ")");
            Console.WriteLine($"Critical Hit: {CriticalHit}x Attack");
            Console.WriteLine($"Critical Chance: {CriticalChance}%");

            var expLimit = ((Level * 100) + (Health * Level / 2));
            Console.WriteLine($"Experience: {Experience} / {expLimit}");
            //Console.WriteLine($"Gold: {Gold}");

            var (gold, silver, copper) = ConvertCopper(Gold);
            var amount = "";
            if (gold != 0)
            {
                amount += ($"{gold} Gold ");
            }
            if (silver != 0)
            {
                amount += ($"{silver} Silver ");
            }
            if (copper != 0)
            {
                amount += ($"{copper} Copper");
            }
            Console.WriteLine($"Uang : {amount}.");
            Console.WriteLine($"Kematian: {Deaths} kali");

            Console.WriteLine($"Senjata: {(EquippedWeapon != null ? EquippedWeapon.Name : "Tidak ada")}");
            Console.WriteLine($"Armor: {(EquippedArmor != null ? EquippedArmor.Name : "Tidak ada")}");
        }

        public void RefineWeapon(Weapon weapon)
        {
            if (Inventory.Contains(weapon))
            {

                int refineCost = weapon.Status * 25; // Biaya perbaikan berdasarkan status
                if (Gold >= refineCost)
                {
                    Console.WriteLine($"Mencoba untuk peningkatan {weapon.Name}...");
                    Gold -= refineCost;
                    Random random = new Random();
                    // Menentukan hasil refine dengan peluang 70% keberhasilan
                    if (random.NextDouble() < 0.7)
                    {
                        // Refine berhasil, tambahkan Attack sesuai dengan peningkatan yang diinginkan
                        int increaseAmount = random.Next(1, Level < 10 ? 8 : Level); // Contoh: peningkatan antara 1 sampai Level Player
                        weapon.Status += increaseAmount;
                        weapon.Price = weapon.Price + (weapon.Price * 2 / 100);

                        Console.WriteLine($"Berhasil meningkatkan {weapon.Name}! Attack +{increaseAmount}");
                    }
                    else
                    {
                        // Refine gagal, tidak ada perubahan pada senjata
                        Console.WriteLine($"Maaf, refine {weapon.Name} gagal.");
                    }
                }
                else
                {
                    Console.WriteLine("Uang kamu tidak cukup untuk meningkatkan senjata ini.");
                }
            }
            else
            {
                Console.WriteLine("Senjata tidak ditemukan di dalam inventori.");
            }
        }

        public void RefineArmor(Armor armor)
        {
            if (Inventory.Contains(armor))
            {
                int refineCost = armor.Status * 25; // Biaya perbaikan berdasarkan status
                if (Gold >= refineCost)
                {
                    Console.WriteLine($"Mencoba untuk peningkatan {armor.Name}...");
                    Gold -= refineCost;
                    Random random = new Random();

                    //armor.Status = Convert.ToInt32(Math.Ceiling(armor.Status * 1.5)); // Meningkatkan status armor
                    //Console.WriteLine($"Armor {armor.Name} telah diperbaiki. Status sekarang: {armor.Status}. Gold tersisa: {Gold}");
                    if (random.NextDouble() < 0.7)
                    {
                        // Refine berhasil, tambahkan Attack sesuai dengan peningkatan yang diinginkan
                        int increaseAmount = random.Next(1, Level < 10 ? 8 : Level); // Contoh: peningkatan antara 1 sampai Level Player
                        armor.Status += increaseAmount;
                        armor.Price = armor.Price + (armor.Price * 2 / 100);

                        Console.WriteLine($"Berhasil meningkatkan {armor.Name}! Defense +{increaseAmount}");
                    }
                    else
                    {
                        // Refine gagal, tidak ada perubahan pada senjata
                        Console.WriteLine($"Maaf, refine {armor.Name} gagal.");
                    }
                }
                else
                {
                    Console.WriteLine("Uang kamu tidak cukup untuk meningkatkan armor ini.");
                }
            }
            else
            {
                Console.WriteLine("Armor tidak ditemukan di dalam inventori.");
            }
        }
    }
}
