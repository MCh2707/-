using System.Security.Cryptography;
using System.Text;
using BAGEBI.Data;
using Microsoft.EntityFrameworkCore;

namespace BAGEBI.Services;

public class AuthService
{
    private readonly AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return null;

        var cleanUsername = username.Trim();
        var user = await _context.Users
            .Include(u => u.Kindergarten)
            .FirstOrDefaultAsync(u => u.Username.ToLower() == cleanUsername.ToLower());

        if (user == null)
            return null;

        var hashedPassword = HashPassword(password);
        if (user.PasswordHash == hashedPassword)
        {
            return user;
        }

        return null;
    }

    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password + "BAGEBI_SALT_2026");
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
