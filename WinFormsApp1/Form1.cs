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
        public RadioButton radAfterGood = new RadioButton { Text = "Sửa thành công - thiết bị tốt", AutoSize = true, Left = 15, Top = 23 };
        public RadioButton radAfterBroken = new RadioButton { Text = "Chưa sửa được - thiết bị hỏng", AutoSize = true, Left = 340, Top = 23 };
        private int currentUserId = 1;
        private string currentUserName = "Quản trị hệ thống";
        private string currentUserRole = "Admin";

        public bool LogoutRequested { get; private set; }

        public Form1() : this(1, "Quản trị hệ thống", "Admin") { }

        public Form1(int userId, string fullName, string role)
        {
            InitializeComponent();
            txtLogCost.Leave += (_, _) => { if (VndMoney.TryParse(txtLogCost.Text, out double amount)) txtLogCost.Text = VndMoney.Format(amount); };
            // Không mang kết quả của phiếu trước sang thiết bị vừa chọn.
            cboPendingRequests.SelectedValueChanged += (_, _) =>
            {
                radAfterGood.Checked = radAfterBroken.Checked = false;
                txtLogAction.Clear();
                txtLogCost.Text = "0";
            };
            currentUserId = userId;
            currentUserName = fullName;
            currentUserRole = role;

            ApplySoftModernUI();
            tabStats.ClientSizeChanged += (_, _) => LayoutStatistics();
            LayoutStatistics();
            BindEvents();
            LoadAllData();
            UpdateNavigation();
        }

        private void LayoutStatistics()
        {
            // Dùng lề đã được WinForms scale để các cột có khoảng cách nhất quán.
            int margin = pnlCardTotalEq.Left;
            int gap = margin;
            int width = tabStats.ClientSize.Width - 2 * margin;
            if (width <= 2 * gap) return;

            int cardWidth = (width - 2 * gap) / 3;
            pnlCardTotalEq.Width = cardWidth;
            pnlCardBroken.Left = pnlCardTotalEq.Right + gap;
            pnlCardBroken.Width = cardWidth;
            pnlCardCost.Left = pnlCardBroken.Right + gap;
            pnlCardCost.Width = margin + width - pnlCardCost.Left;

            dgvStatsStatus.Width = (width - gap) / 2;
            dgvStatsLab.Left = dgvStatsStatus.Right + gap;
            dgvStatsLab.Width = margin + width - dgvStatsLab.Left;
        }

        // =====================================================================
        // HÀM LÀM ĐẸP & BO TRÒN GÓC MỀM MẠI (SOFT MODERN UI)
        // =====================================================================
        private void ApplySoftModernUI()
        {
            lblUserInfo.Text = $"👤 {currentUserName}\r\n🔑 Vai trò: {currentUserRole}";
            lblRoleBadge.Text = $"Quyền: {currentUserRole}";

            bool canManageUsers = RolePermissions.CanManageUsers(currentUserRole);
            bool canManageEquipment = RolePermissions.CanManageEquipment(currentUserRole);
            bool canManageLabs = RolePermissions.CanManageLabs(currentUserRole);
            bool canAssign = RolePermissions.CanAssignTechnician(currentUserRole);
            bool canComplete = RolePermissions.CanCompleteMaintenance(currentUserRole);

            // Xóa cả TabPage để không thể mở bằng chuột hoặc Ctrl+Tab.
            if (!canManageUsers) tabMain.TabPages.Remove(tabUsers);
            if (!canManageLabs) tabMain.TabPages.Remove(tabLabs);
            btnNavUsers.Visible = canManageUsers;
            btnNavLabs.Visible = canManageLabs;
            btnUserAdd.Enabled = btnUserUpdate.Enabled = btnUserDelete.Enabled = canManageUsers;
            btnLabAdd.Enabled = btnLabUpdate.Enabled = btnLabDelete.Enabled = canManageLabs;
            btnEqAdd.Enabled = btnEqUpdate.Enabled = btnEqDelete.Enabled = canManageEquipment;
            pnlEqCard.Visible = canManageEquipment;
            if (!canManageEquipment)
            {
                int reclaimedHeight = dgvEquipments.Top - pnlEqCard.Top;
                dgvEquipments.Top = pnlEqCard.Top;
                dgvEquipments.Height += reclaimedHeight;
            }
            pnlHisCard.Visible = canComplete;
            btnHisComplete.Enabled = canComplete;
            if (!canComplete)
            {
                int reclaimedHeight = dgvLogs.Top - pnlHisCard.Top;
                dgvLogs.Top = pnlHisCard.Top;
                dgvLogs.Height += reclaimedHeight;
            }
            lblReqTechnician.Visible = cboReqTechnician.Visible = canAssign;
            if (!canAssign)
            {
                int compactHeight = Math.Max(btnReqSubmit.Bottom, txtReqIssue.Bottom) + cboReqEquipment.Top;
                int reclaimedHeight = pnlReqCard.Height - compactHeight;
                pnlReqCard.Height = compactHeight;
                dgvRequests.Top -= reclaimedHeight;
                dgvRequests.Height += reclaimedHeight;
            }

            tabEquipments.Text = "Thiết bị phòng Lab";
            tabLabs.Text = "Phòng thí nghiệm";
            tabRequests.Text = "Tạo phiếu bảo trì";
            tabHistory.Text = "Xử lý và lịch sử";
            tabStats.Text = "Thống kê báo cáo";
            tabUsers.Text = "Người dùng";

            int navTop = btnNavEquipments.Top;
            int navSpacing = btnNavLabs.Top - btnNavEquipments.Top;
            foreach (var button in new[] { btnNavEquipments, btnNavLabs, btnNavRequest, btnNavHistory, btnNavStats, btnNavUsers })
            {
                // Visible phụ thuộc cả Form cha (chưa Show), nên dùng quyền trực tiếp.
                if ((button == btnNavLabs && !canManageLabs) || (button == btnNavUsers && !canManageUsers)) continue;
                button.Top = navTop;
                navTop += navSpacing;
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
                btnEqAdd, btnEqUpdate, btnEqDelete, btnLabAdd, btnLabUpdate, btnLabDelete,
                btnReqSubmit, btnHisComplete, btnUserAdd, btnUserUpdate, btnUserDelete, btnLogout
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
            ApplyRoundedRegion(ctrl, radius);
            ctrl.SizeChanged += (_, _) => ApplyRoundedRegion(ctrl, radius);
        }

        private static void ApplyRoundedRegion(Control ctrl, int radius)
        {
            if (ctrl.Width <= 0 || ctrl.Height <= 0) return;
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(ctrl.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(ctrl.Width - radius, ctrl.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, ctrl.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                var previousRegion = ctrl.Region;
                ctrl.Region = new Region(path);
                previousRegion?.Dispose();
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

        private void SwitchTab(TabPage page)
        {
            if (!tabMain.TabPages.Contains(page)) return;
            tabMain.SelectedTab = page;
            UpdateNavigation();
        }

        private void UpdateNavigation()
        {
            var pages = new[] { tabEquipments, tabLabs, tabRequests, tabHistory, tabStats, tabUsers };
            var navs = new[] { btnNavEquipments, btnNavLabs, btnNavRequest, btnNavHistory, btnNavStats, btnNavUsers };
            var titles = new[] {
                RolePermissions.CanManageEquipment(currentUserRole) ? "Quản lý thiết bị phòng Lab" : "Thiết bị phòng Lab",
                "Quản lý danh mục phòng Lab", "Tạo phiếu yêu cầu bảo trì", "Xử lý và lịch sử bảo trì",
                "Báo cáo và thống kê tổng hợp", "Quản lý tài khoản người dùng"
            };
            for (int i = 0; i < pages.Length; i++)
            {
                bool active = tabMain.SelectedTab == pages[i];
                navs[i].BackColor = active ? Color.FromArgb(59, 130, 246) : Color.FromArgb(30, 41, 59);
                navs[i].ForeColor = active ? Color.White : Color.FromArgb(203, 213, 225);
                if (active) lblPageTitle.Text = titles[i];
            }
        }

        private bool RequirePermission(bool allowed)
        {
            if (!allowed) MessageBox.Show("Bạn không có quyền thực hiện chức năng này.", "Phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return allowed;
        }

        // =====================================================================
        // XỬ LÝ SỰ KIỆN NÚT BẤM & CRUD 6 CHỨC NĂNG
        // =====================================================================
        private void BindEvents()
        {
            // Chuyển trang qua thanh Sidebar bên trái
            btnNavEquipments.Click += (s, e) => SwitchTab(tabEquipments);
            btnNavLabs.Click += (s, e) => SwitchTab(tabLabs);
            btnNavRequest.Click += (s, e) => SwitchTab(tabRequests);
            btnNavHistory.Click += (s, e) => SwitchTab(tabHistory);
            btnNavStats.Click += (s, e) => SwitchTab(tabStats);
            btnNavUsers.Click += (s, e) => SwitchTab(tabUsers);
            tabMain.SelectedIndexChanged += (s, e) => UpdateNavigation();
            btnLogout.Click += (s, e) =>
            {
                LogoutRequested = true;
                Close();
            };

            // Click vào dòng trên bảng Thiết bị -> Đổ dữ liệu lên ô nhập để Sửa/Xóa
            // CurrentCellChanged fires after CurrentRow points to the new device.
            dgvEquipments.CurrentCellChanged += (s, e) =>
            {
                if (dgvEquipments.CurrentRow != null)
                {
                    txtEqCode.Text = dgvEquipments.CurrentRow.Cells["Mã TB"].Value?.ToString();
                    txtEqName.Text = dgvEquipments.CurrentRow.Cells["Tên Thiết Bị"].Value?.ToString();
                    txtEqCategory.Text = dgvEquipments.CurrentRow.Cells["Danh mục"].Value?.ToString();
                    cboEqStatus.Text = dgvEquipments.CurrentRow.Cells["Trạng thái"].Value?.ToString();
                    if (RolePermissions.CanManageEquipment(currentUserRole) && dgvEquipments.CurrentRow.Cells["LabID"].Value is object labId)
                        cboEqLab.SelectedValue = labId;
                }
            };

            dgvLabs.CurrentCellChanged += (_, _) =>
            {
                if (dgvLabs.CurrentRow == null) return;
                txtLabCode.Text = dgvLabs.CurrentRow.Cells["Mã Phòng"].Value?.ToString();
                txtLabName.Text = dgvLabs.CurrentRow.Cells["Tên Phòng Lab"].Value?.ToString();
                txtLabLocation.Text = dgvLabs.CurrentRow.Cells["Vị trí"].Value?.ToString();
            };
            dgvUsers.CurrentCellChanged += (_, _) =>
            {
                txtNewPass.Clear(); // Never carry a password from another selected account.
                if (dgvUsers.CurrentRow == null) return;
                txtNewUser.Text = dgvUsers.CurrentRow.Cells["Tài khoản"].Value?.ToString();
                txtNewFullName.Text = dgvUsers.CurrentRow.Cells["Họ và tên"].Value?.ToString();
                cboNewRole.Text = dgvUsers.CurrentRow.Cells["Vai trò"].Value?.ToString();
            };

            // 1. Thêm thiết bị
            btnEqAdd.Click += (s, e) =>
            {
                if (!RequirePermission(RolePermissions.CanManageEquipment(currentUserRole))) return;
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
                if (!RequirePermission(RolePermissions.CanManageEquipment(currentUserRole))) return;
                if (dgvEquipments.CurrentRow == null) return;
                if (string.IsNullOrWhiteSpace(txtEqCode.Text) || string.IsNullOrWhiteSpace(txtEqName.Text) || cboEqLab.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đủ Mã, Tên thiết bị và Phòng Lab!", "Thông báo");
                    return;
                }
                int id = Convert.ToInt32(dgvEquipments.CurrentRow.Cells["EquipmentID"].Value);
                try {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Equipments SET EquipmentCode=@c, EquipmentName=@n, Category=@cat, LabID=@l, Status=@s WHERE EquipmentID=@id",
                    new SQLiteParameter("@c", txtEqCode.Text.Trim()),
                    new SQLiteParameter("@n", txtEqName.Text.Trim()),
                    new SQLiteParameter("@cat", txtEqCategory.Text.Trim()),
                    new SQLiteParameter("@l", cboEqLab.SelectedValue),
                    new SQLiteParameter("@s", cboEqStatus.Text),
                    new SQLiteParameter("@id", id));
                LoadAllData();
                } catch (Exception ex) { MessageBox.Show("Không thể cập nhật: " + ex.Message); }
            };

            // 3. Xóa thiết bị
            btnEqDelete.Click += (s, e) =>
            {
                if (!RequirePermission(RolePermissions.CanManageEquipment(currentUserRole))) return;
                if (dgvEquipments.CurrentRow == null) return;
                int id = Convert.ToInt32(dgvEquipments.CurrentRow.Cells["EquipmentID"].Value);
                if (Convert.ToInt64(DatabaseHelper.GetDataTable("SELECT COUNT(*) FROM MaintenanceRequests WHERE EquipmentID=@id", new SQLiteParameter("@id", id)).Rows[0][0]) > 0)
                { MessageBox.Show("Không thể xóa do đã có lịch sử hoặc phiếu bảo trì. Hãy chuyển thiết bị sang thanh lý."); return; }
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                try { DatabaseHelper.ExecuteNonQuery("DELETE FROM Equipments WHERE EquipmentID=@id", new SQLiteParameter("@id", id)); LoadAllData(); }
                catch (Exception ex) { MessageBox.Show("Không thể xóa: " + ex.Message); }
            };

            // 4. Thêm & Xóa Phòng Lab
            btnLabAdd.Click += (s, e) =>
            {
                if (!RequirePermission(RolePermissions.CanManageLabs(currentUserRole))) return;
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

            btnLabUpdate.Click += (_, _) =>
            {
                if (!RequirePermission(RolePermissions.CanManageLabs(currentUserRole))) return;
                if (dgvLabs.CurrentRow == null) { MessageBox.Show("Vui lòng chọn phòng Lab cần sửa."); return; }
                try
                {
                    AdministrationService.UpdateLab(currentUserId, Convert.ToInt32(dgvLabs.CurrentRow.Cells["LabID"].Value),
                        txtLabCode.Text, txtLabName.Text, txtLabLocation.Text);
                    LoadAllData();
                }
                catch (Exception ex) { MessageBox.Show("Không thể sửa phòng Lab: " + ex.Message); }
            };

            btnLabDelete.Click += (s, e) =>
            {
                if (!RequirePermission(RolePermissions.CanManageLabs(currentUserRole))) return;
                if (dgvLabs.CurrentRow == null) return;
                int id = Convert.ToInt32(dgvLabs.CurrentRow.Cells["LabID"].Value);
                if (Convert.ToInt64(DatabaseHelper.GetDataTable("SELECT COUNT(*) FROM Equipments WHERE LabID=@id", new SQLiteParameter("@id", id)).Rows[0][0]) > 0)
                { MessageBox.Show("Không thể xóa phòng Lab đang có thiết bị."); return; }
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                try { DatabaseHelper.ExecuteNonQuery("DELETE FROM Labs WHERE LabID=@id", new SQLiteParameter("@id", id)); LoadAllData(); }
                catch (Exception ex) { MessageBox.Show("Không thể xóa: " + ex.Message); }
            };

            // 5. Gửi yêu cầu bảo trì
            btnReqSubmit.Click += (s, e) =>
            {
                if (cboReqEquipment.SelectedValue == null || string.IsNullOrWhiteSpace(txtReqIssue.Text))
                {
                    MessageBox.Show("Vui lòng chọn thiết bị và nhập mô tả sự cố!");
                    return;
                }
                try
                {
                    int? technicianId = cboReqTechnician.SelectedValue == null ? null : Convert.ToInt32(cboReqTechnician.SelectedValue);
                    MaintenanceService.CreateRequest(currentUserId, Convert.ToInt32(cboReqEquipment.SelectedValue),
                        txtReqIssue.Text, cboReqPriority.Text, technicianId);
                    txtReqIssue.Clear();
                    LoadAllData();
                    MessageBox.Show("Đã tạo phiếu và phân công kỹ thuật viên thành công!");
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Không thể tạo phiếu", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            };

            // 6. Hoàn tất bảo trì
            btnHisComplete.Click += (s, e) =>
            {
                if (!RequirePermission(RolePermissions.CanCompleteMaintenance(currentUserRole))) return;
                if (cboPendingRequests.SelectedValue == null || string.IsNullOrWhiteSpace(txtLogAction.Text))
                {
                    MessageBox.Show("Vui lòng chọn phiếu chờ và nhập kết quả sửa chữa!");
                    return;
                }
                if (!VndMoney.TryParse(txtLogCost.Text, out double cost))
                {
                    MessageBox.Show("Chi phí không hợp lệ. Chi phí phải là số nguyên không âm (VND).");
                    return;
                }
                try
                {
                    if (!radAfterGood.Checked && !radAfterBroken.Checked) { MessageBox.Show("Vui lòng chọn tình trạng thiết bị sau bảo trì."); return; }
                    if (MessageBox.Show("Xác nhận hoàn tất bảo trì?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                    MaintenanceService.CompleteRequest(currentUserId, Convert.ToInt32(cboPendingRequests.SelectedValue), txtLogAction.Text, cost, radAfterGood.Checked ? "Tốt" : "Hỏng");
                    radAfterGood.Checked = radAfterBroken.Checked = false;
                    txtLogAction.Clear(); txtLogCost.Text = "0";
                    LoadAllData();
                    MessageBox.Show("Đã ghi nhận kết quả và hoàn tất phiếu bảo trì!");
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Không thể hoàn tất phiếu", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            };

            // 7. Thêm, sửa và xóa người dùng
            btnUserAdd.Click += (_, _) =>
            {
                if (!RequirePermission(RolePermissions.CanManageUsers(currentUserRole))) return;
                try
                {
                    AdministrationService.CreateUser(currentUserId, txtNewUser.Text, txtNewPass.Text, txtNewFullName.Text, cboNewRole.Text);
                    txtNewPass.Clear();
                    LoadAllData();
                }
                catch (Exception ex) { MessageBox.Show("Lỗi tạo tài khoản: " + ex.Message); }
            };

            btnUserUpdate.Click += (_, _) =>
            {
                if (!RequirePermission(RolePermissions.CanManageUsers(currentUserRole))) return;
                if (dgvUsers.CurrentRow == null) { MessageBox.Show("Vui lòng chọn người dùng cần sửa."); return; }
                try
                {
                    int id = Convert.ToInt32(dgvUsers.CurrentRow.Cells["UserID"].Value);
                    AdministrationService.UpdateUser(currentUserId, id, txtNewUser.Text, txtNewPass.Text, txtNewFullName.Text, cboNewRole.Text);
                    if (id == currentUserId)
                    {
                        currentUserName = txtNewFullName.Text.Trim();
                        lblUserInfo.Text = $"👤 {currentUserName}\r\n🔑 Vai trò: {currentUserRole}";
                    }
                    txtNewPass.Clear();
                    LoadAllData();
                }
                catch (Exception ex) { MessageBox.Show("Không thể sửa người dùng: " + ex.Message); }
            };

            btnUserDelete.Click += (s, e) =>
            {
                if (!RequirePermission(RolePermissions.CanManageUsers(currentUserRole))) return;
                if (dgvUsers.CurrentRow == null) return;
                int id = Convert.ToInt32(dgvUsers.CurrentRow.Cells["UserID"].Value);
                if (id == 1 || id == currentUserId) { MessageBox.Show("Không thể xóa Admin mặc định hoặc tài khoản đang đăng nhập!"); return; }
                var assigned = DatabaseHelper.GetDataTable("SELECT COUNT(*) FROM MaintenanceRequests WHERE AssignedTechnicianID=@id AND Status='Chờ xử lý'", new SQLiteParameter("@id", id));
                if (Convert.ToInt64(assigned.Rows[0][0]) > 0) { MessageBox.Show("Không thể xóa kỹ thuật viên đang được phân công phiếu chưa hoàn thành!"); return; }
                try { DatabaseHelper.ExecuteNonQuery("DELETE FROM Users WHERE UserID=@id", new SQLiteParameter("@id", id)); }
                catch (Exception ex) { MessageBox.Show("Không thể xóa người dùng có dữ liệu liên quan: " + ex.Message); return; }
                LoadAllData();
            };
        }

        private void LoadAllData()
        {
            var dtLabs = DatabaseHelper.GetDataTable("SELECT LabID, LabCode AS 'Mã Phòng', LabName AS 'Tên Phòng Lab', Location AS 'Vị trí', Status AS 'Trạng thái' FROM Labs");
            if (RolePermissions.CanManageLabs(currentUserRole))
            {
                dgvLabs.DataSource = dtLabs;
                cboEqLab.DisplayMember = "Tên Phòng Lab";
                cboEqLab.ValueMember = "LabID";
                cboEqLab.DataSource = dtLabs.Copy();
            }

            if (RolePermissions.CanAssignTechnician(currentUserRole))
            {
                var technicians = DatabaseHelper.GetDataTable("SELECT UserID, FullName || ' (' || Username || ')' AS DisplayName FROM Users WHERE Role='Technician' ORDER BY FullName, UserID");
                cboReqTechnician.DisplayMember = "DisplayName";
                cboReqTechnician.ValueMember = "UserID";
                cboReqTechnician.DataSource = technicians;
                cboReqTechnician.SelectedIndex = -1;
            }
            btnReqSubmit.Enabled = RolePermissions.CanCreateRequest(currentUserRole);

            var dtEq = DatabaseHelper.GetDataTable(@"
                SELECT e.EquipmentID, e.LabID, e.EquipmentCode AS 'Mã TB', e.EquipmentName AS 'Tên Thiết Bị',
                       e.Category AS 'Danh mục', l.LabName AS 'Phòng Lab', e.Status AS 'Trạng thái'
                FROM Equipments e LEFT JOIN Labs l ON e.LabID = l.LabID");
            dgvEquipments.DataSource = dtEq;
            if (dgvEquipments.Columns["LabID"] is DataGridViewColumn labColumn) labColumn.Visible = false;
            cboReqEquipment.DisplayMember = "Tên Thiết Bị";
            cboReqEquipment.ValueMember = "EquipmentID";
            cboReqEquipment.DataSource = DatabaseHelper.GetDataTable(@"
                SELECT e.EquipmentID, e.EquipmentName AS 'Tên Thiết Bị'
                FROM Equipments e
                WHERE e.Status <> 'Thanh lý'
                  AND NOT EXISTS (SELECT 1 FROM MaintenanceRequests r
                                  WHERE r.EquipmentID=e.EquipmentID AND r.Status <> 'Hoàn thành')
                ORDER BY e.EquipmentName, e.EquipmentID");
            cboReqEquipment.SelectedIndex = -1;

            dgvRequests.DataSource = DatabaseHelper.GetDataTable(@"
                SELECT r.RequestID, e.EquipmentCode || ' - ' || e.EquipmentName AS 'Thiết bị',
                       u.FullName AS 'Người báo', COALESCE(t.FullName, 'Chưa phân công') AS 'Kỹ thuật viên phụ trách', r.RequestDate AS 'Ngày tạo',
                       r.IssueDescription AS 'Mô tả lỗi', r.Priority AS 'Ưu tiên', r.Status AS 'Trạng thái'
                FROM MaintenanceRequests r
                JOIN Equipments e ON r.EquipmentID = e.EquipmentID
                JOIN Users u ON r.RequestedBy = u.UserID
                LEFT JOIN Users t ON r.AssignedTechnicianID = t.UserID ORDER BY r.RequestID DESC");

            cboPendingRequests.DisplayMember = "DisplayText";
            cboPendingRequests.ValueMember = "RequestID";
            cboPendingRequests.DataSource = MaintenanceService.GetPendingRequests(currentUserId);

            dgvLogs.DataSource = DatabaseHelper.GetDataTable(@"
                SELECT l.LogID AS 'Mã LS', e.EquipmentName AS 'Thiết bị', u.FullName AS 'Kỹ thuật viên',
                       l.CompletedDate AS 'Ngày hoàn thành', COALESCE(l.EquipmentStatusAfter, 'Chưa ghi nhận') AS 'Tình trạng thiết bị sau bảo trì', l.ActionTaken AS 'Kết quả xử lý', l.Cost AS 'Chi phí (VNĐ)'
                FROM MaintenanceLogs l
                JOIN MaintenanceRequests r ON l.RequestID = r.RequestID
                JOIN Equipments e ON r.EquipmentID = e.EquipmentID
                JOIN Users u ON l.TechnicianID = u.UserID ORDER BY l.LogID DESC");

            if (dgvLogs.Columns["Chi phí (VNĐ)"] is DataGridViewColumn costColumn)
            {
                costColumn.DefaultCellStyle.Format = "N0";
                costColumn.DefaultCellStyle.FormatProvider = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
            }
            dgvStatsStatus.DataSource = DatabaseHelper.GetDataTable("SELECT Status AS 'Trạng thái', COUNT(*) AS 'Số lượng' FROM Equipments GROUP BY Status");
            dgvStatsLab.DataSource = DatabaseHelper.GetDataTable("SELECT l.LabName AS 'Phòng Lab', COUNT(e.EquipmentID) AS 'Số thiết bị' FROM Labs l LEFT JOIN Equipments e ON l.LabID = e.LabID GROUP BY l.LabName");

            long totalEq = Convert.ToInt64(DatabaseHelper.GetDataTable("SELECT COUNT(*) FROM Equipments").Rows[0][0]);
            long brokenEq = Convert.ToInt64(DatabaseHelper.GetDataTable("SELECT COUNT(*) FROM Equipments WHERE Status IN ('Hỏng', 'Đang bảo trì')").Rows[0][0]);
            double totalCost = Convert.ToDouble(DatabaseHelper.GetDataTable("SELECT IFNULL(SUM(Cost), 0) FROM MaintenanceLogs").Rows[0][0]);

            lblCardTotalVal.Text = $"{totalEq} Thiết bị";
            lblCardBrokenVal.Text = $"{brokenEq} Thiết bị";
            lblCardCostVal.Text = VndMoney.Format(totalCost) + " VND";

            if (RolePermissions.CanManageUsers(currentUserRole))
                dgvUsers.DataSource = DatabaseHelper.GetDataTable("SELECT UserID, Username AS 'Tài khoản', FullName AS 'Họ và tên', Role AS 'Vai trò' FROM Users");
        }
    }
}
