# Hướng dẫn chạy và nộp LabCare Pro

## Phạm vi bài làm

Ứng dụng desktop Windows Forms bằng C#/.NET 10, SQLite; quản lý thiết bị phòng thí nghiệm và quy trình bảo trì theo ba vai trò. Ứng dụng chạy trên Windows, không chạy trong trình duyệt. Nếu đề bài yêu cầu web thì cần triển khai giao diện web và backend riêng.

## Chạy từ mã nguồn

1. Dùng Windows 10/11 x64, cài .NET 10 SDK.
2. Giải nén mã nguồn vào thư mục có quyền ghi; mở PowerShell tại thư mục có README.
3. Chạy `dotnet run --project WinFormsApp1/WinFormsApp1.csproj`.
4. Nhập một trong các tài khoản: `admin` (quản trị), `ql_lab1` (quản lý lab), `ktv_01` (kỹ thuật viên). Mật khẩu mẫu: `123456`.

## Tạo bản chạy để thầy không cần cài .NET

Chạy trên Windows, ở thư mục gốc repo:

```powershell
dotnet publish WinFormsApp1/WinFormsApp1.csproj -c Release -r win-x64 --self-contained true -o artifacts/LabCare-Pro
Copy-Item README.md, HUONG_DAN_NOP_BAI.md artifacts/LabCare-Pro/
Compress-Archive -Path artifacts/LabCare-Pro/* -DestinationPath artifacts/LabCare-Pro-win-x64.zip -Force
```

Nộp file ZIP này cùng mã nguồn nếu thầy yêu cầu. Giữ toàn bộ DLL và thư mục con trong bản publish, không chỉ gửi riêng EXE. Sau khi giải nén, mở PowerShell trong thư mục đó và chạy `.\WinFormsApp1.exe`.

GitHub Actions cũng tự build, kiểm tra và đóng gói Windows x64. Trong tab **Actions**, chọn lần chạy thành công của **Windows regression checks**, tải artifact **LabCare-Pro-win-x64**, giải nén và chạy EXE. Chỉ dùng bản có bước kiểm tra thành công.

## Kịch bản trình bày khoảng 5 phút

1. Đăng nhập Admin, giới thiệu thiết bị, phòng lab và ba vai trò.
2. Chọn máy hiện sóng ở LAB-02, sửa tên; chọn lại để kiểm tra máy vẫn ở LAB-02.
3. Trong tab Phòng Lab, chọn một phòng và sửa vị trí. Trong tab Người dùng, chọn tài khoản thử nghiệm, sửa họ tên với ô mật khẩu trống để giữ mật khẩu; nhập mật khẩu mới nếu muốn đặt lại. Sau đó tạo phiếu cho thiết bị đang rảnh, nhập sự cố, chọn ưu tiên và kỹ thuật viên `ktv_01`.
4. Đăng xuất, đăng nhập `ql_lab1`: có thể tạo/phân công phiếu, không được hoàn tất sửa chữa hay quản lý người dùng.
5. Đăng xuất, đăng nhập `ktv_01`: chọn phiếu được giao, nhập kết quả và chi phí `1.950.000`, chọn tình trạng sau sửa rồi hoàn tất.
6. Xem lịch sử và thống kê: kết quả, người thực hiện, chi phí, trạng thái thiết bị đã cập nhật.
7. Giải thích bảo vệ dữ liệu: mỗi thiết bị chỉ có một phiếu mở; không xóa thiết bị có lịch sử; không đổi trạng thái khi đang có phiếu mở; mật khẩu lưu dạng băm.

## Dữ liệu và sao lưu

`LabMaintenance.db` nằm cạnh EXE, không thay đổi khi chạy từ shortcut có thư mục **Start in** khác. Khi chạy bằng `dotnet run`, DB nằm trong `WinFormsApp1/bin/Debug/net10.0-windows/`; nếu dùng `-c Release`, DB nằm trong thư mục Release tương ứng. Bản publish dùng DB trong thư mục publish. Thư mục chứa EXE cần có quyền ghi.

Nâng cấp từ bản cũ: thoát ứng dụng, sao lưu DB đang sử dụng rồi chép DB đó vào cạnh EXE mới trước lần chạy đầu. Nếu đã có DB ở đích, sao lưu cả hai và xác định bản cần giữ trước khi thay thế. Bản mới không tự nhập DB từ thư mục làm việc để tránh chọn nhầm dữ liệu. Không cần xóa dữ liệu để nâng cấp cấu trúc CSDL.

Để sao lưu, thoát ứng dụng rồi sao chép `LabMaintenance.db` cạnh EXE sang thư mục riêng.

Bản đóng gói không kèm dữ liệu cá nhân. Để demo từ đầu, dùng một thư mục giải nén mới chưa có `LabMaintenance.db`. Không xóa CSDL thật để làm mới demo.

## Kiểm tra trước khi nộp

```powershell
dotnet build WinFormsApp1.slnx -c Release
dotnet run --project tests/LabCare.RegressionTests/LabCare.RegressionTests.csproj -c Release
```

Sau khi kiểm tra thành công, chạy thử bản ZIP trên Windows bằng các bước demo ở trên. Các chức năng tìm kiếm nâng cao, xuất Excel, đặt lịch bảo trì và dùng nhiều máy qua mạng chưa nằm trong phạm vi phiên bản này.
