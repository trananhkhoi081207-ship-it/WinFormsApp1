namespace LabMaintenanceApp;

public static class InputLimits
{
    public const int Code = 50;
    public const int Username = 50;
    public const int Password = 128;
    public const int Name = 200;
    public const int Category = 100;
    public const int Location = 200;
    public const int Description = 2000;
    public const int Money = 21; // 9.007.199.254.740.991

    public static string Required(string value, int maximum, string label)
    {
        value = value.Trim();
        if (value.Length == 0 || value.Length > maximum)
            throw new ArgumentException($"{label} phải có từ 1 đến {maximum} ký tự.");
        return value;
    }

    public static string Optional(string value, int maximum, string label)
    {
        value = value.Trim();
        if (value.Length > maximum) throw new ArgumentException($"{label} tối đa {maximum} ký tự.");
        return value;
    }
}
