using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace GameGabut.Model
{
    public class GameData
    {
        [JsonPropertyName("weapons")]
        public List<WeaponData> Weapons { get; set; } = new List<WeaponData>();
        [JsonPropertyName("armors")]
        public List<ArmorData> Armors { get; set; } = new List<ArmorData>();
        [JsonPropertyName("potions")]
        public List<PotionData> Potions { get; set; } = new List<PotionData>();
        [JsonPropertyName("enemies")]
        public List<EnemyData> Enemies { get; set; } = new List<EnemyData>();
        [JsonPropertyName("rune_stone")]
        public List<RuneStoneData> RuneStones { get; set; } = new List<RuneStoneData>();

        public static GameData LoadFromJson(string filePath)
        {
            try
            {
                string jsonString = File.ReadAllText(filePath);
                GameData gameData = new GameData();

                JsonDocument document = JsonDocument.Parse(jsonString);
                JsonElement root = document.RootElement;

                // Deserialize Weapons
                if (root.TryGetProperty("weapons", out JsonElement weaponsElement))
                {
                    WeaponData[] weaponArray = JsonSerializer.Deserialize<WeaponData[]>(weaponsElement.GetRawText());
                    gameData.Weapons.AddRange(weaponArray);
                }

                // Deserialize Armors
                if (root.TryGetProperty("armors", out JsonElement armorsElement))
                {
                    ArmorData[] armorArray = JsonSerializer.Deserialize<ArmorData[]>(armorsElement.GetRawText());
                    gameData.Armors.AddRange(armorArray);
                }

                // Deserialize Potions
                if (root.TryGetProperty("potions", out JsonElement potionsElement))
                {
                    PotionData[] potionArray = JsonSerializer.Deserialize<PotionData[]>(potionsElement.GetRawText());
                    gameData.Potions.AddRange(potionArray);
                }

                // Deserialize Enemies
                if (root.TryGetProperty("enemies", out JsonElement enemiesElement))
                {
                    EnemyData[] enemyArray = JsonSerializer.Deserialize<EnemyData[]>(enemiesElement.GetRawText());
                    gameData.Enemies.AddRange(enemyArray);
                }

                // Deserialize rune stone
                if (root.TryGetProperty("rune_stones", out JsonElement runeStoneElement))
                {
                    RuneStoneData[] runeStoneArray = JsonSerializer.Deserialize<RuneStoneData[]>(runeStoneElement.GetRawText());
                    gameData.RuneStones.AddRange(runeStoneArray);
                }

                return gameData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading GameData from JSON: {ex.Message}");
                return new GameData();
            }
        }
    }

    public class WeaponData
    {
        
        public string Name { get; set; }
        public int Price { get; set; }
        public int AttackBonus { get; set; }
    }

    public class ArmorData
    {

        public string Name { get; set; }
        public int Price { get; set; }
        public int DefenseBonus { get; set; }
    }
    public class RuneStoneData
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }
        public double CriticalChanceBonus { get; set; }
    }

    public class PotionData
    {
        
        public string Name { get; set; }
        public int Price { get; set; }
        public int HealAmount { get; set; }
    }

    public class EnemyData
    {
        
        public string Name { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public double CriticalChance { get; set; }
        public double CriticalHit { get; set; }
        public int ExpReward { get; set; }
        public int GoldReward { get; set; }
    }

}
