namespace BAGEBI.Services;

public class MenuProductNorm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = "კგ"; // "კგ", "გრამი", "ცალი", "ლიტრი"
    
    // Norms per child in grams or units for each day of the week (Mon-Fri)
    public decimal MondayNorm { get; set; }
    public decimal TuesdayNorm { get; set; }
    public decimal WednesdayNorm { get; set; }
    public decimal ThursdayNorm { get; set; }
    public decimal FridayNorm { get; set; }

    public decimal GetNormForDay(DayOfWeek dayOfWeek) => dayOfWeek switch
    {
        DayOfWeek.Monday => MondayNorm,
        DayOfWeek.Tuesday => TuesdayNorm,
        DayOfWeek.Wednesday => WednesdayNorm,
        DayOfWeek.Thursday => ThursdayNorm,
        DayOfWeek.Friday => FridayNorm,
        _ => 0m
    };
}

public class FoodCalculationResult
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal NormPerChild { get; set; }
    public int ChildrenCount { get; set; }
    public decimal TotalQuantityGrams { get; set; } // Raw grams / units
    public decimal TotalQuantityDisplay { get; set; } // Converted to kg / liters / units for easy reading
    public string DisplayString { get; set; } = string.Empty;
}

public class MenuCalculationService
{
    public static readonly List<MenuProductNorm> Products = new()
    {
        new MenuProductNorm { Id = 1,  Name = "შაქარი", Unit = "კგ", MondayNorm = 10, TuesdayNorm = 10, WednesdayNorm = 20, ThursdayNorm = 20, FridayNorm = 30 },
        new MenuProductNorm { Id = 2,  Name = "მაკარონი", Unit = "კგ", MondayNorm = 35, TuesdayNorm = 0, WednesdayNorm = 0, ThursdayNorm = 15, FridayNorm = 0 },
        new MenuProductNorm { Id = 3,  Name = "მარილი", Unit = "კგ", MondayNorm = 1.3m, TuesdayNorm = 1.1m, WednesdayNorm = 1.0m, ThursdayNorm = 0.8m, FridayNorm = 1.0m },
        new MenuProductNorm { Id = 4,  Name = "პურის ფქვილი", Unit = "კგ", MondayNorm = 0, TuesdayNorm = 42, WednesdayNorm = 40, ThursdayNorm = 2, FridayNorm = 14 },
        new MenuProductNorm { Id = 5,  Name = "ჩაი", Unit = "კგ", MondayNorm = 0, TuesdayNorm = 0.2m, WednesdayNorm = 0.2m, ThursdayNorm = 0.2m, FridayNorm = 0.2m },
        new MenuProductNorm { Id = 6,  Name = "პეჩენია / ორცხობილა", Unit = "კგ", MondayNorm = 0, TuesdayNorm = 0, WednesdayNorm = 40, ThursdayNorm = 0, FridayNorm = 40 },
        new MenuProductNorm { Id = 7,  Name = "ჭარხალი", Unit = "კგ", MondayNorm = 0, TuesdayNorm = 10, WednesdayNorm = 0, ThursdayNorm = 0, FridayNorm = 10 },
        new MenuProductNorm { Id = 8,  Name = "ნიორი", Unit = "კგ", MondayNorm = 2.5m, TuesdayNorm = 0.5m, WednesdayNorm = 2.5m, ThursdayNorm = 2.5m, FridayNorm = 0.5m },
        new MenuProductNorm { Id = 9,  Name = "მანანის ბურღული", Unit = "კგ", MondayNorm = 0, TuesdayNorm = 0, WednesdayNorm = 0, ThursdayNorm = 0, FridayNorm = 20 },
        new MenuProductNorm { Id = 10, Name = "ტომატი", Unit = "კგ", MondayNorm = 2, TuesdayNorm = 2, WednesdayNorm = 2, ThursdayNorm = 2, FridayNorm = 2 },
        new MenuProductNorm { Id = 11, Name = "კვერცხი", Unit = "ცალი", MondayNorm = 0, TuesdayNorm = 0.1m, WednesdayNorm = 0.1m, ThursdayNorm = 0.6m, FridayNorm = 0.3m },
        new MenuProductNorm { Id = 12, Name = "საფუარი", Unit = "კგ", MondayNorm = 0, TuesdayNorm = 0, WednesdayNorm = 1, ThursdayNorm = 0, FridayNorm = 0 },
        new MenuProductNorm { Id = 13, Name = "ბრინჯი", Unit = "კგ", MondayNorm = 10, TuesdayNorm = 0, WednesdayNorm = 0, ThursdayNorm = 25, FridayNorm = 0 },
        new MenuProductNorm { Id = 14, Name = "ხახვი", Unit = "კგ", MondayNorm = 15, TuesdayNorm = 10, WednesdayNorm = 15, ThursdayNorm = 10, FridayNorm = 10 },
        new MenuProductNorm { Id = 15, Name = "კარტოფილი", Unit = "კგ", MondayNorm = 120, TuesdayNorm = 160, WednesdayNorm = 140, ThursdayNorm = 160, FridayNorm = 160 },
        new MenuProductNorm { Id = 16, Name = "კომბოსტო", Unit = "კგ", MondayNorm = 0, TuesdayNorm = 65, WednesdayNorm = 0, ThursdayNorm = 0, FridayNorm = 65 },
        new MenuProductNorm { Id = 17, Name = "სტაფილო", Unit = "კგ", MondayNorm = 35, TuesdayNorm = 10, WednesdayNorm = 35, ThursdayNorm = 35, FridayNorm = 10 },
        new MenuProductNorm { Id = 18, Name = "წიწიბურა", Unit = "კგ", MondayNorm = 25, TuesdayNorm = 0, WednesdayNorm = 0, ThursdayNorm = 30, FridayNorm = 0 },
        new MenuProductNorm { Id = 19, Name = "ხმელი სუნელი", Unit = "კგ", MondayNorm = 0.3m, TuesdayNorm = 0.3m, WednesdayNorm = 0.3m, ThursdayNorm = 0.3m, FridayNorm = 0.3m },
        new MenuProductNorm { Id = 20, Name = "ლობიო", Unit = "კგ", MondayNorm = 0, TuesdayNorm = 0, WednesdayNorm = 40, ThursdayNorm = 0, FridayNorm = 0 },
        new MenuProductNorm { Id = 21, Name = "ვაშლი", Unit = "კგ", MondayNorm = 0, TuesdayNorm = 0, WednesdayNorm = 30, ThursdayNorm = 0, FridayNorm = 0 },
        new MenuProductNorm { Id = 22, Name = "ზეთი", Unit = "ლიტრი", MondayNorm = 7, TuesdayNorm = 7, WednesdayNorm = 7, ThursdayNorm = 12, FridayNorm = 10 },
        new MenuProductNorm { Id = 23, Name = "კარაქი", Unit = "კგ", MondayNorm = 8, TuesdayNorm = 4, WednesdayNorm = 14, ThursdayNorm = 8, FridayNorm = 11 },
        new MenuProductNorm { Id = 24, Name = "ყველი", Unit = "კგ", MondayNorm = 15, TuesdayNorm = 15, WednesdayNorm = 20, ThursdayNorm = 0, FridayNorm = 0 },
        new MenuProductNorm { Id = 25, Name = "რძე ნატურალური პასტერიზებული", Unit = "ლიტრი", MondayNorm = 60, TuesdayNorm = 5, WednesdayNorm = 0, ThursdayNorm = 70, FridayNorm = 65 },
        new MenuProductNorm { Id = 26, Name = "ხორცი", Unit = "კგ", MondayNorm = 70, TuesdayNorm = 70, WednesdayNorm = 0, ThursdayNorm = 70, FridayNorm = 0 },
        new MenuProductNorm { Id = 27, Name = "ქათმის ფილე", Unit = "კგ", MondayNorm = 0, TuesdayNorm = 0, WednesdayNorm = 0, ThursdayNorm = 0, FridayNorm = 60 },
        new MenuProductNorm { Id = 28, Name = "პური", Unit = "კგ", MondayNorm = 70, TuesdayNorm = 80, WednesdayNorm = 110, ThursdayNorm = 110, FridayNorm = 110 },
    };

    public static decimal RoundEggQuantity(decimal totalEggs)
    {
        decimal whole = Math.Floor(totalEggs);
        decimal fraction = totalEggs - whole;
        if (fraction >= 0.55m)
        {
            return whole + 1m;
        }
        return whole;
    }

    public List<FoodCalculationResult> CalculateFoodForChildren(int childrenCount, DayOfWeek dayOfWeek)
    {
        var list = new List<FoodCalculationResult>();

        foreach (var p in Products)
        {
            decimal norm = p.GetNormForDay(dayOfWeek);
            if (norm <= 0) continue; // Skip products not served on this day

            decimal totalGramsOrUnits = childrenCount * norm;
            if (p.Unit == "ცალი" || p.Name == "კვერცხი")
            {
                totalGramsOrUnits = RoundEggQuantity(totalGramsOrUnits);
            }

            decimal totalDisplay = p.Unit == "ცალი" ? totalGramsOrUnits : totalGramsOrUnits / 1000m;

            string displayStr = p.Unit == "ცალი" 
                ? $"{totalGramsOrUnits:N0} ცალი" 
                : $"{totalDisplay:N3} {p.Unit}";

            list.Add(new FoodCalculationResult
            {
                ProductId = p.Id,
                ProductName = p.Name,
                Unit = p.Unit,
                NormPerChild = norm,
                ChildrenCount = childrenCount,
                TotalQuantityGrams = totalGramsOrUnits,
                TotalQuantityDisplay = totalDisplay,
                DisplayString = displayStr
            });
        }

        return list;
    }
}
