using GameGabut.Interface;
using GameGabut.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameGabut
{
    class Shop
    {
        private List<Item> items;
        private Player player;

        public Shop(GameData gameData, Player player)
        {
            items = new List<Item>();
            this.player = player;
            items.AddRange(gameData.Weapons.Select(w => new Weapon(w.Name, w.Price, w.AttackBonus, w.AttackBonus)));
            items.AddRange(gameData.Armors.Select(a => new Armor(a.Name, a.Price, a.DefenseBonus, a.DefenseBonus)));
            items.AddRange(gameData.Potions.Select(p => new Potion(p.Name, p.Price, p.HealAmount, p.HealAmount)));
        }
        public void Visit()
        {
            Console.WriteLine("\nSelamat datang di Toko!");

            var (gold, silver, copper) = player.ConvertCopper(player.Gold);
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
            Console.WriteLine($"Uang Anda: {amount}");
            while (true)
            {
                Console.WriteLine("\nPilihan:");
                Console.WriteLine("1. Beli barang");
                Console.WriteLine("2. Jual barang");
                Console.WriteLine("0. Keluar dari toko");

                int choice = GetIntegerInput("Masukkan pilihan Anda: ");

                if (choice == 0) { player.SaveToJson(); break; }
                else if (choice == 1) BuyItem();
                else if (choice == 2) SellItem();
                else
                {
                    Console.WriteLine("Pilihan tidak valid.");
                }
            }
        }
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
        private void BuyItem()
        {
            // Kode untuk membeli barang (sama seperti sebelumnya)
            while (true)
            {
                Console.WriteLine("\nBarang yang tersedia:");
                for (int i = 0; i < items.Count; i++)
                {
                    var (gold, silver, copper) = player.ConvertCopper(items[i].Price);
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
                    Console.WriteLine($"{i + 1}. {items[i].Name} [+{items[i].Status}] - {amount}");
                }

                Console.WriteLine("0. Kembali");
                Console.WriteLine("");

                var (gold1, silver1, copper1) = player.ConvertCopper(player.Gold);
                var newAmount = "";
                if (gold1 != 0)
                {
                    newAmount += ($"{gold1} Gold ");
                }
                if (silver1 != 0)
                {
                    newAmount += ($"{silver1} Silver ");
                }
                if (copper1 != 0)
                {
                    newAmount += ($"{copper1} Copper");
                }
                Console.WriteLine($"Uang Anda: {newAmount}");

                int choice = GetIntegerInput("Pilih barang yang ingin dibeli: ");

                if (choice == 0) break;

                if (choice > 0 && choice <= items.Count)
                {
                    Item selectedItem = items[choice - 1];
                    if (player.Gold >= selectedItem.Price)
                    {
                        Console.WriteLine("");
                        player.Inventory.Add(selectedItem);
                        player.GainGold(-selectedItem.Price);
                        Console.WriteLine($"Kamu membeli {selectedItem.Name}.");
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Gold tidak cukup.");
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("Pilihan tidak valid.");
                }
            }
        }

        private void SellItem()
        {
            Console.WriteLine("\nBarang yang ingin Anda jual:");
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                Item item = player.Inventory[i];
                string equippedStatus = player.IsItemEquipped(item) ? "(Sedang digunakan)" : "";
                var (gold, silver, copper) = player.ConvertCopper(item.Price);
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
                Console.WriteLine($"{i + 1}. {item.Name} - {amount} {equippedStatus}");
            }

            int choice = GetIntegerInput("Pilih barang yang ingin dijual: ");

            if (choice > 0 && choice <= player.Inventory.Count)
            {
                Item itemToSell = player.Inventory[choice - 1];
                if (player.IsItemEquipped(itemToSell))
                {
                    Console.WriteLine("Anda tidak dapat menjual barang yang sedang digunakan.");
                }
                else
                {
                    int soldPrice = player.Sell(itemToSell);
                    if (soldPrice > 0)
                    {
                        var (gold, silver, copper) = player.ConvertCopper(soldPrice);
                        var soldAmount = "";
                        if (gold != 0)
                        {
                            soldAmount += ($"{gold} Gold ");
                        }
                        if (silver != 0)
                        {
                            soldAmount += ($"{silver} Silver ");
                        }
                        if (copper != 0)
                        {
                            soldAmount += ($"{copper} Copper");
                        }
                        Console.WriteLine($"Kamu menjual {itemToSell.Name} seharga {soldAmount}.");

                        var (gold1, silver1, copper1) = player.ConvertCopper(player.Gold);
                        var newAmount = "";
                        if (gold1 != 0)
                        {
                            newAmount += ($"{gold1} Gold ");
                        }
                        if (silver1 != 0)
                        {
                            newAmount += ($"{silver1} Silver ");
                        }
                        if (copper1 != 0)
                        {
                            newAmount += ($"{copper1} Copper");
                        }
                        Console.WriteLine($"Uang Anda: {newAmount}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Pilihan tidak valid.");
            }
        }
    }
}
