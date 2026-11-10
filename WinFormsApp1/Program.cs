using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace LabMaintenanceApp
{
    static class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
        [STAThread]
        static void Main()
        {
            SetProcessDPIAware();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DatabaseHelper.InitializeDatabase();

            while (true)
            {
                using (FrmLogin login = new FrmLogin())
                {
                    if (login.ShowDialog() != DialogResult.OK) break;
                    using (var main = new Form1(login.LoggedInUserId, login.LoggedInFullName, login.LoggedInRole))
                    {
                        main.WindowState = FormWindowState.Maximized;
                        Application.Run(main);
                        if (!main.LogoutRequested) break;
                    }
                }
            }
        }
    }

    // Màn hình Đăng nhập hiện đại, bo góc mềm mại
    public class FrmLogin : Form
    {
        private TextBox txtUser, txtPass;
        public int LoggedInUserId { get; private set; }
        public string LoggedInFullName { get; private set; } = string.Empty;
        public string LoggedInRole { get; private set; } = string.Empty;

        public FrmLogin()
        {
            this.Text = "Đăng nhập - LabCare Pro";
            this.Size = new Size(420, 310);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(241, 245, 249);
            this.MaximizeBox = false;

            Panel card = new Panel { BackColor = Color.White, Location = new Point(25, 20), Size = new Size(355, 230) };
            Form1.MakeRounded(card, 18);

            Label lblTitle = new Label { Text = "🧪 LABCARE PRO", Font = new Font("Segoe UI", 15, FontStyle.Bold), ForeColor = Color.FromArgb(37, 99, 235), AutoSize = true, Location = new Point(90, 18) };
            Label lblU = new Label { Text = "Tài khoản:", Font = new Font("Segoe UI Semibold", 9.5f), Location = new Point(30, 68), AutoSize = true };
            txtUser = new TextBox { Location = new Point(120, 65), Width = 195, Font = new Font("Segoe UI", 10f) };

            Label lblP = new Label { Text = "Mật khẩu:", Font = new Font("Segoe UI Semibold", 9.5f), Location = new Point(30, 108), AutoSize = true };
            txtPass = new TextBox { Location = new Point(120, 105), Width = 195, Font = new Font("Segoe UI", 10f), PasswordChar = '●' };

            Button btnLogin = new Button { Text = "ĐĂNG NHẬP HỆ THỐNG", Location = new Point(30, 150), Size = new Size(285, 38), BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnLogin.FlatAppearance.BorderSize = 0;
            Form1.MakeRounded(btnLogin, 10);
            btnLogin.Click += (s, e) =>
            {
                var dt = DatabaseHelper.GetDataTable("SELECT UserID, FullName, Role, PasswordHash FROM Users WHERE Username=@u",
                    new SQLiteParameter("@u", txtUser.Text.Trim()));
                if (dt.Rows.Count == 1 && PasswordHasher.Verify(txtPass.Text, dt.Rows[0]["PasswordHash"].ToString() ?? ""))
                {
                    LoggedInUserId = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    LoggedInFullName = dt.Rows[0]["FullName"].ToString() ?? string.Empty;
                    LoggedInRole = dt.Rows[0]["Role"].ToString() ?? string.Empty;
                    this.DialogResult = DialogResult.OK;
                }
                else MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            Label lblHint = new Label { Text = "TK mẫu: admin / ql_lab1 / ktv_01 (MK: 123456)", ForeColor = Color.Gray, Font = new Font("Segoe UI", 8.5f), Location = new Point(45, 198), AutoSize = true };

            card.Controls.AddRange(new Control[] { lblTitle, lblU, txtUser, lblP, txtPass, btnLogin, lblHint });
            this.Controls.Add(card);
            this.AcceptButton = btnLogin;
        }
    }

    public static class DatabaseHelper
    {
        private static readonly string dbFile = "LabMaintenance.db";
        public static readonly string ConnectionString = $"Data Source={dbFile};Version=3;Foreign Keys=True;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(dbFile)) SQLiteConnection.CreateFile(dbFile);
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"
                CREATE TABLE IF NOT EXISTS Users (UserID INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT NOT NULL UNIQUE, PasswordHash TEXT NOT NULL, FullName TEXT NOT NULL, Role TEXT NOT NULL);
                CREATE TABLE IF NOT EXISTS Labs (LabID INTEGER PRIMARY KEY AUTOINCREMENT, LabCode TEXT NOT NULL UNIQUE, LabName TEXT NOT NULL, Location TEXT, Status TEXT DEFAULT 'Hoạt động');
                CREATE TABLE IF NOT EXISTS Equipments (EquipmentID INTEGER PRIMARY KEY AUTOINCREMENT, EquipmentCode TEXT NOT NULL UNIQUE, EquipmentName TEXT NOT NULL, Category TEXT, LabID INTEGER NOT NULL REFERENCES Labs(LabID) ON DELETE RESTRICT, Status TEXT DEFAULT 'Tốt');
                CREATE TABLE IF NOT EXISTS MaintenanceRequests (RequestID INTEGER PRIMARY KEY AUTOINCREMENT, EquipmentID INTEGER NOT NULL REFERENCES Equipments(EquipmentID) ON DELETE RESTRICT, RequestedBy INTEGER NOT NULL REFERENCES Users(UserID) ON DELETE RESTRICT, RequestDate TEXT NOT NULL, IssueDescription TEXT NOT NULL, Priority TEXT DEFAULT 'Trung bình', Status TEXT DEFAULT 'Chờ xử lý');
                CREATE TABLE IF NOT EXISTS MaintenanceLogs (LogID INTEGER PRIMARY KEY AUTOINCREMENT, RequestID INTEGER NOT NULL REFERENCES MaintenanceRequests(RequestID) ON DELETE RESTRICT, TechnicianID INTEGER NOT NULL REFERENCES Users(UserID) ON DELETE RESTRICT, CompletedDate TEXT, ActionTaken TEXT NOT NULL, Cost REAL DEFAULT 0);";
                using (var cmd = new SQLiteCommand(sql, conn)) cmd.ExecuteNonQuery();

                // Nâng cấp CSDL cũ tại chỗ, không xóa dữ liệu người dùng.
                bool hasAssignedTechnician = false;
                using (var cmd = new SQLiteCommand("PRAGMA table_info(MaintenanceRequests)", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        if (reader["name"].ToString() == "AssignedTechnicianID") hasAssignedTechnician = true;
                }
                if (!hasAssignedTechnician)
                {
                    using (var cmd = new SQLiteCommand(
                        "ALTER TABLE MaintenanceRequests ADD COLUMN AssignedTechnicianID INTEGER REFERENCES Users(UserID)", conn))
                        cmd.ExecuteNonQuery();
                }

                // Phiếu cũ do kỹ thuật viên tạo được giao lại cho đúng người tạo.
                using (var cmd = new SQLiteCommand(@"
                    UPDATE MaintenanceRequests SET AssignedTechnicianID = RequestedBy
                    WHERE AssignedTechnicianID IS NULL
                      AND RequestedBy IN (SELECT UserID FROM Users WHERE Role = 'Technician');", conn))
                    cmd.ExecuteNonQuery();

                using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Users", conn))
                {
                    if ((long)checkCmd.ExecuteScalar() == 0)
                    {
                        string seed = @"
                        INSERT INTO Users (Username, PasswordHash, FullName, Role) VALUES ('admin', '123456', 'Quản trị hệ thống', 'Admin'), ('ql_lab1', '123456', 'Nguyễn Văn Quản Lý', 'LabManager'), ('ktv_01', '123456', 'Trần Kỹ Thuật', 'Technician');
                        INSERT INTO Labs (LabCode, LabName, Location) VALUES ('LAB-01', 'Phòng Thí Nghiệm Hóa Sinh', 'Tòa A - Tầng 2'), ('LAB-02', 'Phòng Viễn Thông & Điện Tử', 'Tòa B - Tầng 3');
                        INSERT INTO Equipments (EquipmentCode, EquipmentName, Category, LabID, Status) VALUES ('EQ-001', 'Kính hiển vi quang học Olympus', 'Quang học', 1, 'Tốt'), ('EQ-002', 'Máy ly tâm tốc độ cao', 'Hóa sinh', 1, 'Đang bảo trì'), ('EQ-003', 'Máy hiện sóng số Rigol', 'Đo lường', 2, 'Tốt');
                        INSERT INTO MaintenanceRequests (EquipmentID, RequestedBy, RequestDate, IssueDescription, Priority, Status) VALUES (2, 2, '2026-05-01 09:00', 'Kêu to khi quay trên 3000 vòng/phút', 'Khẩn cấp', 'Chờ xử lý');";
                        using (var seedCmd = new SQLiteCommand(seed, conn)) seedCmd.ExecuteNonQuery();
                    }
                }
                // Nâng cấp mật khẩu cũ tại chỗ; không thay đổi mật khẩu người dùng nhập.
                using (var transaction = conn.BeginTransaction())
                {
                    var passwords = new System.Collections.Generic.List<(long Id, string Value)>();
                    using (var cmd = new SQLiteCommand("SELECT UserID, PasswordHash FROM Users", conn, transaction))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read()) passwords.Add((reader.GetInt64(0), reader.GetString(1)));
                    foreach (var (id, value) in passwords)
                    {
                        if (PasswordHasher.IsHashed(value)) continue;
                        using var cmd = new SQLiteCommand("UPDATE Users SET PasswordHash=@p WHERE UserID=@id", conn, transaction);
                        cmd.Parameters.AddWithValue("@p", PasswordHasher.Hash(value));
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                // Bổ sung cột nullable: lịch sử cũ không được suy đoán là sửa thành công.
                bool hasOutcome = false;
                using (var cmd = new SQLiteCommand("PRAGMA table_info(MaintenanceLogs)", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read()) if (reader["name"].ToString() == "EquipmentStatusAfter") hasOutcome = true;
                if (!hasOutcome)
                    using (var cmd = new SQLiteCommand("ALTER TABLE MaintenanceLogs ADD COLUMN EquipmentStatusAfter TEXT CHECK (EquipmentStatusAfter IN ('Tốt','Hỏng'))", conn)) cmd.ExecuteNonQuery();

                // Bảng SQLite cũ không thể thêm FK bằng ALTER COLUMN. Trigger tương đương
                // bảo vệ các lần ghi mới mà không xóa hay tự sửa dữ liệu lịch sử.
                foreach (var relation in new[] {
                    ("Equipments", "LabID", "Labs", "LabID"),
                    ("MaintenanceRequests", "EquipmentID", "Equipments", "EquipmentID"),
                    ("MaintenanceRequests", "RequestedBy", "Users", "UserID"),
                    ("MaintenanceRequests", "AssignedTechnicianID", "Users", "UserID"),
                    ("MaintenanceLogs", "RequestID", "MaintenanceRequests", "RequestID"),
                    ("MaintenanceLogs", "TechnicianID", "Users", "UserID") })
                {
                    var (child, column, parent, key) = relation;
                    string name = child + "_" + column;
                    string guards = $@"
                        CREATE TRIGGER IF NOT EXISTS FK_{name}_delete BEFORE DELETE ON {parent}
                        WHEN EXISTS (SELECT 1 FROM {child} WHERE {column}=OLD.{key})
                        BEGIN SELECT RAISE(ABORT, 'Dữ liệu đang được tham chiếu, không thể xóa.'); END;
                        CREATE TRIGGER IF NOT EXISTS FK_{name}_key BEFORE UPDATE OF {key} ON {parent}
                        WHEN NEW.{key}<>OLD.{key} AND EXISTS (SELECT 1 FROM {child} WHERE {column}=OLD.{key})
                        BEGIN SELECT RAISE(ABORT, 'Không thể thay đổi khóa đang được tham chiếu.'); END;";
                    foreach (string operation in new[] { "INSERT", "UPDATE" })
                        guards += $@"CREATE TRIGGER IF NOT EXISTS FK_{name}_{operation} BEFORE {operation} ON {child}
                            WHEN NEW.{column} IS NOT NULL AND NOT EXISTS (SELECT 1 FROM {parent} WHERE {key}=NEW.{column})
                            BEGIN SELECT RAISE(ABORT, 'Dữ liệu tham chiếu không tồn tại.'); END;";
                    using (var cmd = new SQLiteCommand(guards, conn)) cmd.ExecuteNonQuery();
                }
                using (var cmd = new SQLiteCommand(@"
                    CREATE TRIGGER IF NOT EXISTS GuardRequestInsert BEFORE INSERT ON MaintenanceRequests
                    WHEN (SELECT Status FROM Equipments WHERE EquipmentID=NEW.EquipmentID)='Thanh lý'
                      OR (NEW.Status<>'Hoàn thành' AND EXISTS (SELECT 1 FROM MaintenanceRequests WHERE EquipmentID=NEW.EquipmentID AND Status<>'Hoàn thành'))
                    BEGIN SELECT RAISE(ABORT, 'Thiết bị đã thanh lý hoặc đang có phiếu chưa hoàn thành.'); END;
                    CREATE TRIGGER IF NOT EXISTS GuardRequestUpdate BEFORE UPDATE OF EquipmentID, Status ON MaintenanceRequests
                    WHEN NEW.Status<>'Hoàn thành' AND (
                        (SELECT Status FROM Equipments WHERE EquipmentID=NEW.EquipmentID)='Thanh lý'
                        OR EXISTS (SELECT 1 FROM MaintenanceRequests WHERE EquipmentID=NEW.EquipmentID AND RequestID<>NEW.RequestID AND Status<>'Hoàn thành'))
                    BEGIN SELECT RAISE(ABORT, 'Thiết bị đã thanh lý hoặc đang có phiếu chưa hoàn thành.'); END;
                    CREATE TRIGGER IF NOT EXISTS GuardActiveMaintenanceStatus BEFORE UPDATE OF Status ON Equipments
                    WHEN NEW.Status<>'Đang bảo trì' AND EXISTS (
                        SELECT 1 FROM MaintenanceRequests WHERE EquipmentID=NEW.EquipmentID AND Status<>'Hoàn thành')
                    BEGIN SELECT RAISE(ABORT, 'Thiết bị còn phiếu mở phải giữ trạng thái Đang bảo trì.'); END;
                    CREATE TRIGGER IF NOT EXISTS GuardRetirement BEFORE UPDATE OF Status ON Equipments
                    WHEN NEW.Status='Thanh lý' AND EXISTS (SELECT 1 FROM MaintenanceRequests WHERE EquipmentID=NEW.EquipmentID AND Status<>'Hoàn thành')
                    BEGIN SELECT RAISE(ABORT, 'Phải hoàn tất phiếu bảo trì trước khi thanh lý.'); END;", conn)) cmd.ExecuteNonQuery();

            }
        }

        public static DataTable GetDataTable(string query, params SQLiteParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static int ExecuteNonQuery(string query, params SQLiteParameter[] parameters)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
