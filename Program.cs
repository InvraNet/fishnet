using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Program
{
    static Dictionary<string, decimal> fishDatabase = new Dictionary<string, decimal>
    {
        { "salmon", 10.0m },
        { "trout", 5.0m },
        { "tuna", 20.0m },
        { "cod", 8.0m },
        { "miyofuji", 20.0m}
    };

    static List<string> inventory = new List<string>();
    static decimal money = 0m;
    static string saveFilePath;

    static void Main(string[] args)
    {
        string appDataFolder = GetAppDataFolder();
        string saveFolder = Path.Combine(appDataFolder, "fishnet", "save");
        Directory.CreateDirectory(saveFolder);
        saveFilePath = Path.Combine(saveFolder, "save00.json");
        LoadGame();

        if (args.Length == 0)
        {
            Console.WriteLine("You need to provide an argument.");
            Console.WriteLine("Run fishnet help for help.");
        }
        else if (args[0] == "help")
        {
            Console.WriteLine("\nFishNET help");
            Console.WriteLine("fish: Fish for fish stuff.");
            Console.WriteLine("sell: Sell all fish in inventory.");
            Console.WriteLine("inventory: View what is in your inventory.");
            Console.WriteLine("stats: View your current stats.");
        }
        else if (args[0] == "fish")
        {
            Fish();
        }
        else if (args[0] == "sell")
        {
            Sell();
        }
        else if (args[0] == "inventory")
        {
            Inventory();
        }
        else if (args[0] == "stats")
        {
            Stats();
        }
        else
        {
            Console.WriteLine("The command specified does not exist.");
        }
        SaveGame();
    }

    static string GetAppDataFolder()
    {
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (OperatingSystem.IsMacOS())
        {
            appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Application Support");
        }

        return appDataFolder;
    }

    static void Fish()
    {
        Random rand = new Random();
        var fishTypes = new List<string>(fishDatabase.Keys);
        string caughtFish = fishTypes[rand.Next(fishTypes.Count)];
        inventory.Add(caughtFish);
        Console.WriteLine($"You caught a {caughtFish}!");
    }

    static void Sell()
    {
        if (inventory.Count == 0)
        {
            Console.WriteLine("Your inventory is empty.");
            return;
        }

        decimal totalValue = 0;
        foreach (var fish in inventory)
        {
            if (fishDatabase.TryGetValue(fish, out var price))
            {
                totalValue += price;
            }
        }

        inventory.Clear();
        money += totalValue;
        Console.WriteLine($"You sold all your fish for ${totalValue:F2}. Your new balance is ${money:F2}.");
    }

    static void Inventory()
    {
        if (inventory.Count == 0)
        {
            Console.WriteLine("Your inventory is empty.");
            return;
        }

        Console.WriteLine("Your inventory:");
        var fishCounts = new Dictionary<string, int>();
        foreach (var fish in inventory)
        {
            if (fishCounts.ContainsKey(fish))
            {
                fishCounts[fish]++;
            }
            else
            {
                fishCounts[fish] = 1;
            }
        }

        foreach (var fish in fishCounts)
        {
            Console.WriteLine($"{fish.Key}: {fish.Value}");
        }
    }

    static void Stats()
    {
        Console.WriteLine($"Current Balance: ${money:F2}");
        Console.WriteLine($"Fish in Inventory: {inventory.Count}");
    }

    static void SaveGame()
    {
        var saveData = new SaveData
        {
            Inventory = inventory,
            Money = money
        };

        string jsonString = JsonSerializer.Serialize(saveData);
        File.WriteAllText(saveFilePath, jsonString);
    }

    static void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string jsonString = File.ReadAllText(saveFilePath);
            var saveData = JsonSerializer.Deserialize<SaveData>(jsonString);

            if (saveData != null)
            {
                inventory = saveData.Inventory ?? new List<string>();
                money = saveData.Money;
            }
        }
    }

    class SaveData
    {
        public List<string> Inventory { get; set; }
        public decimal Money { get; set; }
    }
}