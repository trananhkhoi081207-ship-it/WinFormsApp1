namespace LabMaintenanceApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            if (disposing)
            {
                tabUsers?.Dispose();
                tabLabs?.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.lblUserInfo = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnNavUsers = new System.Windows.Forms.Button();
            this.btnNavStats = new System.Windows.Forms.Button();
            this.btnNavHistory = new System.Windows.Forms.Button();
            this.btnNavRequest = new System.Windows.Forms.Button();
            this.btnNavLabs = new System.Windows.Forms.Button();
            this.btnNavEquipments = new System.Windows.Forms.Button();
            this.lblAppLogo = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblRoleBadge = new System.Windows.Forms.Label();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabEquipments = new System.Windows.Forms.TabPage();
            this.dgvEquipments = new System.Windows.Forms.DataGridView();
            this.pnlEqCard = new System.Windows.Forms.Panel();
            this.btnEqDelete = new System.Windows.Forms.Button();
            this.btnEqUpdate = new System.Windows.Forms.Button();
            this.btnEqAdd = new System.Windows.Forms.Button();
            this.cboEqStatus = new System.Windows.Forms.ComboBox();
            this.lblEqStatus = new System.Windows.Forms.Label();
            this.cboEqLab = new System.Windows.Forms.ComboBox();
            this.lblEqLab = new System.Windows.Forms.Label();
            this.txtEqCategory = new System.Windows.Forms.TextBox();
            this.lblEqCategory = new System.Windows.Forms.Label();
            this.txtEqName = new System.Windows.Forms.TextBox();
            this.lblEqName = new System.Windows.Forms.Label();
            this.txtEqCode = new System.Windows.Forms.TextBox();
            this.lblEqCode = new System.Windows.Forms.Label();
            this.tabLabs = new System.Windows.Forms.TabPage();
            this.dgvLabs = new System.Windows.Forms.DataGridView();
            this.pnlLabCard = new System.Windows.Forms.Panel();
            this.btnLabDelete = new System.Windows.Forms.Button();
            this.btnLabAdd = new System.Windows.Forms.Button();
            this.txtLabLocation = new System.Windows.Forms.TextBox();
            this.lblLabLoc = new System.Windows.Forms.Label();
            this.txtLabName = new System.Windows.Forms.TextBox();
            this.lblLabName = new System.Windows.Forms.Label();
            this.txtLabCode = new System.Windows.Forms.TextBox();
            this.lblLabCode = new System.Windows.Forms.Label();
            this.tabRequests = new System.Windows.Forms.TabPage();
            this.dgvRequests = new System.Windows.Forms.DataGridView();
            this.pnlReqCard = new System.Windows.Forms.Panel();
            this.btnReqSubmit = new System.Windows.Forms.Button();
            this.txtReqIssue = new System.Windows.Forms.TextBox();
            this.lblReqIssue = new System.Windows.Forms.Label();
            this.cboReqPriority = new System.Windows.Forms.ComboBox();
            this.lblReqPriority = new System.Windows.Forms.Label();
            this.cboReqEquipment = new System.Windows.Forms.ComboBox();
            this.lblReqEq = new System.Windows.Forms.Label();
            this.lblReqTechnician = new System.Windows.Forms.Label();
            this.cboReqTechnician = new System.Windows.Forms.ComboBox();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.dgvLogs = new System.Windows.Forms.DataGridView();
            this.pnlHisCard = new System.Windows.Forms.Panel();
            this.grpAfterCondition = new System.Windows.Forms.GroupBox();
            this.btnHisComplete = new System.Windows.Forms.Button();
            this.txtLogAction = new System.Windows.Forms.TextBox();
            this.lblHisAction = new System.Windows.Forms.Label();
            this.txtLogCost = new System.Windows.Forms.TextBox();
            this.lblHisCost = new System.Windows.Forms.Label();
            this.cboPendingRequests = new System.Windows.Forms.ComboBox();
            this.lblHisPending = new System.Windows.Forms.Label();
            this.tabStats = new System.Windows.Forms.TabPage();
            this.dgvStatsLab = new System.Windows.Forms.DataGridView();
            this.dgvStatsStatus = new System.Windows.Forms.DataGridView();
            this.pnlCardCost = new System.Windows.Forms.Panel();
            this.lblCardCostVal = new System.Windows.Forms.Label();
            this.lblCardCostTitle = new System.Windows.Forms.Label();
            this.pnlCardBroken = new System.Windows.Forms.Panel();
            this.lblCardBrokenVal = new System.Windows.Forms.Label();
            this.lblCardBrokenTitle = new System.Windows.Forms.Label();
            this.pnlCardTotalEq = new System.Windows.Forms.Panel();
            this.lblCardTotalVal = new System.Windows.Forms.Label();
            this.lblCardTotalTitle = new System.Windows.Forms.Label();
            this.tabUsers = new System.Windows.Forms.TabPage();
            this.dgvUsers = new System.Windows.Forms.DataGridView();
            this.pnlUserCard = new System.Windows.Forms.Panel();
            this.btnUserDelete = new System.Windows.Forms.Button();
            this.btnUserAdd = new System.Windows.Forms.Button();
            this.cboNewRole = new System.Windows.Forms.ComboBox();
            this.lblUserRole = new System.Windows.Forms.Label();
            this.txtNewFullName = new System.Windows.Forms.TextBox();
            this.lblUserFullName = new System.Windows.Forms.Label();
            this.txtNewPass = new System.Windows.Forms.TextBox();
            this.lblUserPass = new System.Windows.Forms.Label();
            this.txtNewUser = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.pnlSidebar.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabEquipments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquipments)).BeginInit();
            this.pnlEqCard.SuspendLayout();
            this.tabLabs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLabs)).BeginInit();
            this.pnlLabCard.SuspendLayout();
            this.tabRequests.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequests)).BeginInit();
            this.pnlReqCard.SuspendLayout();
            this.tabHistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).BeginInit();
            this.pnlHisCard.SuspendLayout();
            this.tabStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatsLab)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatsStatus)).BeginInit();
            this.pnlCardCost.SuspendLayout();
            this.pnlCardBroken.SuspendLayout();
            this.pnlCardTotalEq.SuspendLayout();
            this.tabUsers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).BeginInit();
            this.pnlUserCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.pnlSidebar.Controls.Add(this.lblUserInfo);
            this.pnlSidebar.Controls.Add(this.btnLogout);
            this.pnlSidebar.Controls.Add(this.btnNavUsers);
            this.pnlSidebar.Controls.Add(this.btnNavStats);
            this.pnlSidebar.Controls.Add(this.btnNavHistory);
            this.pnlSidebar.Controls.Add(this.btnNavRequest);
            this.pnlSidebar.Controls.Add(this.btnNavLabs);
            this.pnlSidebar.Controls.Add(this.btnNavEquipments);
            this.pnlSidebar.Controls.Add(this.lblAppLogo);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(230, 680);
            // 
            // lblAppLogo
            // 
            this.lblAppLogo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblAppLogo.ForeColor = System.Drawing.Color.FromArgb(56, 189, 248);
            this.lblAppLogo.Location = new System.Drawing.Point(15, 20);
            this.lblAppLogo.Name = "lblAppLogo";
            this.lblAppLogo.Size = new System.Drawing.Size(200, 45);
            this.lblAppLogo.Text = "🧪 LAB CARE PRO";
            this.lblAppLogo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNavEquipments
            // 
            this.btnNavEquipments.BackColor = System.Drawing.Color.FromArgb(59, 130, 246);
            this.btnNavEquipments.FlatAppearance.BorderSize = 0;
            this.btnNavEquipments.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavEquipments.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnNavEquipments.ForeColor = System.Drawing.Color.White;
            this.btnNavEquipments.Location = new System.Drawing.Point(15, 85);
            this.btnNavEquipments.Name = "btnNavEquipments";
            this.btnNavEquipments.Size = new System.Drawing.Size(200, 44);
            this.btnNavEquipments.Text = "  📦  Thiết bị phòng Lab";
            this.btnNavEquipments.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNavLabs
            // 
            this.btnNavLabs.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnNavLabs.FlatAppearance.BorderSize = 0;
            this.btnNavLabs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavLabs.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnNavLabs.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnNavLabs.Location = new System.Drawing.Point(15, 140);
            this.btnNavLabs.Name = "btnNavLabs";
            this.btnNavLabs.Size = new System.Drawing.Size(200, 44);
            this.btnNavLabs.Text = "  🏢  Phòng thí nghiệm";
            this.btnNavLabs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNavRequest
            // 
            this.btnNavRequest.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnNavRequest.FlatAppearance.BorderSize = 0;
            this.btnNavRequest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavRequest.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnNavRequest.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnNavRequest.Location = new System.Drawing.Point(15, 195);
            this.btnNavRequest.Name = "btnNavRequest";
            this.btnNavRequest.Size = new System.Drawing.Size(200, 44);
            this.btnNavRequest.Text = "  🛠  Tạo phiếu bảo trì";
            this.btnNavRequest.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNavHistory
            // 
            this.btnNavHistory.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnNavHistory.FlatAppearance.BorderSize = 0;
            this.btnNavHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavHistory.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnNavHistory.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnNavHistory.Location = new System.Drawing.Point(15, 250);
            this.btnNavHistory.Name = "btnNavHistory";
            this.btnNavHistory.Size = new System.Drawing.Size(200, 44);
            this.btnNavHistory.Text = "  📋  Xử lý và lịch sử";
            this.btnNavHistory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNavStats
            // 
            this.btnNavStats.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnNavStats.FlatAppearance.BorderSize = 0;
            this.btnNavStats.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavStats.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnNavStats.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnNavStats.Location = new System.Drawing.Point(15, 305);
            this.btnNavStats.Name = "btnNavStats";
            this.btnNavStats.Size = new System.Drawing.Size(200, 44);
            this.btnNavStats.Text = "  📊  Thống kê báo cáo";
            this.btnNavStats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnNavUsers
            // 
            this.btnNavUsers.BackColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.btnNavUsers.FlatAppearance.BorderSize = 0;
            this.btnNavUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavUsers.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnNavUsers.ForeColor = System.Drawing.Color.FromArgb(203, 213, 225);
            this.btnNavUsers.Location = new System.Drawing.Point(15, 360);
            this.btnNavUsers.Name = "btnNavUsers";
            this.btnNavUsers.Size = new System.Drawing.Size(200, 44);
            this.btnNavUsers.Text = "  👤  Quản lý tài khoản";
            this.btnNavUsers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUserInfo
            // 
            this.lblUserInfo.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblUserInfo.ForeColor = System.Drawing.Color.FromArgb(148, 163, 184);
            this.lblUserInfo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.lblUserInfo.Location = new System.Drawing.Point(15, 565);
            this.lblUserInfo.Name = "lblUserInfo";
            this.lblUserInfo.Size = new System.Drawing.Size(200, 50);
            this.lblUserInfo.Text = "Đang đăng nhập:\r\nAdmin";
            // 
            // btnLogout
            //
            this.btnLogout.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnLogout.BackColor = System.Drawing.Color.FromArgb(51, 65, 85);
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(15, 625);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(200, 36);
            this.btnLogout.Text = "Đăng xuất";
            //
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.lblRoleBadge);
            this.pnlHeader.Controls.Add(this.lblPageTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(230, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(920, 65);
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.AutoSize = true;
            this.lblPageTitle.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
            this.lblPageTitle.Location = new System.Drawing.Point(24, 18);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(290, 28);
            this.lblPageTitle.Text = "Quản Lý Thiết Bị Phòng Lab";
            // 
            // lblRoleBadge
            // 
            this.lblRoleBadge.BackColor = System.Drawing.Color.FromArgb(224, 242, 254);
            this.lblRoleBadge.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblRoleBadge.ForeColor = System.Drawing.Color.FromArgb(2, 132, 199);
            this.lblRoleBadge.Location = new System.Drawing.Point(750, 18);
            this.lblRoleBadge.Name = "lblRoleBadge";
            this.lblRoleBadge.Size = new System.Drawing.Size(145, 30);
            this.lblRoleBadge.Text = "Quyền: Admin";
            this.lblRoleBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.tabEquipments);
            this.tabMain.Controls.Add(this.tabLabs);
            this.tabMain.Controls.Add(this.tabRequests);
            this.tabMain.Controls.Add(this.tabHistory);
            this.tabMain.Controls.Add(this.tabStats);
            this.tabMain.Controls.Add(this.tabUsers);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.tabMain.Location = new System.Drawing.Point(230, 65);
            this.tabMain.Name = "tabMain";
            this.tabMain.SelectedIndex = 0;
            this.tabMain.Size = new System.Drawing.Size(920, 615);
            // 
            // tabEquipments (Tab 1: Quản lý thiết bị)
            // 
            this.tabEquipments.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.tabEquipments.Controls.Add(this.dgvEquipments);
            this.tabEquipments.Controls.Add(this.pnlEqCard);
            this.tabEquipments.Location = new System.Drawing.Point(4, 26);
            this.tabEquipments.Name = "tabEquipments";
            this.tabEquipments.Padding = new System.Windows.Forms.Padding(15);
            this.tabEquipments.Size = new System.Drawing.Size(912, 585);
            this.tabEquipments.Text = "1. Thiết bị";
            // 
            // pnlEqCard
            // 
            this.pnlEqCard.BackColor = System.Drawing.Color.White;
            this.pnlEqCard.Controls.Add(this.btnEqDelete);
            this.pnlEqCard.Controls.Add(this.btnEqUpdate);
            this.pnlEqCard.Controls.Add(this.btnEqAdd);
            this.pnlEqCard.Controls.Add(this.cboEqStatus);
            this.pnlEqCard.Controls.Add(this.lblEqStatus);
            this.pnlEqCard.Controls.Add(this.cboEqLab);
            this.pnlEqCard.Controls.Add(this.lblEqLab);
            this.pnlEqCard.Controls.Add(this.txtEqCategory);
            this.pnlEqCard.Controls.Add(this.lblEqCategory);
            this.pnlEqCard.Controls.Add(this.txtEqName);
            this.pnlEqCard.Controls.Add(this.lblEqName);
            this.pnlEqCard.Controls.Add(this.txtEqCode);
            this.pnlEqCard.Controls.Add(this.lblEqCode);
            this.pnlEqCard.Location = new System.Drawing.Point(18, 15);
            this.pnlEqCard.Name = "pnlEqCard";
            this.pnlEqCard.Size = new System.Drawing.Size(875, 115);
            // 
            // Các điều khiển trong pnlEqCard
            // 
            this.lblEqCode.AutoSize = true;
            this.lblEqCode.Location = new System.Drawing.Point(20, 20);
            this.lblEqCode.Text = "Mã thiết bị:";
            this.txtEqCode.Location = new System.Drawing.Point(105, 17);
            this.txtEqCode.Size = new System.Drawing.Size(130, 25);

            this.lblEqName.AutoSize = true;
            this.lblEqName.Location = new System.Drawing.Point(260, 20);
            this.lblEqName.Text = "Tên thiết bị:";
            this.txtEqName.Location = new System.Drawing.Point(345, 17);
            this.txtEqName.Size = new System.Drawing.Size(230, 25);

            this.lblEqCategory.AutoSize = true;
            this.lblEqCategory.Location = new System.Drawing.Point(600, 20);
            this.lblEqCategory.Text = "Danh mục:";
            this.txtEqCategory.Location = new System.Drawing.Point(680, 17);
            this.txtEqCategory.Size = new System.Drawing.Size(170, 25);

            this.lblEqLab.AutoSize = true;
            this.lblEqLab.Location = new System.Drawing.Point(20, 68);
            this.lblEqLab.Text = "Phòng Lab:";
            this.cboEqLab.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEqLab.Location = new System.Drawing.Point(105, 65);
            this.cboEqLab.Size = new System.Drawing.Size(210, 25);

            this.lblEqStatus.AutoSize = true;
            this.lblEqStatus.Location = new System.Drawing.Point(335, 68);
            this.lblEqStatus.Text = "Tình trạng:";
            this.cboEqStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEqStatus.Items.AddRange(new object[] { "Tốt", "Hỏng", "Đang bảo trì", "Thanh lý" });
            this.cboEqStatus.Location = new System.Drawing.Point(415, 65);
            this.cboEqStatus.Size = new System.Drawing.Size(130, 25);

            this.btnEqAdd.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.btnEqAdd.FlatAppearance.BorderSize = 0;
            this.btnEqAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEqAdd.ForeColor = System.Drawing.Color.White;
            this.btnEqAdd.Location = new System.Drawing.Point(565, 60);
            this.btnEqAdd.Size = new System.Drawing.Size(95, 34);
            this.btnEqAdd.Text = "+ Thêm";

            this.btnEqUpdate.BackColor = System.Drawing.Color.FromArgb(59, 130, 246);
            this.btnEqUpdate.FlatAppearance.BorderSize = 0;
            this.btnEqUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEqUpdate.ForeColor = System.Drawing.Color.White;
            this.btnEqUpdate.Location = new System.Drawing.Point(670, 60);
            this.btnEqUpdate.Size = new System.Drawing.Size(90, 34);
            this.btnEqUpdate.Text = "✎ Sửa";

            this.btnEqDelete.BackColor = System.Drawing.Color.FromArgb(239, 68, 68);
            this.btnEqDelete.FlatAppearance.BorderSize = 0;
            this.btnEqDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEqDelete.ForeColor = System.Drawing.Color.White;
            this.btnEqDelete.Location = new System.Drawing.Point(770, 60);
            this.btnEqDelete.Size = new System.Drawing.Size(85, 34);
            this.btnEqDelete.Text = "🗑 Xóa";

            this.dgvEquipments.Location = new System.Drawing.Point(18, 145);
            this.dgvEquipments.Name = "dgvEquipments";
            this.dgvEquipments.Size = new System.Drawing.Size(875, 420);
            // 
            // tabLabs (Tab 2: Phòng thí nghiệm)
            // 
            this.tabLabs.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.tabLabs.Controls.Add(this.dgvLabs);
            this.tabLabs.Controls.Add(this.pnlLabCard);
            this.tabLabs.Location = new System.Drawing.Point(4, 26);
            this.tabLabs.Name = "tabLabs";
            this.tabLabs.Size = new System.Drawing.Size(912, 585);
            this.tabLabs.Text = "2. Phòng Lab";
            // 
            // pnlLabCard
            // 
            this.pnlLabCard.BackColor = System.Drawing.Color.White;
            this.pnlLabCard.Controls.Add(this.btnLabDelete);
            this.pnlLabCard.Controls.Add(this.btnLabAdd);
            this.pnlLabCard.Controls.Add(this.txtLabLocation);
            this.pnlLabCard.Controls.Add(this.lblLabLoc);
            this.pnlLabCard.Controls.Add(this.txtLabName);
            this.pnlLabCard.Controls.Add(this.lblLabName);
            this.pnlLabCard.Controls.Add(this.txtLabCode);
            this.pnlLabCard.Controls.Add(this.lblLabCode);
            this.pnlLabCard.Location = new System.Drawing.Point(18, 15);
            this.pnlLabCard.Size = new System.Drawing.Size(875, 80);

            this.lblLabCode.AutoSize = true;
            this.lblLabCode.Location = new System.Drawing.Point(20, 28);
            this.lblLabCode.Text = "Mã Phòng:";
            this.txtLabCode.Location = new System.Drawing.Point(95, 25);
            this.txtLabCode.Size = new System.Drawing.Size(100, 25);

            this.lblLabName.AutoSize = true;
            this.lblLabName.Location = new System.Drawing.Point(210, 28);
            this.lblLabName.Text = "Tên Phòng:";
            this.txtLabName.Location = new System.Drawing.Point(290, 25);
            this.txtLabName.Size = new System.Drawing.Size(185, 25);

            this.lblLabLoc.AutoSize = true;
            this.lblLabLoc.Location = new System.Drawing.Point(490, 28);
            this.lblLabLoc.Text = "Vị trí:";
            this.txtLabLocation.Location = new System.Drawing.Point(535, 25);
            this.txtLabLocation.Size = new System.Drawing.Size(120, 25);

            this.btnLabAdd.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.btnLabAdd.FlatAppearance.BorderSize = 0;
            this.btnLabAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLabAdd.ForeColor = System.Drawing.Color.White;
            this.btnLabAdd.Location = new System.Drawing.Point(670, 21);
            this.btnLabAdd.Size = new System.Drawing.Size(95, 34);
            this.btnLabAdd.Text = "+ Thêm";

            this.btnLabDelete.BackColor = System.Drawing.Color.FromArgb(239, 68, 68);
            this.btnLabDelete.FlatAppearance.BorderSize = 0;
            this.btnLabDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLabDelete.ForeColor = System.Drawing.Color.White;
            this.btnLabDelete.Location = new System.Drawing.Point(775, 21);
            this.btnLabDelete.Size = new System.Drawing.Size(80, 34);
            this.btnLabDelete.Text = "🗑 Xóa";

            this.dgvLabs.Location = new System.Drawing.Point(18, 110);
            this.dgvLabs.Size = new System.Drawing.Size(875, 455);
            // 
            // tabRequests (Tab 3: Tạo yêu cầu bảo trì)
            // 
            this.tabRequests.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.tabRequests.Controls.Add(this.dgvRequests);
            this.tabRequests.Controls.Add(this.pnlReqCard);
            this.tabRequests.Location = new System.Drawing.Point(4, 26);
            this.tabRequests.Name = "tabRequests";
            this.tabRequests.Size = new System.Drawing.Size(912, 585);
            this.tabRequests.Text = "3. Yêu cầu bảo trì";
            // 
            // pnlReqCard
            // 
            this.pnlReqCard.BackColor = System.Drawing.Color.White;
            this.pnlReqCard.Controls.Add(this.btnReqSubmit);
            this.pnlReqCard.Controls.Add(this.txtReqIssue);
            this.pnlReqCard.Controls.Add(this.lblReqIssue);
            this.pnlReqCard.Controls.Add(this.cboReqPriority);
            this.pnlReqCard.Controls.Add(this.lblReqPriority);
            this.pnlReqCard.Controls.Add(this.cboReqEquipment);
            this.pnlReqCard.Controls.Add(this.lblReqEq);
            this.pnlReqCard.Controls.Add(this.lblReqTechnician);
            this.pnlReqCard.Controls.Add(this.cboReqTechnician);
            this.pnlReqCard.Location = new System.Drawing.Point(18, 15);
            this.pnlReqCard.Size = new System.Drawing.Size(875, 155);

            this.lblReqEq.AutoSize = true;
            this.lblReqEq.Location = new System.Drawing.Point(20, 20);
            this.lblReqEq.Text = "Chọn thiết bị hỏng:";
            this.cboReqEquipment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReqEquipment.Location = new System.Drawing.Point(155, 17);
            this.cboReqEquipment.Size = new System.Drawing.Size(340, 25);

            this.lblReqPriority.AutoSize = true;
            this.lblReqPriority.Location = new System.Drawing.Point(525, 20);
            this.lblReqPriority.Text = "Mức ưu tiên:";
            this.cboReqPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReqPriority.Items.AddRange(new object[] { "Thấp", "Trung bình", "Khẩn cấp" });
            this.cboReqPriority.Location = new System.Drawing.Point(620, 17);
            this.cboReqPriority.Size = new System.Drawing.Size(230, 25);

            this.lblReqIssue.AutoSize = true;
            this.lblReqIssue.Location = new System.Drawing.Point(20, 65);
            this.lblReqIssue.Text = "Mô tả tình trạng lỗi:";
            this.txtReqIssue.Location = new System.Drawing.Point(155, 62);
            this.txtReqIssue.Size = new System.Drawing.Size(510, 25);

            this.btnReqSubmit.BackColor = System.Drawing.Color.FromArgb(245, 158, 11);
            this.btnReqSubmit.FlatAppearance.BorderSize = 0;
            this.btnReqSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReqSubmit.ForeColor = System.Drawing.Color.White;
            this.btnReqSubmit.Location = new System.Drawing.Point(685, 57);
            this.btnReqSubmit.Size = new System.Drawing.Size(165, 35);
            this.btnReqSubmit.Text = "🚀 Gửi Phiếu Bảo Trì";

            this.lblReqTechnician.AutoSize = true;
            this.lblReqTechnician.Location = new System.Drawing.Point(20, 112);
            this.lblReqTechnician.Text = "Phân công kỹ thuật viên:";
            this.cboReqTechnician.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboReqTechnician.Location = new System.Drawing.Point(200, 109);
            this.cboReqTechnician.Size = new System.Drawing.Size(465, 25);
            this.cboReqTechnician.Name = "cboReqTechnician";

            this.dgvRequests.Location = new System.Drawing.Point(18, 185);
            this.dgvRequests.Size = new System.Drawing.Size(875, 380);
            // 
            // tabHistory (Tab 4: Xử lý & Lịch sử bảo trì)
            // 
            this.tabHistory.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.tabHistory.Controls.Add(this.dgvLogs);
            this.tabHistory.Controls.Add(this.pnlHisCard);
            this.tabHistory.Location = new System.Drawing.Point(4, 26);
            this.tabHistory.Name = "tabHistory";
            this.tabHistory.Size = new System.Drawing.Size(912, 585);
            this.tabHistory.Text = "4. Lịch sử bảo trì";
            // 
            // pnlHisCard
            // 
            this.pnlHisCard.BackColor = System.Drawing.Color.White;
            this.pnlHisCard.Controls.Add(this.btnHisComplete);
            this.pnlHisCard.Controls.Add(this.txtLogAction);
            this.pnlHisCard.Controls.Add(this.lblHisAction);
            this.pnlHisCard.Controls.Add(this.txtLogCost);
            this.pnlHisCard.Controls.Add(this.lblHisCost);
            this.pnlHisCard.Controls.Add(this.cboPendingRequests);
            this.pnlHisCard.Controls.Add(this.lblHisPending);
            this.pnlHisCard.Location = new System.Drawing.Point(18, 15);
            this.pnlHisCard.Size = new System.Drawing.Size(875, 170);
            // Tạo toàn bộ bố cục trước ResumeLayout để WinForms scale cùng các ô có sẵn.
            this.grpAfterCondition.Text = "Tình trạng thiết bị sau khi bảo trì";
            this.grpAfterCondition.Location = new System.Drawing.Point(20, 103);
            this.grpAfterCondition.Size = new System.Drawing.Size(830, 55);
            this.grpAfterCondition.TabIndex = 7;
            this.grpAfterCondition.Controls.Add(this.radAfterGood);
            this.grpAfterCondition.Controls.Add(this.radAfterBroken);
            this.pnlHisCard.Controls.Add(this.grpAfterCondition);

            this.lblHisPending.AutoSize = true;
            this.lblHisPending.Location = new System.Drawing.Point(20, 20);
            this.lblHisPending.Text = "Phiếu chờ xử lý:";
            this.cboPendingRequests.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPendingRequests.Location = new System.Drawing.Point(145, 17);
            this.cboPendingRequests.Size = new System.Drawing.Size(430, 25);

            this.lblHisCost.AutoSize = true;
            this.lblHisCost.Location = new System.Drawing.Point(595, 20);
            this.lblHisCost.Text = "Chi phí (VNĐ):";
            this.txtLogCost.Location = new System.Drawing.Point(700, 17);
            this.txtLogCost.Size = new System.Drawing.Size(150, 25);
            this.txtLogCost.Text = "0";

            this.lblHisAction.AutoSize = true;
            this.lblHisAction.Location = new System.Drawing.Point(20, 65);
            this.lblHisAction.Text = "Kết quả sửa chữa:";
            this.txtLogAction.Location = new System.Drawing.Point(145, 62);
            this.txtLogAction.Size = new System.Drawing.Size(520, 25);

            this.btnHisComplete.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.btnHisComplete.FlatAppearance.BorderSize = 0;
            this.btnHisComplete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHisComplete.ForeColor = System.Drawing.Color.White;
            this.btnHisComplete.Location = new System.Drawing.Point(685, 57);
            this.btnHisComplete.Size = new System.Drawing.Size(165, 35);
            this.btnHisComplete.Text = "✔ Hoàn Tất Bảo Trì";

            this.dgvLogs.Location = new System.Drawing.Point(18, 195);
            this.dgvLogs.Size = new System.Drawing.Size(875, 370);
            // 
            // tabStats (Tab 5: Thống kê)
            // 
            this.tabStats.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.tabStats.Controls.Add(this.dgvStatsLab);
            this.tabStats.Controls.Add(this.dgvStatsStatus);
            this.tabStats.Controls.Add(this.pnlCardCost);
            this.tabStats.Controls.Add(this.pnlCardBroken);
            this.tabStats.Controls.Add(this.pnlCardTotalEq);
            this.tabStats.Location = new System.Drawing.Point(4, 26);
            this.tabStats.Name = "tabStats";
            this.tabStats.Size = new System.Drawing.Size(912, 585);
            this.tabStats.Text = "5. Thống kê";
            // 
            // 3 Thẻ thống kê Pastel mềm mại
            // 
            this.pnlCardTotalEq.BackColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.pnlCardTotalEq.Controls.Add(this.lblCardTotalVal);
            this.pnlCardTotalEq.Controls.Add(this.lblCardTotalTitle);
            this.pnlCardTotalEq.Location = new System.Drawing.Point(18, 18);
            this.pnlCardTotalEq.Size = new System.Drawing.Size(275, 105);

            this.lblCardTotalTitle.AutoSize = true;
            this.lblCardTotalTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblCardTotalTitle.ForeColor = System.Drawing.Color.FromArgb(30, 64, 175);
            this.lblCardTotalTitle.Location = new System.Drawing.Point(18, 16);
            this.lblCardTotalTitle.Text = "TỔNG THIẾT BỊ TRONG LAB";

            this.lblCardTotalVal.AutoSize = true;
            this.lblCardTotalVal.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblCardTotalVal.ForeColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.lblCardTotalVal.Location = new System.Drawing.Point(18, 45);
            this.lblCardTotalVal.Text = "0 Thiết bị";

            this.pnlCardBroken.BackColor = System.Drawing.Color.FromArgb(254, 243, 199);
            this.pnlCardBroken.Controls.Add(this.lblCardBrokenVal);
            this.pnlCardBroken.Controls.Add(this.lblCardBrokenTitle);
            this.pnlCardBroken.Location = new System.Drawing.Point(318, 18);
            this.pnlCardBroken.Size = new System.Drawing.Size(275, 105);

            this.lblCardBrokenTitle.AutoSize = true;
            this.lblCardBrokenTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblCardBrokenTitle.ForeColor = System.Drawing.Color.FromArgb(180, 83, 9);
            this.lblCardBrokenTitle.Location = new System.Drawing.Point(18, 16);
            this.lblCardBrokenTitle.Text = "ĐANG BẢO TRÌ / CHỜ SỬA";

            this.lblCardBrokenVal.AutoSize = true;
            this.lblCardBrokenVal.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblCardBrokenVal.ForeColor = System.Drawing.Color.FromArgb(217, 119, 6);
            this.lblCardBrokenVal.Location = new System.Drawing.Point(18, 45);
            this.lblCardBrokenVal.Text = "0 Thiết bị";

            this.pnlCardCost.BackColor = System.Drawing.Color.FromArgb(209, 250, 229);
            this.pnlCardCost.Controls.Add(this.lblCardCostVal);
            this.pnlCardCost.Controls.Add(this.lblCardCostTitle);
            this.pnlCardCost.Location = new System.Drawing.Point(618, 18);
            this.pnlCardCost.Size = new System.Drawing.Size(275, 105);

            this.lblCardCostTitle.AutoSize = true;
            this.lblCardCostTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblCardCostTitle.ForeColor = System.Drawing.Color.FromArgb(6, 95, 70);
            this.lblCardCostTitle.Location = new System.Drawing.Point(18, 16);
            this.lblCardCostTitle.Text = "TỔNG CHI PHÍ BẢO TRÌ";

            this.lblCardCostVal.AutoSize = true;
            this.lblCardCostVal.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblCardCostVal.ForeColor = System.Drawing.Color.FromArgb(5, 150, 105);
            this.lblCardCostVal.Location = new System.Drawing.Point(18, 50);
            this.lblCardCostVal.Text = "0 VNĐ";

            this.dgvStatsStatus.Location = new System.Drawing.Point(18, 145);
            this.dgvStatsStatus.Size = new System.Drawing.Size(425, 420);

            this.dgvStatsLab.Location = new System.Drawing.Point(468, 145);
            this.dgvStatsLab.Size = new System.Drawing.Size(425, 420);
            // 
            // tabUsers (Tab 6: Quản lý người dùng)
            // 
            this.tabUsers.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.tabUsers.Controls.Add(this.dgvUsers);
            this.tabUsers.Controls.Add(this.pnlUserCard);
            this.tabUsers.Location = new System.Drawing.Point(4, 26);
            this.tabUsers.Name = "tabUsers";
            this.tabUsers.Size = new System.Drawing.Size(912, 585);
            this.tabUsers.Text = "6. Người dùng";
            // 
            // pnlUserCard
            // 
            this.pnlUserCard.BackColor = System.Drawing.Color.White;
            this.pnlUserCard.Controls.Add(this.btnUserDelete);
            this.pnlUserCard.Controls.Add(this.btnUserAdd);
            this.pnlUserCard.Controls.Add(this.cboNewRole);
            this.pnlUserCard.Controls.Add(this.lblUserRole);
            this.pnlUserCard.Controls.Add(this.txtNewFullName);
            this.pnlUserCard.Controls.Add(this.lblUserFullName);
            this.pnlUserCard.Controls.Add(this.txtNewPass);
            this.pnlUserCard.Controls.Add(this.lblUserPass);
            this.pnlUserCard.Controls.Add(this.txtNewUser);
            this.pnlUserCard.Controls.Add(this.lblUserName);
            this.pnlUserCard.Location = new System.Drawing.Point(18, 15);
            this.pnlUserCard.Size = new System.Drawing.Size(875, 105);

            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(20, 20);
            this.lblUserName.Text = "Tên đăng nhập:";
            this.txtNewUser.Location = new System.Drawing.Point(130, 17);
            this.txtNewUser.Size = new System.Drawing.Size(150, 25);

            this.lblUserPass.AutoSize = true;
            this.lblUserPass.Location = new System.Drawing.Point(305, 20);
            this.lblUserPass.Text = "Mật khẩu:";
            this.txtNewPass.Location = new System.Drawing.Point(380, 17);
            this.txtNewPass.Size = new System.Drawing.Size(150, 25);

            this.lblUserRole.AutoSize = true;
            this.lblUserRole.Location = new System.Drawing.Point(555, 20);
            this.lblUserRole.Text = "Vai trò:";
            this.cboNewRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboNewRole.Items.AddRange(new object[] { "Admin", "LabManager", "Technician" });
            this.cboNewRole.Location = new System.Drawing.Point(615, 17);
            this.cboNewRole.Size = new System.Drawing.Size(235, 25);

            this.lblUserFullName.AutoSize = true;
            this.lblUserFullName.Location = new System.Drawing.Point(20, 62);
            this.lblUserFullName.Text = "Họ và tên:";
            this.txtNewFullName.Location = new System.Drawing.Point(130, 59);
            this.txtNewFullName.Size = new System.Drawing.Size(400, 25);

            this.btnUserAdd.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.btnUserAdd.FlatAppearance.BorderSize = 0;
            this.btnUserAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUserAdd.ForeColor = System.Drawing.Color.White;
            this.btnUserAdd.Location = new System.Drawing.Point(555, 55);
            this.btnUserAdd.Size = new System.Drawing.Size(155, 34);
            this.btnUserAdd.Text = "+ Tạo Tài Khoản";

            this.btnUserDelete.BackColor = System.Drawing.Color.FromArgb(239, 68, 68);
            this.btnUserDelete.FlatAppearance.BorderSize = 0;
            this.btnUserDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUserDelete.ForeColor = System.Drawing.Color.White;
            this.btnUserDelete.Location = new System.Drawing.Point(725, 55);
            this.btnUserDelete.Size = new System.Drawing.Size(125, 34);
            this.btnUserDelete.Text = "🗑 Xóa User";

            this.dgvUsers.Location = new System.Drawing.Point(18, 135);
            this.dgvUsers.Size = new System.Drawing.Size(875, 430);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
            this.ClientSize = new System.Drawing.Size(1150, 680);
            this.Controls.Add(this.tabMain);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlSidebar);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LabCare Pro - Quản Lý Bảo Trì Thiết Bị Phòng Thí Nghiệm";
            this.pnlSidebar.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.tabMain.ResumeLayout(false);
            this.tabEquipments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEquipments)).EndInit();
            this.pnlEqCard.ResumeLayout(false);
            this.pnlEqCard.PerformLayout();
            this.tabLabs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLabs)).EndInit();
            this.pnlLabCard.ResumeLayout(false);
            this.pnlLabCard.PerformLayout();
            this.tabRequests.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRequests)).EndInit();
            this.pnlReqCard.ResumeLayout(false);
            this.pnlReqCard.PerformLayout();
            this.tabHistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).EndInit();
            this.pnlHisCard.ResumeLayout(false);
            this.pnlHisCard.PerformLayout();
            this.tabStats.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatsLab)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatsStatus)).EndInit();
            this.pnlCardCost.ResumeLayout(false);
            this.pnlCardCost.PerformLayout();
            this.pnlCardBroken.ResumeLayout(false);
            this.pnlCardBroken.PerformLayout();
            this.pnlCardTotalEq.ResumeLayout(false);
            this.pnlCardTotalEq.PerformLayout();
            this.tabUsers.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsers)).EndInit();
            this.pnlUserCard.ResumeLayout(false);
            this.pnlUserCard.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        public System.Windows.Forms.Panel pnlSidebar;
        public System.Windows.Forms.Label lblAppLogo;
        public System.Windows.Forms.Button btnNavEquipments;
        public System.Windows.Forms.Button btnNavLabs;
        public System.Windows.Forms.Button btnNavRequest;
        public System.Windows.Forms.Button btnNavHistory;
        public System.Windows.Forms.Button btnNavStats;
        public System.Windows.Forms.Button btnNavUsers;
        public System.Windows.Forms.Label lblUserInfo;
        public System.Windows.Forms.Button btnLogout;
        public System.Windows.Forms.Panel pnlHeader;
        public System.Windows.Forms.Label lblPageTitle;
        public System.Windows.Forms.Label lblRoleBadge;
        public System.Windows.Forms.TabControl tabMain;
        public System.Windows.Forms.TabPage tabEquipments;
        public System.Windows.Forms.Panel pnlEqCard;
        public System.Windows.Forms.DataGridView dgvEquipments;
        public System.Windows.Forms.Button btnEqDelete;
        public System.Windows.Forms.Button btnEqUpdate;
        public System.Windows.Forms.Button btnEqAdd;
        public System.Windows.Forms.ComboBox cboEqStatus;
        public System.Windows.Forms.Label lblEqStatus;
        public System.Windows.Forms.ComboBox cboEqLab;
        public System.Windows.Forms.Label lblEqLab;
        public System.Windows.Forms.TextBox txtEqCategory;
        public System.Windows.Forms.Label lblEqCategory;
        public System.Windows.Forms.TextBox txtEqName;
        public System.Windows.Forms.Label lblEqName;
        public System.Windows.Forms.TextBox txtEqCode;
        public System.Windows.Forms.Label lblEqCode;
        public System.Windows.Forms.TabPage tabLabs;
        public System.Windows.Forms.DataGridView dgvLabs;
        public System.Windows.Forms.Panel pnlLabCard;
        public System.Windows.Forms.Button btnLabDelete;
        public System.Windows.Forms.Button btnLabAdd;
        public System.Windows.Forms.TextBox txtLabLocation;
        public System.Windows.Forms.Label lblLabLoc;
        public System.Windows.Forms.TextBox txtLabName;
        public System.Windows.Forms.Label lblLabName;
        public System.Windows.Forms.TextBox txtLabCode;
        public System.Windows.Forms.Label lblLabCode;
        public System.Windows.Forms.TabPage tabRequests;
        public System.Windows.Forms.DataGridView dgvRequests;
        public System.Windows.Forms.Panel pnlReqCard;
        public System.Windows.Forms.Button btnReqSubmit;
        public System.Windows.Forms.TextBox txtReqIssue;
        public System.Windows.Forms.Label lblReqIssue;
        public System.Windows.Forms.ComboBox cboReqPriority;
        public System.Windows.Forms.Label lblReqPriority;
        public System.Windows.Forms.ComboBox cboReqEquipment;
        public System.Windows.Forms.Label lblReqEq;
        public System.Windows.Forms.Label lblReqTechnician;
        public System.Windows.Forms.ComboBox cboReqTechnician;
        public System.Windows.Forms.TabPage tabHistory;
        public System.Windows.Forms.DataGridView dgvLogs;
        public System.Windows.Forms.Panel pnlHisCard;
        public System.Windows.Forms.GroupBox grpAfterCondition;
        public System.Windows.Forms.Button btnHisComplete;
        public System.Windows.Forms.TextBox txtLogAction;
        public System.Windows.Forms.Label lblHisAction;
        public System.Windows.Forms.TextBox txtLogCost;
        public System.Windows.Forms.Label lblHisCost;
        public System.Windows.Forms.ComboBox cboPendingRequests;
        public System.Windows.Forms.Label lblHisPending;
        public System.Windows.Forms.TabPage tabStats;
        public System.Windows.Forms.DataGridView dgvStatsLab;
        public System.Windows.Forms.DataGridView dgvStatsStatus;
        public System.Windows.Forms.Panel pnlCardCost;
        public System.Windows.Forms.Label lblCardCostVal;
        public System.Windows.Forms.Label lblCardCostTitle;
        public System.Windows.Forms.Panel pnlCardBroken;
        public System.Windows.Forms.Label lblCardBrokenVal;
        public System.Windows.Forms.Label lblCardBrokenTitle;
        public System.Windows.Forms.Panel pnlCardTotalEq;
        public System.Windows.Forms.Label lblCardTotalVal;
        public System.Windows.Forms.Label lblCardTotalTitle;
        public System.Windows.Forms.TabPage tabUsers;
        public System.Windows.Forms.DataGridView dgvUsers;
        public System.Windows.Forms.Panel pnlUserCard;
        public System.Windows.Forms.Button btnUserDelete;
        public System.Windows.Forms.Button btnUserAdd;
        public System.Windows.Forms.ComboBox cboNewRole;
        public System.Windows.Forms.Label lblUserRole;
        public System.Windows.Forms.TextBox txtNewFullName;
        public System.Windows.Forms.Label lblUserFullName;
        public System.Windows.Forms.TextBox txtNewPass;
        public System.Windows.Forms.Label lblUserPass;
        public System.Windows.Forms.TextBox txtNewUser;
        public System.Windows.Forms.Label lblUserName;
    }
}
