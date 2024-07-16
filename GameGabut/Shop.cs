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
            Console.WriteLine($"Gold Anda: {player.Gold}");
            while (true)
            {
                Console.WriteLine("\nPilihan:");
                Console.WriteLine("1. Beli barang");
                Console.WriteLine("2. Jual barang");
                Console.WriteLine("0. Keluar dari toko");
                Console.Write("Masukkan pilihan Anda: ");

                int choice = int.Parse(Console.ReadLine());

                if (choice == 0) { player.SaveToJson(); break; }
                else if (choice == 1) BuyItem();
                else if (choice == 2) SellItem();
                else
                {
                    Console.WriteLine("Pilihan tidak valid.");
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
                    Console.WriteLine($"{i + 1}. {items[i].Name} [+{items[i].Status}] - {items[i].Price} Gold");
                }

                Console.WriteLine("0. Kembali");
                Console.WriteLine("");
                Console.WriteLine($"Gold Anda: {player.Gold}");
                Console.Write("Pilih barang yang ingin dibeli:");

                int choice = int.Parse(Console.ReadLine());

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
                Console.WriteLine($"{i + 1}. {item.Name} - {item.Price} Gold {equippedStatus}");
            }

            Console.Write("Pilih barang yang ingin dijual: ");
            int choice = int.Parse(Console.ReadLine());

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
                        Console.WriteLine($"Kamu menjual {itemToSell.Name} seharga {soldPrice} gold.");
                        Console.WriteLine($"Gold Anda: {player.Gold}");
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
