using BAGEBI.Data;
using Microsoft.EntityFrameworkCore;

namespace BAGEBI.Services;

public class AttendanceService
{
    private readonly AppDbContext _context;

    public AttendanceService(AppDbContext context)
    {
        _context = context;
    }

    // ─── Kindergarten list ──────────────────────────────────────────────────

    public async Task<List<Kindergarten>> GetAllKindergartensAsync()
        => await _context.Kindergartens.OrderBy(k => k.Id).ToListAsync();

    public async Task<Kindergarten?> GetKindergartenByIdAsync(int id)
        => await _context.Kindergartens.FirstOrDefaultAsync(k => k.Id == id);

    // ─── Save / Update daily attendance count ───────────────────────────────

    public async Task SaveDailyAttendanceAsync(int kindergartenId, DateTime date, int childrenCount, User currentUser)
    {
        if (currentUser.Role != "Admin" && currentUser.KindergartenId != kindergartenId)
            throw new UnauthorizedAccessException("თქვენ არ გაქვთ ამ ჩანაწერის შეცვლის უფლება.");

        var d = date.Date;
        var kg = await _context.Kindergartens.FindAsync(kindergartenId)
                 ?? throw new KeyNotFoundException("Kindergarten not found.");

        var existing = await _context.DailyAttendances
            .FirstOrDefaultAsync(a => a.KindergartenId == kindergartenId
                                   && a.Year == d.Year
                                   && a.Month == d.Month
                                   && a.Day == d.Day);

        if (existing == null)
        {
            _context.DailyAttendances.Add(new DailyAttendance
            {
                KindergartenId = kindergartenId,
                KindergartenName = kg.Name,
                Date = d,
                Year = d.Year,
                Month = d.Month,
                Day = d.Day,
                ChildrenCount = childrenCount,
                RecordedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.ChildrenCount = childrenCount;
            existing.RecordedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int?> GetTodayCountAsync(int kindergartenId)
    {
        var today = DateTime.Today;
        var rec = await _context.DailyAttendances
            .FirstOrDefaultAsync(a => a.KindergartenId == kindergartenId
                                   && a.Year == today.Year
                                   && a.Month == today.Month
                                   && a.Day == today.Day);
        return rec?.ChildrenCount;
    }

    // ─── Admin queries ───────────────────────────────────────────────────────

    public async Task<List<DailyAttendance>> GetRecordsForDateAsync(DateTime date)
    {
        var d = date.Date;
        return await _context.DailyAttendances
            .Include(a => a.Kindergarten)
            .Where(a => a.Year == d.Year && a.Month == d.Month && a.Day == d.Day)
            .OrderBy(a => a.KindergartenId)
            .ToListAsync();
    }

    public async Task<List<DailyAttendance>> GetRecordsForDateRangeAsync(DateTime startDate, DateTime endDate, int? kindergartenId = null)
    {
        var start = startDate.Date;
        var end = endDate.Date;
        if (end < start)
        {
            (start, end) = (end, start);
        }

        var q = _context.DailyAttendances
            .Include(a => a.Kindergarten)
            .Where(a => a.Date >= start && a.Date <= end);

        if (kindergartenId.HasValue)
        {
            int kgId = kindergartenId.Value;
            q = q.Where(a => a.KindergartenId == kgId);
        }

        return await q.OrderBy(a => a.Date).ThenBy(a => a.KindergartenId).ToListAsync();
    }

    public async Task<List<DailyAttendance>> GetRecordsForMonthAsync(int year, int month, int? kindergartenId = null)
    {
        var q = _context.DailyAttendances
            .Include(a => a.Kindergarten)
            .Where(a => a.Year == year && a.Month == month);

        if (kindergartenId.HasValue)
        {
            int kgId = kindergartenId.Value;
            q = q.Where(a => a.KindergartenId == kgId);
        }

        return await q.OrderBy(a => a.Day).ThenBy(a => a.KindergartenId).ToListAsync();
    }

    public async Task<List<MonthStat>> GetMonthlyStatsAsync(int year, int? kindergartenId = null)
    {
        var q = _context.DailyAttendances.Where(a => a.Year == year);

        if (kindergartenId.HasValue)
        {
            int kgId = kindergartenId.Value;
            q = q.Where(a => a.KindergartenId == kgId);
        }

        var raw = await q.Select(a => new { a.Month, a.ChildrenCount }).ToListAsync();

        return raw
            .GroupBy(a => a.Month)
            .Select(g => new MonthStat(g.Key, g.Sum(a => (long)a.ChildrenCount)))
            .OrderBy(s => s.Month)
            .ToList();
    }

    public async Task<List<YearStat>> GetYearlyStatsAsync(int? kindergartenId = null)
    {
        var q = _context.DailyAttendances.AsQueryable();

        if (kindergartenId.HasValue)
        {
            int kgId = kindergartenId.Value;
            q = q.Where(a => a.KindergartenId == kgId);
        }

        var raw = await q.Select(a => new { a.Year, a.ChildrenCount }).ToListAsync();

        return raw
            .GroupBy(a => a.Year)
            .Select(g => new YearStat(g.Key, g.Sum(a => (long)a.ChildrenCount)))
            .OrderBy(s => s.Year)
            .ToList();
    }

    // ─── Stat DTOs ───────────────────────────────────────────────────────────

    public record MonthStat(int Month, long Total);
    public record YearStat(int Year, long Total);
}
