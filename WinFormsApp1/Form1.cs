using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace LabMaintenanceApp
{
    public partial class Form1 : Form
    {
        private int currentUserId = 1;
        private string currentUserName = "Quản trị hệ thống";
        private string currentUserRole = "Admin";

        public Form1() : this(1, "Quản trị hệ thống", "Admin") { }

        public Form1(int userId, string fullName, string role)
        {
            InitializeComponent();
            currentUserId = userId;
            currentUserName = fullName;
            currentUserRole = role;

            ApplySoftModernUI();
            BindEvents();
            LoadAllData();
        }

        // =====================================================================
        // HÀM LÀM ĐẸP & BO TRÒN GÓC MỀM MẠI (SOFT MODERN UI)
        // =====================================================================
        private void ApplySoftModernUI()
        {
            lblUserInfo.Text = $"👤 {currentUserName}\r\n🔑 Vai trò: {currentUserRole}";
            lblRoleBadge.Text = $"Quyền: {currentUserRole}";

            if (currentUserRole != "Admin")
            {
                btnNavUsers.Visible = false;
            }

            cboEqStatus.SelectedIndex = 0;
            cboReqPriority.SelectedIndex = 1;
            cboNewRole.SelectedIndex = 1;

            // Bo góc mềm mại cho các thẻ Card và Nút bấm
            MakeRounded(pnlEqCard, 14);
            MakeRounded(pnlLabCard, 14);
            MakeRounded(pnlReqCard, 14);
            MakeRounded(pnlHisCard, 14);
            MakeRounded(pnlUserCard, 14);
            MakeRounded(pnlCardTotalEq, 16);
            MakeRounded(pnlCardBroken, 16);
            MakeRounded(pnlCardCost, 16);
            MakeRounded(lblRoleBadge, 14);

            Button[] actionButtons = {
                btnNavEquipments, btnNavLabs, btnNavRequest, btnNavHistory, btnNavStats, btnNavUsers,
                btnEqAdd, btnEqUpdate, btnEqDelete, btnLabAdd, btnLabDelete,
                btnReqSubmit, btnHisComplete, btnUserAdd, btnUserDelete
            };
            foreach (var btn in actionButtons)
            {
                MakeRounded(btn, 10);
                btn.Cursor = Cursors.Hand;
            }

            // Làm phẳng và mịn toàn bộ bảng dữ liệu (DataGridView)
            DataGridView[] grids = { dgvEquipments, dgvLabs, dgvRequests, dgvLogs, dgvStatsStatus, dgvStatsLab, dgvUsers };
            foreach (var dgv in grids)
            {
                StyleModernGrid(dgv);
            }
        }

        public static void MakeRounded(Control ctrl, int radius)
        {
            if (ctrl.Width <= 0 || ctrl.Height <= 0) return;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(ctrl.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(ctrl.Width - radius, ctrl.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, ctrl.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                ctrl.Region = new Region(path);
            }
        }

        private void StyleModernGrid(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Color.White;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Color.FromArgb(226, 232, 240);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(71, 85, 105);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 38;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgv.RowTemplate.Height = 34;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.RowHeadersVisible = false;
            MakeRounded(dgv, 14);
        }

        private void SwitchTab(int index, string title, Button activeBtn)
        {
            tabMain.SelectedIndex = index;
            lblPageTitle.Text = title;

            Button[] navs = { btnNavEquipments, btnNavLabs, btnNavRequest, btnNavHistory, btnNavStats, btnNavUsers };
            foreach (var b in navs)
            {
                b.BackColor = Color.FromArgb(30, 41, 59);
                b.ForeColor = Color.FromArgb(203, 213, 225);
            }
            activeBtn.BackColor = Color.FromArgb(59, 130, 246);
            activeBtn.ForeColor = Color.White;
        }

        // =====================================================================
        // XỬ LÝ SỰ KIỆN NÚT BẤM & CRUD 6 CHỨC NĂNG
        // =====================================================================
        private void BindEvents()
        {
            // Chuyển trang qua thanh Sidebar bên trái
            btnNavEquipments.Click += (s, e) => SwitchTab(0, "Quản Lý Thiết Bị Phòng Lab", btnNavEquipments);
            btnNavLabs.Click += (s, e) => SwitchTab(1, "Quản Lý Danh Mục Phòng Lab", btnNavLabs);
            btnNavRequest.Click += (s, e) => SwitchTab(2, "Tạo Phiếu Yêu Cầu Bảo Trì", btnNavRequest);
            btnNavHistory.Click += (s, e) => SwitchTab(3, "Xử Lý & Lịch Sử Bảo Trì", btnNavHistory);
            btnNavStats.Click += (s, e) => SwitchTab(4, "Báo Cáo & Thống Kê Tổng Hợp", btnNavStats);
            btnNavUsers.Click += (s, e) => SwitchTab(5, "Quản Lý Tài Khoản Người Dùng", btnNavUsers);

            // Click vào dòng trên bảng Thiết bị -> Đổ dữ liệu lên ô nhập để Sửa/Xóa
            dgvEquipments.SelectionChanged += (s, e) =>
            {
                if (dgvEquipments.CurrentRow != null)
                {
                    txtEqCode.Text = dgvEquipments.CurrentRow.Cells["Mã TB"].Value?.ToString();
                    txtEqName.Text = dgvEquipments.CurrentRow.Cells["Tên Thiết Bị"].Value?.ToString();
                    txtEqCategory.Text = dgvEquipments.CurrentRow.Cells["Danh mục"].Value?.ToString();
                    cboEqStatus.Text = dgvEquipments.CurrentRow.Cells["Trạng thái"].Value?.ToString();
                }
            };

            // 1. Thêm thiết bị
            btnEqAdd.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtEqCode.Text) || string.IsNullOrWhiteSpace(txtEqName.Text) || cboEqLab.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đủ Mã, Tên thiết bị và Phòng Lab!", "Thông báo");
                    return;
                }
                try
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "INSERT INTO Equipments (EquipmentCode, EquipmentName, Category, LabID, Status) VALUES (@c, @n, @cat, @l, @s)",
                        new SQLiteParameter("@c", txtEqCode.Text.Trim()),
                        new SQLiteParameter("@n", txtEqName.Text.Trim()),
                        new SQLiteParameter("@cat", txtEqCategory.Text.Trim()),
                        new SQLiteParameter("@l", cboEqLab.SelectedValue),
                        new SQLiteParameter("@s", cboEqStatus.Text));
                    txtEqCode.Clear(); txtEqName.Clear(); txtEqCategory.Clear();
                    LoadAllData();
                }
                catch (Exception ex) { MessageBox.Show("Không thể thêm (trùng mã TB): " + ex.Message); }
            };

            // 2. Sửa thiết bị
            btnEqUpdate.Click += (s, e) =>
            {
                if (dgvEquipments.CurrentRow == null) return;
                int id = Convert.ToInt32(dgvEquipments.CurrentRow.Cells["EquipmentID"].Value);
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Equipments SET EquipmentCode=@c, EquipmentName=@n, Category=@cat, LabID=@l, Status=@s WHERE EquipmentID=@id",
                    new SQLiteParameter("@c", txtEqCode.Text.Trim()),
                    new SQLiteParameter("@n", txtEqName.Text.Trim()),
                    new SQLiteParameter("@cat", txtEqCategory.Text.Trim()),
                    new SQLiteParameter("@l", cboEqLab.SelectedValue),
                    new SQLiteParameter("@s", cboEqStatus.Text),
                    new SQLiteParameter("@id", id));
                LoadAllData();
            };

            // 3. Xóa thiết bị
            btnEqDelete.Click += (s, e) =>
            {
                if (dgvEquipments.CurrentRow == null) return;
                int id = Convert.ToInt32(dgvEquipments.CurrentRow.Cells["EquipmentID"].Value);
                DatabaseHelper.ExecuteNonQuery("DELETE FROM Equipments WHERE EquipmentID=@id", new SQLiteParameter("@id", id));
                LoadAllData();
            };

            // 4. Thêm & Xóa Phòng Lab
            btnLabAdd.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtLabCode.Text) || string.IsNullOrWhiteSpace(txtLabName.Text)) return;
                try
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "INSERT INTO Labs (LabCode, LabName, Location) VALUES (@c, @n, @loc)",
                        new SQLiteParameter("@c", txtLabCode.Text.Trim()),
                        new SQLiteParameter("@n", txtLabName.Text.Trim()),
                        new SQLiteParameter("@loc", txtLabLocation.Text.Trim()));
                    txtLabCode.Clear(); txtLabName.Clear(); txtLabLocation.Clear();
                    LoadAllData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi thêm phòng Lab: " + ex.Message); }
            };

            btnLabDelete.Click += (s, e) =>
            {
                if (dgvLabs.CurrentRow == null) return;
                int id = Convert.ToInt32(dgvLabs.CurrentRow.Cells["LabID"].Value);
                DatabaseHelper.ExecuteNonQuery("DELETE FROM Labs WHERE LabID=@id", new SQLiteParameter("@id", id));
                LoadAllData();
            };

            // 5. Gửi yêu cầu bảo trì
            btnReqSubmit.Click += (s, e) =>
            {
                if (cboReqEquipment.SelectedValue == null || string.IsNullOrWhiteSpace(txtReqIssue.Text))
                {
                    MessageBox.Show("Vui lòng chọn thiết bị và nhập mô tả sự cố!");
                    return;
                }
                int eqId = Convert.ToInt32(cboReqEquipment.SelectedValue);
                DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO MaintenanceRequests (EquipmentID, RequestedBy, RequestDate, IssueDescription, Priority, Status) VALUES (@eq, @u, @d, @iss, @p, 'Chờ xử lý')",
                    new SQLiteParameter("@eq", eqId),
                    new SQLiteParameter("@u", currentUserId),
                    new SQLiteParameter("@d", DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                    new SQLiteParameter("@iss", txtReqIssue.Text.Trim()),
                    new SQLiteParameter("@p", cboReqPriority.Text));
                DatabaseHelper.ExecuteNonQuery("UPDATE Equipments SET Status='Đang bảo trì' WHERE EquipmentID=@eq", new SQLiteParameter("@eq", eqId));
                txtReqIssue.Clear();
                MessageBox.Show("Đã tạo phiếu bảo trì thành công!");
                LoadAllData();
            };

            // 6. Hoàn tất bảo trì
            btnHisComplete.Click += (s, e) =>
            {
                if (cboPendingRequests.SelectedValue == null || string.IsNullOrWhiteSpace(txtLogAction.Text))
                {
                    MessageBox.Show("Vui lòng chọn phiếu chờ và nhập kết quả sửa chữa!");
                    return;
                }
                int reqId = Convert.ToInt32(cboPendingRequests.SelectedValue);
                double.TryParse(txtLogCost.Text.Trim(), out double cost);

                DatabaseHelper.ExecuteNonQuery(
                    "INSERT INTO MaintenanceLogs (RequestID, TechnicianID, CompletedDate, ActionTaken, Cost) VALUES (@r, @t, @d, @act, @c)",
                    new SQLiteParameter("@r", reqId),
                    new SQLiteParameter("@t", currentUserId),
                    new SQLiteParameter("@d", DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                    new SQLiteParameter("@act", txtLogAction.Text.Trim()),
                    new SQLiteParameter("@c", cost));
                DatabaseHelper.ExecuteNonQuery("UPDATE MaintenanceRequests SET Status='Hoàn thành' WHERE RequestID=@r", new SQLiteParameter("@r", reqId));
                DatabaseHelper.ExecuteNonQuery("UPDATE Equipments SET Status='Tốt' WHERE EquipmentID = (SELECT EquipmentID FROM MaintenanceRequests WHERE RequestID=@r)", new SQLiteParameter("@r", reqId));

                txtLogAction.Clear(); txtLogCost.Text = "0";
                MessageBox.Show("Đã hoàn tất bảo trì, thiết bị đã hoạt động tốt trở lại!");
                LoadAllData();
            };

            // 7. Thêm & Xóa Người dùng
            btnUserAdd.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtNewUser.Text) || string.IsNullOrWhiteSpace(txtNewPass.Text)) return;
                try
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "INSERT INTO Users (Username, PasswordHash, FullName, Role) VALUES (@u, @p, @f, @r)",
                        new SQLiteParameter("@u", txtNewUser.Text.Trim()),
                        new SQLiteParameter("@p", txtNewPass.Text.Trim()),
                        new SQLiteParameter("@f", txtNewFullName.Text.Trim()),
                        new SQLiteParameter("@r", cboNewRole.Text));
                    txtNewUser.Clear(); txtNewPass.Clear(); txtNewFullName.Clear();
                    LoadAllData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi tạo tài khoản: " + ex.Message); }
            };

            btnUserDelete.Click += (s, e) =>
            {
                if (dgvUsers.CurrentRow == null) return;
                int id = Convert.ToInt32(dgvUsers.CurrentRow.Cells["UserID"].Value);
                if (id == 1) { MessageBox.Show("Không thể xóa Admin mặc định!"); return; }
                DatabaseHelper.ExecuteNonQuery("DELETE FROM Users WHERE UserID=@id", new SQLiteParameter("@id", id));
                LoadAllData();
            };
        }

        private void LoadAllData()
        {
            var dtLabs = DatabaseHelper.GetDataTable("SELECT LabID, LabCode AS 'Mã Phòng', LabName AS 'Tên Phòng Lab', Location AS 'Vị trí', Status AS 'Trạng thái' FROM Labs");
            dgvLabs.DataSource = dtLabs;
            cboEqLab.DataSource = dtLabs.Copy();
            cboEqLab.DisplayMember = "Tên Phòng Lab";
            cboEqLab.ValueMember = "LabID";

            var dtEq = DatabaseHelper.GetDataTable(@"
                SELECT e.EquipmentID, e.EquipmentCode AS 'Mã TB', e.EquipmentName AS 'Tên Thiết Bị', 
                       e.Category AS 'Danh mục', l.LabName AS 'Phòng Lab', e.Status AS 'Trạng thái'
                FROM Equipments e LEFT JOIN Labs l ON e.LabID = l.LabID");
            dgvEquipments.DataSource = dtEq;
            cboReqEquipment.DataSource = dtEq.Copy();
            cboReqEquipment.DisplayMember = "Tên Thiết Bị";
            cboReqEquipment.ValueMember = "EquipmentID";

            dgvRequests.DataSource = DatabaseHelper.GetDataTable(@"
                SELECT r.RequestID, e.EquipmentCode || ' - ' || e.EquipmentName AS 'Thiết bị', 
                       u.FullName AS 'Người báo', r.RequestDate AS 'Ngày tạo', 
                       r.IssueDescription AS 'Mô tả lỗi', r.Priority AS 'Ưu tiên', r.Status AS 'Trạng thái'
                FROM MaintenanceRequests r
                JOIN Equipments e ON r.EquipmentID = e.EquipmentID
                JOIN Users u ON r.RequestedBy = u.UserID ORDER BY r.RequestID DESC");

            var dtPending = DatabaseHelper.GetDataTable(@"
                SELECT r.RequestID, ('[Phiếu #' || r.RequestID || '] ' || e.EquipmentName || ' - ' || r.IssueDescription) AS DisplayText
                FROM MaintenanceRequests r JOIN Equipments e ON r.EquipmentID = e.EquipmentID
                WHERE r.Status = 'Chờ xử lý'");
            cboPendingRequests.DataSource = dtPending;
            cboPendingRequests.DisplayMember = "DisplayText";
            cboPendingRequests.ValueMember = "RequestID";

            dgvLogs.DataSource = DatabaseHelper.GetDataTable(@"
                SELECT l.LogID AS 'Mã LS', e.EquipmentName AS 'Thiết bị', u.FullName AS 'Kỹ thuật viên',
                       l.CompletedDate AS 'Ngày hoàn thành', l.ActionTaken AS 'Kết quả xử lý', l.Cost AS 'Chi phí (VNĐ)'
                FROM MaintenanceLogs l
                JOIN MaintenanceRequests r ON l.RequestID = r.RequestID
                JOIN Equipments e ON r.EquipmentID = e.EquipmentID
                JOIN Users u ON l.TechnicianID = u.UserID ORDER BY l.LogID DESC");

            dgvStatsStatus.DataSource = DatabaseHelper.GetDataTable("SELECT Status AS 'Trạng thái', COUNT(*) AS 'Số lượng' FROM Equipments GROUP BY Status");
            dgvStatsLab.DataSource = DatabaseHelper.GetDataTable("SELECT l.LabName AS 'Phòng Lab', COUNT(e.EquipmentID) AS 'Số thiết bị' FROM Labs l LEFT JOIN Equipments e ON l.LabID = e.LabID GROUP BY l.LabName");

            long totalEq = Convert.ToInt64(DatabaseHelper.GetDataTable("SELECT COUNT(*) FROM Equipments").Rows[0][0]);
            long brokenEq = Convert.ToInt64(DatabaseHelper.GetDataTable("SELECT COUNT(*) FROM Equipments WHERE Status != 'Tốt'").Rows[0][0]);
            double totalCost = Convert.ToDouble(DatabaseHelper.GetDataTable("SELECT IFNULL(SUM(Cost), 0) FROM MaintenanceLogs").Rows[0][0]);

            lblCardTotalVal.Text = $"{totalEq} Thiết bị";
            lblCardBrokenVal.Text = $"{brokenEq} Thiết bị";
            lblCardCostVal.Text = $"{totalCost:N0} VNĐ";

            dgvUsers.DataSource = DatabaseHelper.GetDataTable("SELECT UserID, Username AS 'Tài khoản', FullName AS 'Họ và tên', Role AS 'Vai trò' FROM Users");
        }
    }
}