# LabCare Pro

Ứng dụng Windows Forms quản lý thiết bị và bảo trì phòng thí nghiệm, dùng .NET 10 và SQLite.

## Chạy ứng dụng

Trên Windows, cài .NET 10 SDK rồi mở `WinFormsApp1.slnx` bằng Visual Studio hỗ trợ .NET 10, hoặc chạy:

```powershell
dotnet run --project WinFormsApp1/WinFormsApp1.csproj
```

Các tài khoản mẫu trong CSDL mới: `admin`, `ql_lab1`, `ktv_01`; mật khẩu mẫu là `123456`.

## Phân quyền

| Chức năng | Admin | LabManager | Technician |
| --- | --- | --- | --- |
| Xem thiết bị | Có | Có | Có |
| Thêm, sửa, xóa thiết bị | Có | Có | Không |
| Truy cập phòng thí nghiệm | Có | Có | Không |
| Truy cập và quản lý người dùng | Có | Không | Không |
| Tạo phiếu bảo trì | Chọn kỹ thuật viên | Chọn kỹ thuật viên | Tự nhận phiếu mình tạo |
| Hoàn tất bảo trì | Có | Không | Phiếu được giao cho mình |
| Xem lịch sử và thống kê | Có | Có | Có |

Technician có đúng bốn tab: **Thiết bị phòng Lab**, **Tạo phiếu bảo trì**, **Xử lý và lịch sử**, **Thống kê báo cáo**. Khung nhập và các nút thay đổi thiết bị được ẩn. Tab Người dùng chỉ xuất hiện với Admin; tab Phòng thí nghiệm không xuất hiện với Technician, kể cả khi chuyển tab bằng bàn phím.

LabManager và Admin chọn **Phân công kỹ thuật viên** khi tạo phiếu. Danh sách chỉ gồm tài khoản có vai trò Technician và bắt buộc chọn trước khi gửi. Technician tạo phiếu thì hệ thống tự gán cho chính tài khoản đang đăng nhập, không cần quản lý duyệt. Cột **Kỹ thuật viên phụ trách** thể hiện người nhận phiếu; lịch sử ghi lại người thực sự hoàn tất sửa chữa.

LabManager xem được lịch sử nhưng không có khung hoàn tất sửa chữa. Technician chỉ xử lý phiếu của mình. Admin giữ quyền xử lý bảo trì. Hoàn tất phiếu, ghi lịch sử và cập nhật trạng thái thiết bị nằm trong cùng một giao dịch; nếu còn phiếu chờ cho thiết bị đó thì trạng thái vẫn là Đang bảo trì.

Nút **Đăng xuất** đóng phiên hiện tại và mở lại màn hình đăng nhập, cho phép đổi tài khoản mà không cần chạy lại app. Đóng cửa sổ chính bằng X sẽ thoát ứng dụng.

## CSDL đã có dữ liệu

Ứng dụng tự thêm cột `AssignedTechnicianID` vào `MaintenanceRequests` khi chạy trên CSDL cũ; không cần xóa `LabMaintenance.db`. Phiếu cũ do Technician tạo được gán lại cho người tạo. Phiếu cũ chưa có người phụ trách được đánh dấu **Chưa phân công**; Technician có thể nhận bằng cách hoàn tất xử lý. Không được xóa tài khoản kỹ thuật viên còn phiếu đang được giao và chưa hoàn thành.

## Kiểm tra

Chạy trên Windows:

```powershell
dotnet run --project tests/LabCare.RegressionTests/LabCare.RegressionTests.csproj --configuration Release
```

Bộ kiểm tra dùng CSDL tạm riêng, kiểm tra nâng cấp CSDL cũ, phân công, tự nhận phiếu, chặn xử lý phiếu của người khác, giao dịch và ghi lịch sử, giao diện theo vai trò, điều hướng sau khi gỡ tab và đăng xuất. GitHub Actions chạy cùng các kiểm tra này trên Windows khi push hoặc mở pull request.
