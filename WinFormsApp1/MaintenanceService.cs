using System;
using System.Data;
using System.Data.SQLite;

namespace LabMaintenanceApp
{
    public static class MaintenanceService
    {
        public static DataTable GetPendingRequests(int userId)
        {
            var user = DatabaseHelper.GetDataTable("SELECT Role FROM Users WHERE UserID=@id", new SQLiteParameter("@id", userId));
            string role = user.Rows.Count == 1 ? user.Rows[0]["Role"].ToString() ?? "" : "";
            if (!RolePermissions.CanCompleteMaintenance(role))
                return DatabaseHelper.GetDataTable("SELECT RequestID, '' AS DisplayText FROM MaintenanceRequests WHERE 1=0");

            return DatabaseHelper.GetDataTable(@"
                SELECT r.RequestID, ('[Phiếu #' || r.RequestID || '] ' || e.EquipmentName || ' - ' || r.IssueDescription
                    || ' (' || COALESCE(u.FullName, 'Chưa phân công') || ')') AS DisplayText
                FROM MaintenanceRequests r JOIN Equipments e ON r.EquipmentID = e.EquipmentID
                LEFT JOIN Users u ON r.AssignedTechnicianID = u.UserID
                WHERE r.Status = 'Chờ xử lý'
                  AND (@admin = 1 OR r.AssignedTechnicianID = @id OR r.AssignedTechnicianID IS NULL)
                ORDER BY r.RequestID DESC",
                new SQLiteParameter("@admin", role == "Admin" ? 1 : 0), new SQLiteParameter("@id", userId));
        }

        public static void CreateRequest(int userId, int equipmentId, string issue, string priority, int? assignedTechnicianId)
        {
            if (string.IsNullOrWhiteSpace(issue)) throw new ArgumentException("Vui lòng nhập mô tả sự cố!");
            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    string role = GetRole(conn, transaction, userId);
                    if (!RolePermissions.CanCreateRequest(role)) throw new UnauthorizedAccessException("Bạn không có quyền tạo phiếu bảo trì.");
                    // Kỹ thuật viên luôn tự nhận phiếu, kể cả khi bên gọi gửi ID người khác.
                    int technicianId = role == "Technician" ? userId : assignedTechnicianId
                        ?? throw new ArgumentException("Vui lòng phân công kỹ thuật viên!");
                    if (GetRole(conn, transaction, technicianId) != "Technician")
                        throw new ArgumentException("Người được phân công phải là kỹ thuật viên.");

                    using (var cmd = new SQLiteCommand(@"
                        INSERT INTO MaintenanceRequests
                            (EquipmentID, RequestedBy, RequestDate, IssueDescription, Priority, Status, AssignedTechnicianID)
                        SELECT EquipmentID, @u, @d, @iss, @p, 'Chờ xử lý', @t FROM Equipments WHERE EquipmentID=@eq", conn, transaction))
                    {
                        cmd.Parameters.AddRange(new[] {
                            new SQLiteParameter("@u", userId), new SQLiteParameter("@d", DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                            new SQLiteParameter("@iss", issue.Trim()), new SQLiteParameter("@p", priority),
                            new SQLiteParameter("@t", technicianId), new SQLiteParameter("@eq", equipmentId)
                        });
                        if (cmd.ExecuteNonQuery() != 1) throw new ArgumentException("Thiết bị không còn tồn tại.");
                    }
                    using (var cmd = new SQLiteCommand("UPDATE Equipments SET Status='Đang bảo trì' WHERE EquipmentID=@eq", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@eq", equipmentId);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        public static void CompleteRequest(int userId, int requestId, string action, double cost)
        {
            if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("Vui lòng nhập kết quả sửa chữa!");
            if (double.IsNaN(cost) || double.IsInfinity(cost) || cost < 0) throw new ArgumentException("Chi phí phải là số không âm.");
            using (var conn = new SQLiteConnection(DatabaseHelper.ConnectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    string role = GetRole(conn, transaction, userId);
                    if (!RolePermissions.CanCompleteMaintenance(role))
                        throw new UnauthorizedAccessException("Chỉ kỹ thuật viên được phân công hoặc Admin mới được hoàn tất bảo trì.");

                    // Phiếu cũ chưa phân công có thể được kỹ thuật viên nhận khi xử lý.
                    // Điều kiện trong UPDATE ngăn xử lý phiếu người khác và ghi lịch sử trùng.
                    using (var cmd = new SQLiteCommand(@"
                        UPDATE MaintenanceRequests SET Status='Hoàn thành', AssignedTechnicianID=COALESCE(AssignedTechnicianID, @id)
                        WHERE RequestID=@r AND Status='Chờ xử lý'
                          AND (@admin=1 OR AssignedTechnicianID=@id OR AssignedTechnicianID IS NULL)", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@r", requestId);
                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.Parameters.AddWithValue("@admin", role == "Admin" ? 1 : 0);
                        if (cmd.ExecuteNonQuery() != 1)
                            throw new InvalidOperationException("Phiếu đã hoàn thành, không tồn tại hoặc được giao cho kỹ thuật viên khác.");
                    }
                    using (var cmd = new SQLiteCommand(@"
                        INSERT INTO MaintenanceLogs (RequestID, TechnicianID, CompletedDate, ActionTaken, Cost)
                        VALUES (@r, @t, @d, @act, @c)", conn, transaction))
                    {
                        cmd.Parameters.AddRange(new[] {
                            new SQLiteParameter("@r", requestId), new SQLiteParameter("@t", userId),
                            new SQLiteParameter("@d", DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                            new SQLiteParameter("@act", action.Trim()), new SQLiteParameter("@c", cost)
                        });
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new SQLiteCommand(@"
                        UPDATE Equipments SET Status = CASE WHEN EXISTS (
                            SELECT 1 FROM MaintenanceRequests r WHERE r.EquipmentID=Equipments.EquipmentID AND r.Status='Chờ xử lý'
                        ) THEN 'Đang bảo trì' ELSE 'Tốt' END
                        WHERE EquipmentID=(SELECT EquipmentID FROM MaintenanceRequests WHERE RequestID=@r)", conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@r", requestId);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
            }
        }

        private static string GetRole(SQLiteConnection conn, SQLiteTransaction transaction, int userId)
        {
            using (var cmd = new SQLiteCommand("SELECT Role FROM Users WHERE UserID=@id", conn, transaction))
            {
                cmd.Parameters.AddWithValue("@id", userId);
                return cmd.ExecuteScalar()?.ToString() ?? "";
            }
        }
    }
}
