namespace BAGEBI.Data;

public class Kindergarten
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<User> Users { get; set; } = new();
    public List<DailyAttendance> DailyAttendances { get; set; } = new();
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "KindergartenUser"; // "Admin" or "KindergartenUser"
    public int? KindergartenId { get; set; }
    public Kindergarten? Kindergarten { get; set; }
}

/// <summary>
/// Daily attendance record: one row per kindergarten per calendar day.
/// Stores only the total head count — no individual children.
/// </summary>
public class DailyAttendance
{
    public int Id { get; set; }
    public int KindergartenId { get; set; }
    public Kindergarten? Kindergarten { get; set; }
    public string KindergartenName { get; set; } = string.Empty;

    public DateTime Date { get; set; }   // date part only (year/month/day)
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }

    public int ChildrenCount { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}
