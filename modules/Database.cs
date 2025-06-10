using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace InfixBot.modules
{
    public static class Database
    {
        private static readonly string DB_FILE = "blockchain_users.json";

        public class Wallet
        {
            public long UserId { get; set; }
            public string Username { get; set; }
            public string Address { get; set; }
            public string Secret { get; set; }
            public string CreatedAt { get; set; }
        }

        public static List<Wallet> GetUserWallet(long userId)
        {
            var db = LoadDb();
            return db.Where(w => w .UserId == userId).ToList();
        }

        public static void Insert(Wallet wallet)
        {
            var db = LoadDb();
            db.Add(wallet);
            SaveDb(db);
        }

        public static void Remove(long userId)
        {
            var db = LoadDb();
            db.RemoveAll(w => w.UserId == userId);
            SaveDb(db);
        }

        private static List<Wallet> LoadDb()
        {
            if (File.Exists(DB_FILE))
            {
                var json = File.ReadAllText(DB_FILE);
                return JsonSerializer.Deserialize<List<Wallet>>(json) ?? new List<Wallet>();
            }
            return new List<Wallet>();
        }

        private static void SaveDb(List<Wallet> db)
        {
            var json = JsonSerializer.Serialize(db, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(DB_FILE, json);
        }
    }
}
