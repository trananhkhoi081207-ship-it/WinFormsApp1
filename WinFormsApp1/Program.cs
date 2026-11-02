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

            using (FrmLogin login = new FrmLogin())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new Form1(login.LoggedInUserId, login.LoggedInFullName, login.LoggedInRole));
                }
            }
        }
    }

    // Màn hình Đăng nhập hiện đại, bo góc mềm mại
    public class FrmLogin : Form
    {
        private TextBox txtUser, txtPass;
        public int LoggedInUserId { get; private set; }
        public string LoggedInFullName { get; private set; }
        public string LoggedInRole { get; private set; }

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
            txtUser = new TextBox { Location = new Point(120, 65), Width = 195, Font = new Font("Segoe UI", 10f), Text = "admin" };

            Label lblP = new Label { Text = "Mật khẩu:", Font = new Font("Segoe UI Semibold", 9.5f), Location = new Point(30, 108), AutoSize = true };
            txtPass = new TextBox { Location = new Point(120, 105), Width = 195, Font = new Font("Segoe UI", 10f), PasswordChar = '●', Text = "123456" };

            Button btnLogin = new Button { Text = "ĐĂNG NHẬP HỆ THỐNG", Location = new Point(30, 150), Size = new Size(285, 38), BackColor = Color.FromArgb(59, 130, 246), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold), Cursor = Cursors.Hand };
            btnLogin.FlatAppearance.BorderSize = 0;
            Form1.MakeRounded(btnLogin, 10);
            btnLogin.Click += (s, e) =>
            {
                var dt = DatabaseHelper.GetDataTable("SELECT UserID, FullName, Role FROM Users WHERE Username=@u AND PasswordHash=@p",
                    new SQLiteParameter("@u", txtUser.Text.Trim()), new SQLiteParameter("@p", txtPass.Text.Trim()));
                if (dt.Rows.Count > 0)
                {
                    LoggedInUserId = Convert.ToInt32(dt.Rows[0]["UserID"]);
                    LoggedInFullName = dt.Rows[0]["FullName"].ToString();
                    LoggedInRole = dt.Rows[0]["Role"].ToString();
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
        public static readonly string ConnectionString = $"Data Source={dbFile};Version=3;";

        public static void InitializeDatabase()
        {
            if (!File.Exists(dbFile)) SQLiteConnection.CreateFile(dbFile);
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                string sql = @"
                CREATE TABLE IF NOT EXISTS Users (UserID INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT NOT NULL UNIQUE, PasswordHash TEXT NOT NULL, FullName TEXT NOT NULL, Role TEXT NOT NULL);
                CREATE TABLE IF NOT EXISTS Labs (LabID INTEGER PRIMARY KEY AUTOINCREMENT, LabCode TEXT NOT NULL UNIQUE, LabName TEXT NOT NULL, Location TEXT, Status TEXT DEFAULT 'Hoạt động');
                CREATE TABLE IF NOT EXISTS Equipments (EquipmentID INTEGER PRIMARY KEY AUTOINCREMENT, EquipmentCode TEXT NOT NULL UNIQUE, EquipmentName TEXT NOT NULL, Category TEXT, LabID INTEGER NOT NULL, Status TEXT DEFAULT 'Tốt');
                CREATE TABLE IF NOT EXISTS MaintenanceRequests (RequestID INTEGER PRIMARY KEY AUTOINCREMENT, EquipmentID INTEGER NOT NULL, RequestedBy INTEGER NOT NULL, RequestDate TEXT NOT NULL, IssueDescription TEXT NOT NULL, Priority TEXT DEFAULT 'Trung bình', Status TEXT DEFAULT 'Chờ xử lý');
                CREATE TABLE IF NOT EXISTS MaintenanceLogs (LogID INTEGER PRIMARY KEY AUTOINCREMENT, RequestID INTEGER NOT NULL, TechnicianID INTEGER NOT NULL, CompletedDate TEXT, ActionTaken TEXT NOT NULL, Cost REAL DEFAULT 0);";
                using (var cmd = new SQLiteCommand(sql, conn)) cmd.ExecuteNonQuery();

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