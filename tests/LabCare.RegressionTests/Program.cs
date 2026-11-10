using System.Data.SQLite;
using LabMaintenanceApp;

internal static class RegressionTests
{
    private static int checks;

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.SequenceEqual(new[] { "--database-path" }))
        {
            Console.WriteLine(DatabaseHelper.DatabasePath);
            return 0;
        }
        string previousDirectory = Environment.CurrentDirectory;
        string directory = Path.Combine(Path.GetTempPath(), "LabCare-tests-" + Guid.NewGuid());
        Directory.CreateDirectory(directory);
        AppContext.SetData("LabCare.DatabasePath", Path.Combine(directory, "LabMaintenance.db"));
        Environment.CurrentDirectory = directory;
        try
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            TestMigration();
            TestDatabaseLocation();
            TestPasswords();
            TestAssignmentAndCompletion();
            TestSafetyAndMoney();
            TestRoleScreensAndLogout();
            TestAdministration();
            TestInputAndEditScreens();
            TestEquipmentEditingAndStatistics();
            TestHistoryScaling();
            TestWideWindowLayout();
            Console.WriteLine($"PASS: {checks} regression checks");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
        finally
        {
            SQLiteConnection.ClearAllPools();
            Environment.CurrentDirectory = previousDirectory;
            Directory.Delete(directory, true);
        }
    }

    private static void TestMigration()
    {
        // Tái tạo cấu trúc phiếu của bản cũ, trước khi có cột phân công.
        DatabaseHelper.ExecuteNonQuery(@"
            CREATE TABLE Users (UserID INTEGER PRIMARY KEY AUTOINCREMENT, Username TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL, FullName TEXT NOT NULL, Role TEXT NOT NULL);
            CREATE TABLE Labs (LabID INTEGER PRIMARY KEY AUTOINCREMENT, LabCode TEXT NOT NULL UNIQUE,
                LabName TEXT NOT NULL, Location TEXT, Status TEXT DEFAULT 'Hoạt động');
            CREATE TABLE Equipments (EquipmentID INTEGER PRIMARY KEY AUTOINCREMENT, EquipmentCode TEXT NOT NULL UNIQUE,
                EquipmentName TEXT NOT NULL, Category TEXT, LabID INTEGER NOT NULL, Status TEXT DEFAULT 'Tốt');
            CREATE TABLE MaintenanceLogs (LogID INTEGER PRIMARY KEY AUTOINCREMENT, RequestID INTEGER NOT NULL,
                TechnicianID INTEGER NOT NULL, CompletedDate TEXT, ActionTaken TEXT NOT NULL, Cost REAL DEFAULT 0);
            CREATE TABLE MaintenanceRequests (RequestID INTEGER PRIMARY KEY AUTOINCREMENT,
                EquipmentID INTEGER NOT NULL, RequestedBy INTEGER NOT NULL, RequestDate TEXT NOT NULL,
                IssueDescription TEXT NOT NULL, Priority TEXT DEFAULT 'Trung bình', Status TEXT DEFAULT 'Chờ xử lý');
            INSERT INTO MaintenanceRequests (EquipmentID, RequestedBy, RequestDate, IssueDescription)
                VALUES (1, 3, '2026-01-01', 'Legacy technician request');");
        DatabaseHelper.InitializeDatabase();
        DatabaseHelper.InitializeDatabase();
        Check(Scalar("SELECT COUNT(*) FROM pragma_table_info('MaintenanceRequests') WHERE name='AssignedTechnicianID'") == 1, "Migration is idempotent");
        Check(Scalar("SELECT AssignedTechnicianID FROM MaintenanceRequests WHERE RequestID=1") == 3, "Legacy technician request is assigned to its creator");
        Check(Scalar("SELECT COUNT(*) FROM Users") == 3, "Existing data and seeded users survive migration");
        Check(Scalar("SELECT COUNT(*) FROM pragma_table_info('MaintenanceLogs') WHERE name='EquipmentStatusAfter'") == 1, "Old history gains a nullable repair outcome");
        Throws<SQLiteException>(() => DatabaseHelper.ExecuteNonQuery("INSERT INTO Equipments (EquipmentCode, EquipmentName, LabID) VALUES ('ORPHAN', 'Orphan', 9999)"));
        Throws<SQLiteException>(() => DatabaseHelper.ExecuteNonQuery("UPDATE Equipments SET LabID=9999 WHERE EquipmentID=1"));
    }

    private static void TestDatabaseLocation()
    {
        string databasePath = DatabaseHelper.DatabasePath;
        long users = Scalar("SELECT COUNT(*) FROM Users");
        string original = Environment.CurrentDirectory;
        string shortcutDirectory = Path.Combine(original, "shortcut-start-in");
        Directory.CreateDirectory(shortcutDirectory);
        try
        {
            Environment.CurrentDirectory = shortcutDirectory;
            DatabaseHelper.InitializeDatabase();
            Check(Path.IsPathFullyQualified(databasePath) && DatabaseHelper.DatabasePath == databasePath,
                "Database uses an absolute path unaffected by shortcut working directory");
            Check(Scalar("SELECT COUNT(*) FROM Users") == users && !File.Exists("LabMaintenance.db"),
                "Changing working directory preserves data without creating a second database");
            using var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = Environment.ProcessPath!, Arguments = "--database-path", UseShellExecute = false,
                RedirectStandardOutput = true, RedirectStandardError = true
            })!;
            string defaultPath = process.StandardOutput.ReadToEnd().Trim();
            Check(process.WaitForExit(10_000) && process.ExitCode == 0
                && defaultPath == Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LabMaintenance.db"),
                "Fresh process defaults to the EXE directory even from a different working directory");
        }
        finally { Environment.CurrentDirectory = original; }
    }

    private static void TestPasswords()
    {
        string stored = DatabaseHelper.GetDataTable("SELECT PasswordHash FROM Users WHERE UserID=1").Rows[0][0].ToString()!;
        Check(stored != "123456" && PasswordHasher.Verify("123456", stored), "Sample passwords are hashed and remain usable");
        Check(!PasswordHasher.Verify("incorrect", stored), "Wrong password is rejected");
        Check(!PasswordHasher.Verify("123456", "123456"), "Login does not accept plaintext storage");
        Check(!PasswordHasher.Verify("123456", "pbkdf2-sha256$600000$bad$bad"), "Malformed hashes are rejected");
        string spaced = PasswordHasher.Hash(" password ");
        Check(PasswordHasher.Verify(" password ", spaced) && !PasswordHasher.Verify("password", spaced), "Password whitespace is preserved");
        Check(PasswordHasher.Hash("123456") != stored, "Each password has a random salt");
        DatabaseHelper.ExecuteNonQuery("INSERT INTO Users (Username, PasswordHash, FullName, Role) VALUES ('legacy-login', 'old-password', 'Legacy', 'Technician')");
        DatabaseHelper.InitializeDatabase();
        Check(PasswordHasher.Verify("old-password", DatabaseHelper.GetDataTable("SELECT PasswordHash FROM Users WHERE Username='legacy-login'").Rows[0][0].ToString()!), "Existing plaintext password migrates without a reset");
        Check(stored == DatabaseHelper.GetDataTable("SELECT PasswordHash FROM Users WHERE UserID=1").Rows[0][0].ToString(), "Repeated migration preserves existing hashes");
        DatabaseHelper.ExecuteNonQuery("DELETE FROM Users WHERE Username='legacy-login'");
        using var login = new FrmLogin();
        var inputs = login.Controls.OfType<Panel>().Single().Controls.OfType<TextBox>().ToArray();
        Check(inputs.All(input => input.Text == ""), "Login does not prefill credentials");
        inputs.Single(input => input.PasswordChar == '\0').Text = "admin";
        inputs.Single(input => input.PasswordChar != '\0').Text = "123456";
        login.Shown += (_, _) => login.BeginInvoke(new Action(() => login.AcceptButton!.PerformClick()));
        Check(login.ShowDialog() == DialogResult.OK && login.LoggedInUserId == 1, "Hashed sample account can sign in through the login screen");
    }

    private static void TestAssignmentAndCompletion()
    {
        DatabaseHelper.ExecuteNonQuery("INSERT INTO Users (Username, PasswordHash, FullName, Role) VALUES ('ktv_02', '123456', 'Kỹ thuật viên thứ hai', 'Technician')");
        int otherTech = (int)Scalar("SELECT UserID FROM Users WHERE Username='ktv_02'");
        int before = (int)Scalar("SELECT COUNT(*) FROM MaintenanceRequests");
        Throws<ArgumentException>(() => MaintenanceService.CreateRequest(2, 1, "Missing assignment", "Thấp", null));
        Throws<ArgumentException>(() => MaintenanceService.CreateRequest(2, 1, "Invalid assignment", "Thấp", 1));
        Throws<ArgumentException>(() => MaintenanceService.CreateRequest(2, 1, "Invalid assignee", "Thấp", 9999));
        Throws<ArgumentException>(() => MaintenanceService.CreateRequest(2, 9999, "Missing equipment", "Thấp", 3));
        Throws<UnauthorizedAccessException>(() => MaintenanceService.CreateRequest(9999, 1, "Missing user", "Thấp", 3));
        Check(Scalar("SELECT COUNT(*) FROM MaintenanceRequests") == before, "Rejected requests leave no rows");

        MaintenanceService.CompleteRequest(3, 1, "Legacy fixed", 0);
        MaintenanceService.CreateRequest(2, 1, "Manager request", "Khẩn cấp", otherTech);
        int managerRequest = (int)Scalar("SELECT MAX(RequestID) FROM MaintenanceRequests");
        Check(Scalar($"SELECT AssignedTechnicianID FROM MaintenanceRequests WHERE RequestID={managerRequest}") == otherTech, "Manager assignment persists");
        Check(!PendingIds(3).Contains(managerRequest) && PendingIds(otherTech).Contains(managerRequest), "Each technician sees assigned work only");
        Check(MaintenanceService.GetPendingRequests(2).Rows.Count == 0, "Manager cannot repair");
        Throws<UnauthorizedAccessException>(() => MaintenanceService.CompleteRequest(2, managerRequest, "Manager repair", 0));
        Throws<InvalidOperationException>(() => MaintenanceService.CompleteRequest(3, managerRequest, "Wrong technician", 0));
        Throws<ArgumentException>(() => MaintenanceService.CompleteRequest(otherTech, managerRequest, "Invalid cost", -1));

        Throws<InvalidOperationException>(() => MaintenanceService.CreateRequest(3, 1, "Duplicate", "Thấp", 3));
        MaintenanceService.CompleteRequest(otherTech, managerRequest, "Not repaired", 0, "Hỏng");
        Check(Scalar("SELECT COUNT(*) FROM Equipments WHERE EquipmentID=1 AND Status='Hỏng'") == 1, "Failed repair preserves broken status");
        MaintenanceService.CreateRequest(3, 1, "Technician request", "Trung bình", otherTech);
        int ownRequest = (int)Scalar("SELECT MAX(RequestID) FROM MaintenanceRequests");
        Check(Scalar($"SELECT AssignedTechnicianID FROM MaintenanceRequests WHERE RequestID={ownRequest}") == 3, "Technician is always self-assigned");

        DatabaseHelper.ExecuteNonQuery("CREATE TRIGGER FailLog BEFORE INSERT ON MaintenanceLogs BEGIN SELECT RAISE(ABORT, 'Test rollback'); END");
        Throws<SQLiteException>(() => MaintenanceService.CompleteRequest(3, ownRequest, "Failure", 0));
        Check(Scalar($"SELECT COUNT(*) FROM MaintenanceRequests WHERE RequestID={ownRequest} AND Status='Chờ xử lý'") == 1, "Failed log rolls back completion");
        DatabaseHelper.ExecuteNonQuery("DROP TRIGGER FailLog");

        MaintenanceService.CompleteRequest(3, ownRequest, "Fixed", 100000);
        Check(Scalar($"SELECT TechnicianID FROM MaintenanceLogs WHERE RequestID={ownRequest}") == 3, "History records the technician who repaired");
        Check(Scalar("SELECT COUNT(*) FROM Equipments WHERE EquipmentID=1 AND Status='Tốt'") == 1, "Successful repair marks equipment good");
        Throws<InvalidOperationException>(() => MaintenanceService.CompleteRequest(3, ownRequest, "Duplicate", 0));
        Check(Scalar($"SELECT COUNT(*) FROM MaintenanceLogs WHERE RequestID={ownRequest}") == 1, "Completion cannot be duplicated");


        Check(Scalar("SELECT COUNT(*) FROM Equipments WHERE EquipmentID=1 AND Status='Tốt'") == 1, "Last completed request restores equipment status");
        int legacyUnassigned = (int)Scalar("SELECT RequestID FROM MaintenanceRequests WHERE AssignedTechnicianID IS NULL LIMIT 1");
        MaintenanceService.CompleteRequest(3, legacyUnassigned, "Claim old unassigned request", 0);
        Check(Scalar($"SELECT AssignedTechnicianID FROM MaintenanceRequests WHERE RequestID={legacyUnassigned}") == 3, "Legacy unassigned work can be claimed");
        MaintenanceService.CreateRequest(1, 3, "Admin request", "Thấp", otherTech);
        int adminRequest = (int)Scalar("SELECT MAX(RequestID) FROM MaintenanceRequests");
        MaintenanceService.CompleteRequest(1, adminRequest, "Admin override", 0);
        Check(Scalar($"SELECT COUNT(*) FROM MaintenanceLogs WHERE RequestID={adminRequest}") == 1, "Admin retains maintenance access");
    }

    private static void TestSafetyAndMoney()
    {
        foreach (string text in new[] { "1.950.000", "1950000", "1,950,000" })
            Check(VndMoney.TryParse(text, out double value) && value == 1950000 && VndMoney.Format(value) == "1.950.000", "VND normalization " + text);
        foreach (string text in new[] { "-1", "1.5", "1,95,000", "1.950,000", "NaN", "1e6", "", "9007199254740992" })
            Check(!VndMoney.TryParse(text, out _), "Reject invalid VND " + text);
        Throws<SQLiteException>(() => DatabaseHelper.ExecuteNonQuery("DELETE FROM Equipments WHERE EquipmentID=1"));
        Throws<SQLiteException>(() => DatabaseHelper.ExecuteNonQuery("DELETE FROM Labs WHERE LabID=1"));
        DatabaseHelper.ExecuteNonQuery("UPDATE Equipments SET Status='Thanh lý' WHERE EquipmentID=1");
        Throws<InvalidOperationException>(() => MaintenanceService.CreateRequest(3, 1, "Retired", "Thấp", 3));
        Throws<SQLiteException>(() => DatabaseHelper.ExecuteNonQuery("INSERT INTO MaintenanceRequests (EquipmentID, RequestedBy, RequestDate, IssueDescription) VALUES (1, 3, '2026-01-01', 'Retired')"));
        Check(Scalar("SELECT COUNT(*) FROM MaintenanceLogs WHERE EquipmentStatusAfter='Hỏng'") == 1, "Outcome stored independently in history");
        DatabaseHelper.ExecuteNonQuery("INSERT INTO Labs (LabCode, LabName) VALUES ('EMPTY', 'Empty'); DELETE FROM Labs WHERE LabCode='EMPTY'");
        Check(Scalar("SELECT COUNT(*) FROM Labs WHERE LabCode='EMPTY'") == 0, "Empty lab may be deleted");
        DatabaseHelper.ExecuteNonQuery(@"
            INSERT INTO Equipments (EquipmentCode, EquipmentName, LabID) VALUES ('DELETE-ME', 'No maintenance', 1);
            DELETE FROM Equipments WHERE EquipmentCode='DELETE-ME';");
        Check(Scalar("SELECT COUNT(*) FROM Equipments WHERE EquipmentCode='DELETE-ME'") == 0, "Equipment without requests may be deleted");
        MaintenanceService.CreateRequest(3, 2, "First pending repair", "Thấp", 3);
        MaintenanceService.CreateRequest(3, 3, "Second pending repair", "Thấp", 3);
        Throws<SQLiteException>(() => DatabaseHelper.ExecuteNonQuery("INSERT INTO MaintenanceRequests (EquipmentID, RequestedBy, RequestDate, IssueDescription) VALUES (2, 3, '2026-01-01', 'Duplicate')"));
        Throws<SQLiteException>(() => DatabaseHelper.ExecuteNonQuery("UPDATE Equipments SET Status='Thanh lý' WHERE EquipmentID=2"));
        Throws<SQLiteException>(() => DatabaseHelper.ExecuteNonQuery("UPDATE Equipments SET Status='Tốt' WHERE EquipmentID=2"));
        Throws<SQLiteException>(() => DatabaseHelper.ExecuteNonQuery("UPDATE Equipments SET Status='Hỏng' WHERE EquipmentID=2"));
        Check(Scalar("SELECT COUNT(*) FROM Equipments WHERE EquipmentID=2 AND Status='Đang bảo trì'") == 1, "Open repair preserves maintenance status");
        foreach (double value in new[] { double.NaN, double.PositiveInfinity, 1.5, VndMoney.MaxExactValue + 1 })
            Throws<ArgumentException>(() => MaintenanceService.CompleteRequest(3, (int)Scalar("SELECT MAX(RequestID) FROM MaintenanceRequests"), "Invalid cost", value));
    }

    private static void TestRoleScreensAndLogout()
    {
        foreach (var (id, role, count) in new[] { (3, "Technician", 4), (2, "LabManager", 5), (1, "Admin", 6) })
        {
            using var form = new Form1(id, role, role);
            form.Show();
            Application.DoEvents();
            Check(form.tabMain.TabPages.Count == count, role + " has the expected tab count");
            Check(form.tabMain.TabPages.Contains(form.tabUsers) == (role == "Admin"), role + " users tab access");
            Check(form.btnUserUpdate.Enabled == (role == "Admin") && form.btnLabUpdate.Enabled == (role != "Technician"), role + " update button permissions");
            Check(form.dgvEquipments.ReadOnly, role + " equipment grid is read-only");
            Check(form.dgvUsers.DataSource != null == (role == "Admin"), role + " users list is loaded only for admin");
            if (role == "Technician")
            {
                Check(!form.tabMain.TabPages.Contains(form.tabLabs) && !form.btnNavLabs.Visible, "Technician has no labs tab or navigation");
                Check(!form.pnlEqCard.Visible && !form.btnEqAdd.Enabled && !form.btnEqUpdate.Enabled && !form.btnEqDelete.Enabled, "Technician cannot edit equipment");
                Check(!form.lblPageTitle.Text.Contains("Quản lý"), "Technician equipment title omits management");
                Check(form.btnNavRequest.Top - form.btnNavEquipments.Top == form.btnNavHistory.Top - form.btnNavRequest.Top, "Technician menu has no gap");
            }
            form.btnNavRequest.PerformClick();
            Check(form.tabMain.SelectedTab == form.tabRequests, role + " request navigation works after tab removal");
            Check(form.cboReqTechnician.Visible == (role != "Technician"), role + " assignment selector visibility");
            Check(form.cboReqEquipment.Items.Count == 0, "Retired equipment and equipment with pending requests are excluded");
            if (role != "Technician")
            {
                Check(form.cboReqTechnician.Items.Count == 2 && form.cboReqTechnician.SelectedIndex == -1, "Assignment needs an explicit technician choice");
                Check(form.cboReqTechnician.Bottom < form.pnlReqCard.ClientSize.Height, "Assignment selector fits inside the request card");
            }
            form.tabMain.SelectedTab = form.tabHistory;
            Check(form.lblPageTitle.Text == "Xử lý và lịch sử bảo trì", "Direct tab selection updates the header");
            Check(form.pnlHisCard.Visible == (role != "LabManager"), role + " completion panel access");
            if (role != "LabManager")
            {
                Check(form.radAfterGood.Parent == form.radAfterBroken.Parent && form.radAfterBroken.Bottom < form.radAfterBroken.Parent!.ClientSize.Height,
                    "Repair outcomes share a group and fit inside it");
                form.radAfterBroken.Checked = true;
                form.txtLogAction.Text = "Notes for the previous request";
                form.txtLogCost.Text = "1.950.000";
                form.cboPendingRequests.SelectedIndex = 1;
                Check(!form.radAfterGood.Checked && !form.radAfterBroken.Checked && form.txtLogAction.Text == "" && form.txtLogCost.Text == "0",
                    "Changing request clears the previous repair outcome, notes and cost");
                Check(form.dgvLogs.Columns["Tình trạng thiết bị sau bảo trì"]!.Index == form.dgvLogs.Columns["Ngày hoàn thành"]!.Index + 1,
                    "Repair outcome follows the completion date in history");
            }
            form.btnNavStats.PerformClick();
            Check(form.tabMain.SelectedTab == form.tabStats, role + " statistics navigation works");
            form.btnLogout.PerformClick();
            Check(form.LogoutRequested && !form.Visible, role + " logout closes the session");
        }
        using var nextLogin = new FrmLogin();
        nextLogin.Shown += (_, _) => nextLogin.BeginInvoke(new Action(() => nextLogin.Close()));
        Check(nextLogin.ShowDialog() != DialogResult.OK, "Login can reopen and cancel after all sessions");
        using var closed = new Form1(3, "Technician", "Technician");
        closed.Show();
        closed.Close();
        Check(!closed.LogoutRequested, "Window close exits rather than logging out");
    }

    private static void TestHistoryScaling()
    {
        foreach (float scale in new[] { 1f, 1.25f, 1.5f, 2f })
        {
            using var form = new Form1(1, "Admin", "Admin");
            form.Show();
            form.tabMain.SelectedTab = form.tabHistory;
            form.Scale(new System.Drawing.SizeF(scale, scale));
            Application.DoEvents();
            var group = form.grpAfterCondition;
            Check(group.Top >= Math.Max(form.txtLogAction.Bottom, form.btnHisComplete.Bottom),
                $"Outcome group stays below repair notes and button at scale {scale}");
            Check(group.Bottom <= form.pnlHisCard.ClientSize.Height && form.dgvLogs.Top > form.pnlHisCard.Bottom,
                $"History grid does not cover completion controls at scale {scale}");
            Check(!form.radAfterGood.Bounds.IntersectsWith(form.radAfterBroken.Bounds)
                && group.ClientRectangle.Contains(form.radAfterBroken.Bounds),
                $"Radio options remain separate and contained at scale {scale}");
            Check(form.pnlHisCard.Region!.IsVisible(form.pnlHisCard.Width / 2, form.pnlHisCard.Height - 2),
                $"Rounded card region follows scaled height at scale {scale}");
            string? captures = Environment.GetEnvironmentVariable("LABCARE_LAYOUT_CAPTURE_DIR");
            if (!string.IsNullOrEmpty(captures))
            {
                Directory.CreateDirectory(captures);
                using var bitmap = new System.Drawing.Bitmap(form.pnlHisCard.Width, form.pnlHisCard.Height);
                form.pnlHisCard.DrawToBitmap(bitmap, form.pnlHisCard.ClientRectangle);
                bitmap.Save(Path.Combine(captures, $"history-scale-{scale}.png"));
            }
            form.Close();
        }
    }

    private static void TestWideWindowLayout()
    {
        using var form = new Form1(1, "Admin", "Admin");
        form.Show();
        // Hosted Windows runners can have a 1024x768 desktop. Start below its
        // working area so Windows does not clamp the requested larger size.
        var workingArea = Screen.FromControl(form).WorkingArea;
        form.ClientSize = new System.Drawing.Size(workingArea.Width - 250, workingArea.Height - 250);
        Application.DoEvents();
        var pages = new[] {
            (form.tabEquipments, form.pnlEqCard, form.dgvEquipments),
            (form.tabLabs, form.pnlLabCard, form.dgvLabs),
            (form.tabRequests, form.pnlReqCard, form.dgvRequests),
            (form.tabHistory, form.pnlHisCard, form.dgvLogs),
            (form.tabUsers, form.pnlUserCard, form.dgvUsers)
        };
        var initialSize = form.ClientSize;
        foreach (var (page, card, grid) in pages)
        {
            form.tabMain.SelectedTab = page;
            Application.DoEvents();
            var initialPageSize = page.ClientSize;
            var initialGridSize = grid.Size;
            int initialCardWidth = card.Width;
            form.ClientSize = new System.Drawing.Size(initialSize.Width + 150, initialSize.Height + 100);
            Application.DoEvents();
            int addedWidth = page.ClientSize.Width - initialPageSize.Width;
            int addedHeight = page.ClientSize.Height - initialPageSize.Height;
            Check(addedWidth > 0 && addedHeight > 0 && card.Width == initialCardWidth + addedWidth
                && grid.Width == initialGridSize.Width + addedWidth && grid.Height == initialGridSize.Height + addedHeight,
                $"Form and grid fill the larger window on {page.Text} (delta {addedWidth}x{addedHeight}, card {card.Width - initialCardWidth}, grid {grid.Width - initialGridSize.Width}x{grid.Height - initialGridSize.Height})");
            form.ClientSize = initialSize;
        }

        form.tabMain.SelectedTab = form.tabStats;
        form.ClientSize = new System.Drawing.Size(initialSize.Width + 150, initialSize.Height + 100);
        int rightEdge = form.tabStats.ClientSize.Width - form.pnlCardTotalEq.Left;
        Check(form.pnlCardCost.Right == rightEdge && form.dgvStatsLab.Right == rightEdge
            && form.dgvStatsStatus.Right < form.dgvStatsLab.Left,
            "Statistics cards and grids fill the available width without overlap");
    }

    private static void TestAdministration()
    {
        string labName = DatabaseHelper.GetDataTable("SELECT LabName FROM Labs WHERE LabID=2").Rows[0][0].ToString()!;
        long labDevices = Scalar("SELECT COUNT(*) FROM Equipments WHERE LabID=2");
        AdministrationService.UpdateLab(2, 2, "LAB-02", "Tên Lab mới", "Tầng 4");
        Check(DatabaseHelper.GetDataTable("SELECT LabName FROM Labs WHERE LabID=2").Rows[0][0].ToString() == "Tên Lab mới"
            && Scalar("SELECT COUNT(*) FROM Equipments WHERE LabID=2") == labDevices, "Manager edits lab details without changing equipment relations");
        Throws<UnauthorizedAccessException>(() => AdministrationService.UpdateLab(3, 2, "LAB-02", "Forbidden", ""));
        Throws<SQLiteException>(() => AdministrationService.UpdateLab(1, 2, "LAB-01", "Duplicate", ""));
        Throws<ArgumentException>(() => AdministrationService.UpdateLab(1, 2, "", "Empty code", ""));
        Throws<ArgumentException>(() => AdministrationService.UpdateLab(1, 2, "LAB-02", new string('x', InputLimits.Name + 1), ""));
        AdministrationService.UpdateLab(1, 2, "LAB-02", labName, "Tòa B - Tầng 3");

        Throws<UnauthorizedAccessException>(() => AdministrationService.CreateUser(2, "forbidden", "password", "Forbidden", "Technician"));
        Throws<ArgumentException>(() => AdministrationService.CreateUser(1, "empty-pass", "", "Empty", "Technician"));
        Throws<ArgumentException>(() => AdministrationService.CreateUser(1, "bad-role", "password", "Bad", "SuperAdmin"));
        Throws<ArgumentException>(() => AdministrationService.CreateUser(1, "long-pass", new string('x', InputLimits.Password + 1), "Long", "Technician"));
        AdministrationService.CreateUser(1, "service-edit", "first-password", "Before", "LabManager");
        int id = (int)Scalar("SELECT UserID FROM Users WHERE Username='service-edit'");
        string hash = DatabaseHelper.GetDataTable($"SELECT PasswordHash FROM Users WHERE UserID={id}").Rows[0][0].ToString()!;
        Check(PasswordHasher.Verify("first-password", hash), "New accounts store a usable password hash");
        AdministrationService.UpdateUser(1, id, "service-edited", "", "After", "Technician");
        Check(hash == DatabaseHelper.GetDataTable($"SELECT PasswordHash FROM Users WHERE UserID={id}").Rows[0][0].ToString()
            && Scalar($"SELECT COUNT(*) FROM Users WHERE UserID={id} AND Username='service-edited' AND FullName='After' AND Role='Technician'") == 1,
            "User edit updates name, username and role while empty password preserves the hash");
        AdministrationService.UpdateUser(1, id, "service-edited", " new-password ", "After", "Technician");
        string newHash = DatabaseHelper.GetDataTable($"SELECT PasswordHash FROM Users WHERE UserID={id}").Rows[0][0].ToString()!;
        Check(PasswordHasher.Verify(" new-password ", newHash) && !PasswordHasher.Verify("first-password", newHash), "Password reset stores a new hash and preserves spaces");
        Throws<UnauthorizedAccessException>(() => AdministrationService.UpdateUser(2, id, "service-edited", "", "Forbidden", "Technician"));
        Throws<InvalidOperationException>(() => AdministrationService.UpdateUser(1, 1, "admin", "", "Admin", "Technician"));
        Throws<InvalidOperationException>(() => AdministrationService.UpdateUser(1, 3, "ktv_01", "", "Tech", "LabManager"));
        Throws<SQLiteException>(() => AdministrationService.UpdateUser(1, id, "admin", "not-saved", "Duplicate", "Admin"));
        Check(newHash == DatabaseHelper.GetDataTable($"SELECT PasswordHash FROM Users WHERE UserID={id}").Rows[0][0].ToString(),
            "Failed duplicate-username edit preserves the original password hash");
        AdministrationService.CreateUser(1, "second-admin", "password", "Second admin", "Admin");
        int secondAdmin = (int)Scalar("SELECT UserID FROM Users WHERE Username='second-admin'");
        Throws<InvalidOperationException>(() => AdministrationService.UpdateUser(secondAdmin, secondAdmin, "second-admin", "", "Second admin", "Technician"));
        DatabaseHelper.ExecuteNonQuery($"DELETE FROM Users WHERE UserID IN ({id}, {secondAdmin})");
    }

    private static void TestInputAndEditScreens()
    {
        using var form = new Form1(1, "Admin", "Admin");
        form.Show();
        Application.DoEvents();
        var limits = new[] {
            (form.txtEqCode, InputLimits.Code), (form.txtEqName, InputLimits.Name), (form.txtEqCategory, InputLimits.Category),
            (form.txtLabCode, InputLimits.Code), (form.txtLabName, InputLimits.Name), (form.txtLabLocation, InputLimits.Location),
            (form.txtReqIssue, InputLimits.Description), (form.txtLogAction, InputLimits.Description), (form.txtLogCost, InputLimits.Money),
            (form.txtNewUser, InputLimits.Username), (form.txtNewPass, InputLimits.Password), (form.txtNewFullName, InputLimits.Name)
        };
        Check(limits.All(pair => pair.Item1.MaxLength == pair.Item2), "All main-screen textboxes have the intended length limits");
        using var login = new FrmLogin();
        var inputs = login.Controls.OfType<Panel>().Single().Controls.OfType<TextBox>().ToArray();
        Check(inputs.All(input => input.MaxLength == (input.PasswordChar == '\0' ? InputLimits.Username : InputLimits.Password)), "Login input lengths match account creation limits");
        Check(form.txtNewPass.PasswordChar != '\0', "User management masks passwords");
        form.tabMain.SelectedTab = form.tabLabs;
        var labRow = form.dgvLabs.Rows.Cast<DataGridViewRow>().Single(row => Convert.ToInt32(row.Cells["LabID"].Value) == 2);
        form.dgvLabs.CurrentCell = labRow.Cells["Tên Phòng Lab"];
        Check(form.txtLabCode.Text == "LAB-02", "Selecting a lab loads its editable fields");
        string labName = form.txtLabName.Text;
        form.txtLabName.Text = "Lab cập nhật từ UI";
        form.btnLabUpdate.PerformClick();
        Check(DatabaseHelper.GetDataTable("SELECT LabName FROM Labs WHERE LabID=2").Rows[0][0].ToString() == "Lab cập nhật từ UI", "Lab update button saves the selected lab");
        AdministrationService.UpdateLab(1, 2, "LAB-02", labName, "Tòa B - Tầng 3");

        form.tabMain.SelectedTab = form.tabUsers;
        // Create through the UI, then select and edit that new account.
        form.txtNewUser.Text = "ui-edit";
        form.txtNewPass.Text = "first-password";
        form.txtNewFullName.Text = "UI before";
        form.cboNewRole.Text = "Technician";
        form.btnUserAdd.PerformClick();
        int id = (int)Scalar("SELECT UserID FROM Users WHERE Username='ui-edit'");
        void SelectUser(int userId)
        {
            var row = form.dgvUsers.Rows.Cast<DataGridViewRow>().Single(row => Convert.ToInt32(row.Cells["UserID"].Value) == userId);
            form.dgvUsers.CurrentCell = row.Cells["Tài khoản"];
        }
        SelectUser(id);
        Check(form.txtNewUser.Text == "ui-edit" && form.txtNewPass.Text == "", "Selecting a user loads fields but never its password");
        string hash = DatabaseHelper.GetDataTable($"SELECT PasswordHash FROM Users WHERE UserID={id}").Rows[0][0].ToString()!;
        form.txtNewFullName.Text = "UI after";
        form.btnUserUpdate.PerformClick();
        Check(hash == DatabaseHelper.GetDataTable($"SELECT PasswordHash FROM Users WHERE UserID={id}").Rows[0][0].ToString()
            && Scalar($"SELECT COUNT(*) FROM Users WHERE UserID={id} AND FullName='UI after'") == 1, "UI user edit preserves the password when left empty");
        SelectUser(id);
        form.txtNewPass.Text = "reset-password";
        form.btnUserUpdate.PerformClick();
        Check(PasswordHasher.Verify("reset-password", DatabaseHelper.GetDataTable($"SELECT PasswordHash FROM Users WHERE UserID={id}").Rows[0][0].ToString()!), "UI password reset hashes the new password");
        SelectUser(id);
        form.txtNewPass.Text = "must-not-carry";
        SelectUser(1);
        Check(form.txtNewPass.Text == "", "Selecting another user clears an unsaved password");
        foreach (var (card, grid, buttons) in new[] {
            (form.pnlLabCard, form.dgvLabs, new[] { form.btnLabAdd, form.btnLabUpdate, form.btnLabDelete }),
            (form.pnlUserCard, form.dgvUsers, new[] { form.btnUserAdd, form.btnUserUpdate, form.btnUserDelete })
        })
            Check(grid.Top > card.Bottom && buttons.All(button => card.ClientRectangle.Contains(button.Bounds))
                && !buttons[0].Bounds.IntersectsWith(buttons[1].Bounds) && !buttons[1].Bounds.IntersectsWith(buttons[2].Bounds), "Edit buttons fit inside their card without covering the data grid");
        DatabaseHelper.ExecuteNonQuery($"DELETE FROM Users WHERE UserID={id}");
    }

    private static void TestEquipmentEditingAndStatistics()
    {
        using var form = new Form1(1, "Admin", "Admin");
        form.Show();
        Application.DoEvents();
        var row = form.dgvEquipments.Rows.Cast<DataGridViewRow>()
            .Single(row => Convert.ToInt32(row.Cells["EquipmentID"].Value) == 3);
        form.dgvEquipments.CurrentCell = row.Cells["Tên Thiết Bị"];
        Check(Convert.ToInt32(form.cboEqLab.SelectedValue) == 2, "Selecting equipment selects its actual lab");
        string name = form.txtEqName.Text;
        form.txtEqName.Text = name + " updated";
        form.btnEqUpdate.PerformClick();
        Check(Scalar("SELECT LabID FROM Equipments WHERE EquipmentID=3") == 2, "Editing a name preserves equipment lab");
        Check(DatabaseHelper.GetDataTable("SELECT EquipmentName FROM Equipments WHERE EquipmentID=3").Rows[0][0].ToString() == name + " updated", "Equipment edit is saved");
        long expected = Scalar("SELECT COUNT(*) FROM Equipments WHERE Status IN ('Hỏng', 'Đang bảo trì')");
        Check(form.lblCardBrokenVal.Text == $"{expected} Thiết bị", "Maintenance statistics exclude retired equipment");
        Check(form.dgvEquipments.Columns["LabID"] is { Visible: false }, "Internal lab key is hidden");
    }

    private static HashSet<int> PendingIds(int userId) => MaintenanceService.GetPendingRequests(userId).Rows
        .Cast<System.Data.DataRow>().Select(row => Convert.ToInt32(row["RequestID"])).ToHashSet();

    private static long Scalar(string sql) => Convert.ToInt64(DatabaseHelper.GetDataTable(sql).Rows[0][0]);

    private static void Check(bool condition, string description)
    {
        if (!condition) throw new Exception("FAIL: " + description);
        checks++;
        Console.WriteLine("PASS: " + description);
    }

    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { checks++; return; }
        throw new Exception("Expected " + typeof(T).Name);
    }
}
