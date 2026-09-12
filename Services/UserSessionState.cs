using BAGEBI.Data;

namespace BAGEBI.Services;

public class UserSessionState
{
    public User? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser != null;
    public bool IsAdmin => CurrentUser?.Role == "Admin";
    public int? KindergartenId => CurrentUser?.KindergartenId;
    public string KindergartenName => CurrentUser?.Kindergarten?.Name ?? (IsAdmin ? "ადმინისტრატორი" : "ბაგა-ბაღი");

    public event Action? OnChange;

    public void Login(User user)
    {
        CurrentUser = user;
        NotifyStateChanged();
    }

    public void Logout()
    {
        CurrentUser = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
