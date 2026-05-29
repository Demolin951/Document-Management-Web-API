namespace Utils;

public class UtilFunktion
{
    public static bool IsAdmin(string currentUsername)
    {
        return string.Equals(currentUsername, "admin", StringComparison.OrdinalIgnoreCase);
    }
}

