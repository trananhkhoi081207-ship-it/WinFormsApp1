using System.Data.SQLite;

namespace LabMaintenanceApp;

public static class AdministrationService
{
    public static void UpdateLab(int actorId, int labId, string code, string name, string location)
    {
        code = InputLimits.Required(code, InputLimits.Code, "Mã phòng");
        name = InputLimits.Required(name, InputLimits.Name, "Tên phòng");
        location = InputLimits.Optional(location, InputLimits.Location, "Vị trí");
        using var conn = new SQLiteConnection(DatabaseHelper.ConnectionString);
        conn.Open();
        using var transaction = conn.BeginTransaction();
        if (!RolePermissions.CanManageLabs(GetRole(conn, transaction, actorId)))
            throw new UnauthorizedAccessException("Bạn không có quyền sửa phòng Lab.");
        using var cmd = new SQLiteCommand("UPDATE Labs SET LabCode=@c, LabName=@n, Location=@l WHERE LabID=@id", conn, transaction);
        cmd.Parameters.AddWithValue("@c", code);
        cmd.Parameters.AddWithValue("@n", name);
        cmd.Parameters.AddWithValue("@l", location);
        cmd.Parameters.AddWithValue("@id", labId);
        if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Phòng Lab không còn tồn tại.");
        transaction.Commit();
    }

    public static void CreateUser(int actorId, string username, string password, string fullName, string role)
        => SaveUser(actorId, null, username, password, fullName, role);

    public static void UpdateUser(int actorId, int userId, string username, string password, string fullName, string role)
        => SaveUser(actorId, userId, username, password, fullName, role);

    private static void SaveUser(int actorId, int? userId, string username, string password, string fullName, string role)
    {
        username = InputLimits.Required(username, InputLimits.Username, "Tên đăng nhập");
        fullName = InputLimits.Required(fullName, InputLimits.Name, "Họ và tên");
        if (role != "Admin" && role != "LabManager" && role != "Technician")
            throw new ArgumentException("Vai trò không hợp lệ.");
        // Only an exactly empty password means 'keep existing'; spaces are significant.
        if ((!userId.HasValue && string.IsNullOrWhiteSpace(password)) || password.Length > InputLimits.Password
            || (password.Length > 0 && string.IsNullOrWhiteSpace(password)))
            throw new ArgumentException($"Mật khẩu phải có từ 1 đến {InputLimits.Password} ký tự và không chỉ gồm khoảng trắng.");

        using var conn = new SQLiteConnection(DatabaseHelper.ConnectionString);
        conn.Open();
        using var transaction = conn.BeginTransaction();
        if (!RolePermissions.CanManageUsers(GetRole(conn, transaction, actorId)))
            throw new UnauthorizedAccessException("Chỉ Admin được quản lý người dùng.");
        if (userId.HasValue)
        {
            string previousRole = GetRole(conn, transaction, userId.Value);
            if (previousRole == "") throw new InvalidOperationException("Người dùng không còn tồn tại.");
            if ((userId == 1 || userId == actorId) && role != previousRole)
                throw new InvalidOperationException("Không thể đổi vai trò Admin mặc định hoặc tài khoản đang đăng nhập.");
            if (previousRole == "Technician" && role != previousRole)
            {
                using var pending = new SQLiteCommand("SELECT COUNT(*) FROM MaintenanceRequests WHERE AssignedTechnicianID=@id AND Status<>'Hoàn thành'", conn, transaction);
                pending.Parameters.AddWithValue("@id", userId.Value);
                if (Convert.ToInt64(pending.ExecuteScalar()) > 0)
                    throw new InvalidOperationException("Kỹ thuật viên còn phiếu mở, chưa thể đổi vai trò.");
            }
        }
        using var cmd = new SQLiteCommand(userId.HasValue
            ? "UPDATE Users SET Username=@u, PasswordHash=COALESCE(@p, PasswordHash), FullName=@f, Role=@r WHERE UserID=@id"
            : "INSERT INTO Users (Username, PasswordHash, FullName, Role) VALUES (@u, @p, @f, @r)", conn, transaction);
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", password.Length == 0 ? (object)DBNull.Value : PasswordHasher.Hash(password));
        cmd.Parameters.AddWithValue("@f", fullName);
        cmd.Parameters.AddWithValue("@r", role);
        if (userId.HasValue) cmd.Parameters.AddWithValue("@id", userId.Value);
        if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Không lưu được người dùng.");
        transaction.Commit();
    }

    private static string GetRole(SQLiteConnection conn, SQLiteTransaction transaction, int userId)
    {
        using var cmd = new SQLiteCommand("SELECT Role FROM Users WHERE UserID=@id", conn, transaction);
        cmd.Parameters.AddWithValue("@id", userId);
        return cmd.ExecuteScalar()?.ToString() ?? "";
    }
}
