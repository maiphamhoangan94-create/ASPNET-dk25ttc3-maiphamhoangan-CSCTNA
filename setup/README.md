# DỮ LIỆU VÀ HƯỚNG DẪN – BẾP NHÀ

Thành phần cần giữ trong thư mục `setup`:

- `du-lieu/anmph_db.bak`: bản sao lưu cơ sở dữ liệu đã kiểm tra hợp lệ bằng RESTORE VERIFYONLY.
- `du-lieu/KHOI-PHUC.sql`: mẫu lệnh khôi phục; sửa đường dẫn theo máy triển khai trước khi chạy.
- `huong-dan/HUONG-DAN-CAI-DAT.md`: hướng dẫn cài môi trường, khôi phục dữ liệu và chạy mã nguồn.
- `huong-dan/NGUON-TAI.md`: các đường dẫn tải phần mềm.
- `huong-dan/SO-DO-TRIEN-KHAI.html`: sơ đồ triển khai, mở bằng trình duyệt.

## Cách sử dụng

1. Tải và cài SQL Server 2025 Express, Visual Studio Community với workload **ASP.NET and web development** và .NET 10 SDK theo tài liệu nguồn tải.
2. Khôi phục `du-lieu/anmph_db.bak` vào SQL Server 2025 hoặc phiên bản mới hơn tương thích.
3. Mở tệp `.csproj` trong thư mục mã nguồn bằng Visual Studio. Chỉnh `ConnectionStrings:DefaultConnection` trong `appsettings.json` để trỏ đến instance chứa database `anmph_db`.
4. Chạy profile `http`, mở http://localhost:5064 và kiểm tra các chức năng trình diễn.

Giữ mã nguồn và hình ảnh trong `wwwroot` khi chuyển sang máy khác. Đối chiếu dữ liệu và tài khoản trình diễn với báo cáo trước khi nộp.
