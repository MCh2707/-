using BAGEBI.Services;
using Microsoft.EntityFrameworkCore;

namespace BAGEBI.Data;

public static class DbInitializer
{
    public static readonly string[] OfficialNames = new string[]
    {
        "N1 საბავშვო ბაღი",
        "N 2 საბავშვო ბაღი",
        "N 3 საბავშვო ბაღი",
        "N 4 საბავშვო ბაღი",
        "N 5 საბავშვო ბაღი",
        "N 6 საბავშვო ბაღი",
        "N 7 საბავშვო ბაღი",
        "N 8 საბავშვო ბაღი",
        "N 9 საბავშვო ბაღი",
        "N 10 საბავშვო ბაღი",
        "N 11 საბავშვო ბაღი",
        "N 12 საბავშვო ბაღი",
        "N 13 საბავშვო ბაღი",
        "N 14 საბავშვო ბაღი",
        "N 15 საბავშვო ბაღი",
        "N 16 საბავშვო ბაღი",
        "N 17 საბავშვო ბაღი",
        "N 18 საბავშვო ბაღი",
        "აბასთუმნის ს/ბაღი",
        "ანაკლიის ს/ბაღი",
        "ახალაბასთუმნის ს/ბაღი",
        "ახალკახათის ს/ბაღი",
        "ახალსოფლის ს/ბაღი",
        "ბაშის ს/ბაღი",
        "განმუხურის ს/ბაღი",
        "გრიგოლიშის ს/ბაღი",
        "დარჩელის  N1 ს/ბაღი",
        "დარჩელის  N2 ს/ბაღი",
        "დიდინეძის ს/ბაღი",
        "ერგეტის ს/ბაღი",
        "ზედაეწერის ს/ბაღი",
        "ინგირის N1 ს/ბაღი",
        "ინგირის N2 ს/ბაღი",
        "ინგირის N3 ს/ბაღი",
        "კახათის ,,რწმენა\" ს/ბაღი",
        "კორცხელის ს/ბაღი",
        "კოკის ს/ბაღი",
        "ნარაზენის N1 ს/ბაღი",
        "ნარაზენის N2 ს/ბაღი",
        "ნაცატუს ს/ბაღი",
        "ნაწულუკუს ს/ბაღი",
        "ოდიშის ,,ოდიში\" ს/ბაღი",
        "ოირემეს ს/ბაღი",
        "ორსანტიის ს/ბაღი",
        "ორულუს ს/ბაღი",
        "ოქტომბრის ს/ბაღი",
        "რიყის ს/ბაღი",
        "რუხის ს/ბაღი",
        "ტყაიის ს/ბაღი",
        "ურთის ს/ბაღი",
        "ქვემოკახათის ს/ბაღი",
        "ყულიშკარის ს/ბაღი",
        "შამგონის ს/ბაღი",
        "ჩხოუშის ს/ბაღი",
        "ჩხორიის ს/ბაღი",
        "ცაცხვის ს/ბაღი",
        "ჭაქვინჯის N1 ს/ბაღი",
        "ჭაქვინჯის N2 ს/ბაღი",
        "ჭითაწყარის ს/ბაღი",
        "ჭკადუაშის ს/ბაღი",
        "ჯიხაშკარის ს/ბაღი",
        "ჯუმის ს/ბაღი",
        "ხურჩის ს/ბაღი"
    };

    public static void Initialize(AppDbContext context)
    {
        // Ensure database tables exist
        bool tableExists = true;
        try
        {
            context.Database.EnsureCreated();
            _ = context.DailyAttendances.FirstOrDefault();
        }
        catch
        {
            tableExists = false;
        }

        if (!tableExists)
        {
            try
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            catch
            {
                context.Database.EnsureCreated();
            }
        }

        if (!context.Kindergartens.Any())
        {
            // Create 63 Kindergartens with official Georgian names
            var kindergartens = new List<Kindergarten>();
            for (int i = 1; i <= 63; i++)
            {
                string name = i <= OfficialNames.Length ? OfficialNames[i - 1] : $"Bagebi N{i}";
                kindergartens.Add(new Kindergarten { Name = name });
            }
            context.Kindergartens.AddRange(kindergartens);
            context.SaveChanges();

            // Create Admin user
            context.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = AuthService.HashPassword("admin123"),
                Role = "Admin",
                KindergartenId = null
            });

            // Create 63 Kindergarten users (bagebi1 .. bagebi63 / bagebi123)
            var defaultHash = AuthService.HashPassword("bagebi123");
            for (int i = 1; i <= 63; i++)
            {
                context.Users.Add(new User
                {
                    Username = $"bagebi{i}",
                    PasswordHash = defaultHash,
                    Role = "KindergartenUser",
                    KindergartenId = kindergartens[i - 1].Id
                });
            }

            context.SaveChanges();
        }
        else
        {
            // Update existing Kindergartens to set official Georgian display names if needed
            var existingKgs = context.Kindergartens.OrderBy(k => k.Id).ToList();
            bool changed = false;
            for (int i = 0; i < existingKgs.Count; i++)
            {
                if (i < OfficialNames.Length)
                {
                    string official = OfficialNames[i];
                    if (existingKgs[i].Name != official)
                    {
                        existingKgs[i].Name = official;
                        changed = true;
                    }
                }
            }
            if (changed)
            {
                context.SaveChanges();

                // Also update stored KindergartenName in DailyAttendances
                var allAttendances = context.DailyAttendances.ToList();
                var kgMap = existingKgs.ToDictionary(k => k.Id, k => k.Name);
                foreach (var att in allAttendances)
                {
                    if (kgMap.TryGetValue(att.KindergartenId, out var newName))
                    {
                        att.KindergartenName = newName;
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
