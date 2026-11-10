# LabCare Pro

Ứng dụng Windows Forms quản lý thiết bị và bảo trì phòng thí nghiệm, dùng .NET 10 và SQLite.

## Chạy ứng dụng

Trên Windows, cài .NET 10 SDK rồi mở `WinFormsApp1.slnx` bằng Visual Studio hỗ trợ .NET 10, hoặc chạy:

```powershell
dotnet run --project WinFormsApp1/WinFormsApp1.csproj
```

Các tài khoản mẫu trong CSDL mới: `admin`, `ql_lab1`, `ktv_01`; mật khẩu mẫu là `123456`. Đây là tài khoản phục vụ chấm bài/demo. Màn hình đăng nhập để trống, bạn tự nhập tài khoản và mật khẩu.

Mật khẩu được băm bằng PBKDF2-SHA256 (600.000 vòng, salt ngẫu nhiên cho mỗi mật khẩu). CSDL cũ được nâng cấp tự động, giữ nguyên mật khẩu đăng nhập. Tài khoản mới cũng chỉ lưu giá trị băm. Không dùng tài khoản mẫu cho dữ liệu thật.

Hướng dẫn đóng gói và kịch bản trình bày: [HUONG_DAN_NOP_BAI.md](HUONG_DAN_NOP_BAI.md).

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

## Quy tắc bảo trì và bảo vệ dữ liệu

Khi sửa thiết bị, phòng lab được chọn theo đúng thiết bị; không được để trống mã, tên hoặc phòng lab. Thống kê “Đang bảo trì / Chờ sửa” chỉ tính thiết bị Hỏng hoặc Đang bảo trì, không tính thiết bị Thanh lý.

Mỗi thiết bị chỉ có một phiếu chưa hoàn thành. Khi còn phiếu mở, không thể sửa thủ công trạng thái thiết bị sang trạng thái khác Đang bảo trì. Danh sách tạo phiếu chỉ hiển thị thiết bị chưa thanh lý và không có phiếu đang mở; service và CSDL tiếp tục kiểm tra khi lưu.

Khi hoàn tất, phải chọn **Sửa thành công - thiết bị tốt** hoặc **Chưa sửa được - thiết bị hỏng**. Kết quả sửa chữa dùng để ghi chú chi tiết. Lịch sử lưu tình trạng sau bảo trì ngay sau ngày hoàn thành; lịch sử cũ chưa có kết quả được hiển thị là **Chưa ghi nhận**. Khi đổi phiếu, lựa chọn tình trạng, ghi chú và chi phí được xóa để tránh dùng nhầm dữ liệu của phiếu trước.

Chi phí là số nguyên không âm theo VND. `1.950.000`, `1950000`, `1,950,000` đều được hiểu là **1.950.000**. Lịch sử và tổng chi phí thống kê dùng định dạng Việt Nam.

Thiết bị chưa từng có phiếu bảo trì được xóa sau khi xác nhận. Thiết bị đã có phiếu phải giữ lại để bảo toàn lịch sử và có thể chuyển sang **Thanh lý** khi đã hoàn tất các phiếu mở. Phòng Lab chỉ được xóa khi không còn thiết bị, kể cả thiết bị thanh lý. CSDL mới dùng khóa ngoại; CSDL cũ được bổ sung trigger bảo vệ quan hệ mà không xóa dữ liệu.

## Chạy kiểm tra

Chạy trên Windows:

```powershell
dotnet run --project tests/LabCare.RegressionTests/LabCare.RegressionTests.csproj --configuration Release
```

Bộ kiểm tra dùng CSDL tạm riêng, kiểm tra nâng cấp CSDL cũ, phân công, tự nhận phiếu, chặn xử lý phiếu của người khác, giao dịch và ghi lịch sử, giao diện theo vai trò, điều hướng sau khi gỡ tab và đăng xuất. GitHub Actions chạy cùng các kiểm tra này trên Windows khi push hoặc mở pull request.

