# Hướng dẫn cài đặt và triển khai

## 1. Yêu cầu và thành phần

Windows x64; SQL Server 2025 Express (17.x) hoặc bản SQL Server tương thích mới hơn. Backup được đọc bằng RESTORE HEADERONLY: database anmph_db, phiên bản engine 17.0.4025, compatibility level 170, bản full ngày 25/10/2026. Không khôi phục backup này vào SQL Server phiên bản cũ hơn.

Để biên dịch và chạy mã nguồn cần .NET 10 SDK; cài Visual Studio Community và chọn workload **ASP.NET and web development**, bảo đảm có SDK .NET 10.

## 2. Cài SQL Server và khôi phục dữ liệu

1. Tải bộ cài SQL Server từ liên kết trong NGUON-TAI.md, chạy bộ cài, cài Database Engine, chọn Windows Authentication. Ghi lại tên instance; hướng dẫn này dùng `.\SQLEXPRESS`. Bộ cài cần Internet. Nếu dùng tên instance khác, sửa cấu hình kết nối ở bước 3.
2. Dùng SQL Server Management Studio (SSMS) kết nối instance vừa cài với tài khoản có quyền khôi phục database. Nếu chưa có SSMS, tải từ https://learn.microsoft.com/en-us/ssms/install/install . SSMS là công cụ quản trị riêng, không thay thế SQL Server Database Engine.
3. Sao chép cả thư mục setup đến vị trí cố định, ví dụ `C:\CSCTNA\setup`. Mở `du-lieu/KHOI-PHUC.sql`; sửa mọi đường dẫn theo vị trí thực tế. Tạo thư mục `C:\CSCTNA\sql-data` và cấp tài khoản dịch vụ SQL Server quyền ghi; cấp quyền đọc tệp .bak.
4. Chạy HEADERONLY, VERIFYONLY, FILELISTONLY trước. Đối chiếu logical names trong kết quả với hai tên trong WITH MOVE; điều chỉnh nếu cần. Chạy khối RESTORE còn lại. Script dừng nếu database anmph_db đã có, không dùng WITH REPLACE.
5. Có thể dùng giao diện SSMS: Databases → Restore Database → Device → chọn .bak → Files → chọn đường dẫn MDF/LDF hợp lệ → OK. Không chọn ghi đè database hiện có.
6. Kiểm tra database có các bảng Users, Categories, Posts, Provinces, PostProvinces, PostComments, PostRatings. Có thể chạy `SELECT COUNT(*) FROM dbo.Posts;` và đối chiếu báo cáo.

## 3. Mở và chạy mã nguồn

Mở tệp `.csproj` tại thư mục gốc bằng Visual Studio. Kết nối mặc định của mã nguồn là `(localdb)\MSSQLLocalDB`, database `anmph_db`; phải khôi phục database vào đúng instance hoặc sửa appsettings.json. Chạy profile http, địa chỉ http://localhost:5064.

Nếu cần tạo bản chạy riêng, mở PowerShell tại thư mục mã nguồn:

```powershell
dotnet publish .\ASPNET-dk25ttc3-maiphamhoangan-CSCTNA.csproj -c Release -r win-x64 --self-contained true -o .\artifacts\publish
```

Cần Internet để restore NuGet nếu máy chưa có cache. Project đã loại setup khỏi các đầu vào build/publish để tránh đóng gói lặp.

## 4. Kiểm tra trình diễn

- Mở trang chủ, danh sách và chi tiết công thức/góc bếp; kiểm tra ảnh, danh mục và tỉnh thành.
- Đăng ký tài khoản thử qua `/Account/Register`, đăng nhập tại `/Account/Login`.
- Kiểm tra bình luận, đánh giá và đăng bài theo chức năng báo cáo.
- Tài khoản quản trị phải dùng thông tin hợp lệ của bộ dữ liệu cung cấp; gói này không đặt lại hay suy đoán mật khẩu. Cần người cung cấp xác nhận tài khoản quản trị trước buổi bảo vệ.
- Thư mục wwwroot được lấy từ mã nguồn hiện tại; nếu ảnh được lưu ngoài mã nguồn thì phải bổ sung từ bản gốc. Đối chiếu số lượng bản ghi và nội dung với báo cáo trước khi nộp.

## 5. Xử lý lỗi

- Không kết nối SQL: kiểm tra dịch vụ, tên instance, Windows user và quyền database.
- Restore báo phiên bản không phù hợp: dùng SQL Server 2025 hoặc mới hơn tương thích.
- Access denied khi restore: cấp quyền cho tài khoản dịch vụ SQL Server tại đường dẫn .bak và MDF/LDF, không chỉ cho user đang mở SSMS.
- Invalid object name Posts: chưa restore đúng database/instance. Chương trình không tự tạo toàn bộ schema; không thay restore bằng database rỗng.
- Cổng 5064 bận: đổi applicationUrl trong Properties/launchSettings.json và mở đúng địa chỉ mới.
- Ảnh không hiện: kiểm tra đường dẫn bản ghi và các tệp tương ứng trong wwwroot.

## 6. Phạm vi kiểm chứng

Backup đã được kiểm tra hợp lệ bằng RESTORE VERIFYONLY. Cần khôi phục trên máy triển khai và kiểm tra nghiệp vụ, dữ liệu theo báo cáo. Tải các phần mềm riêng theo NGUON-TAI.md.

