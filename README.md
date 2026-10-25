# Bếp Nhà — Chia sẻ công thức nấu ăn

Đồ án ASP.NET Core MVC xây dựng website chia sẻ công thức, khám phá món ăn theo tỉnh thành và giới thiệu góc bếp. README được đối chiếu với mã nguồn và lịch sử commit đến `f6e5df9`.

## Tác giả mã nguồn

- Họ và tên: Mai Phạm Hoàng Ân.
- Email: [anmph300894@tvu-onschool.edu.vn](mailto:anmph300894@tvu-onschool.edu.vn).
- Số điện thoại: 0397241771.

## Công nghệ

- ASP.NET Core MVC, Razor Views trên **.NET 10** (`net10.0`).
- Entity Framework Core **10.0.12**, SQL Server, Microsoft.Data.SqlClient **6.1.6**.
- Bootstrap, jQuery, jQuery Validation, CSS và JavaScript.
- Đăng nhập bằng session với thời gian chờ 30 phút; vai trò `User` và `Admin`.

## Chức năng đã triển khai

### Người dùng

- Đăng ký, đăng nhập bằng tên đăng nhập hoặc email, đăng xuất.
- Xem hồ sơ cá nhân và bài chia sẻ của tác giả. Chủ tài khoản xem được cả bài chờ duyệt, đã duyệt và bị từ chối; người khác chỉ xem bài đã duyệt.
- Xem danh sách công thức có phân trang, lọc theo danh mục và xem chi tiết.
- Đăng công thức với nguyên liệu, cách làm, ảnh tải lên, danh mục và các tỉnh thành liên quan. Bài mới ở trạng thái chờ duyệt.
- Bình luận, trả lời bình luận và đánh giá công thức từ 1 đến 5 sao.
- Khám phá món ăn theo tỉnh thành.
- Chọn nguyên liệu có sẵn để nhận gợi ý từ công thức đã duyệt. Hệ thống chuẩn hóa, gộp tên và đếm tần suất nguyên liệu; kết quả ưu tiên số nguyên liệu trùng nhiều hơn, số nguyên liệu thiếu ít hơn, rồi đến bài mới hơn, tối đa 12 công thức.
- Chia sẻ góc bếp với 1–5 ảnh, xem chi tiết và đánh giá sao. Trang chi tiết liên kết đến công thức đã duyệt của cùng tác giả.

### Quản trị

- Quản lý danh mục, công thức và tài khoản: danh sách, chi tiết, thêm, sửa, xóa.
- Duyệt hoặc từ chối công thức và góc bếp, ghi nhận lý do từ chối; xem và xóa góc bếp.
- Báo cáo tổng số công thức, góc bếp, người dùng, danh mục, bình luận, lượt đánh giá và điểm trung bình; thống kê bài theo trạng thái, danh mục và số bài trong 6 tháng gần nhất.
- Tài khoản `Admin` được chuyển đến trang quản lý danh mục sau khi đăng nhập.

## Cấu trúc repository

```text
README.md
ASPNET-dk25ttc3-maiphamhoangan-CSCTNA.slnx
ASPNET-dk25ttc3-maiphamhoangan-CSCTNA/
├── Areas/Admin/        # Controller và giao diện quản trị
├── Controllers/        # Trang chủ, tài khoản, công thức, góc bếp
├── Models/             # Entity, DbContext và ViewModel
├── Services/           # Xử lý tải ảnh
├── Views/              # Giao diện Razor
├── Properties/         # Cấu hình chạy cục bộ
├── wwwroot/            # CSS, JavaScript, thư viện và ảnh tải lên
├── Program.cs          # Dịch vụ, cập nhật một phần schema, routing
└── appsettings.json    # Kết nối SQL Server
```

## Cài đặt và chạy

### 1. Chuẩn bị

- .NET SDK 10 và SQL Server; mặc định dự án dùng SQL Server LocalDB trên Windows.
- Cơ sở dữ liệu `anmph_db` với schema phù hợp các entity trong `Models`.
- Terminal hoặc IDE hỗ trợ .NET 10.

### 2. Cấu hình database

Trong thư mục dự án, `appsettings.json` sử dụng:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=anmph_db;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Thay `DefaultConnection` nếu dùng SQL Server khác. Các bảng DbContext sử dụng gồm `Users`, `Categories`, `Posts`, `Provinces`, `PostProvinces`, `PostComments` và `PostRatings`.

**Repository hiện chưa có EF migration hoặc script SQL khởi tạo được theo dõi bằng Git.** Cần khôi phục database phù hợp hoặc chuẩn bị schema từ các model trước khi chạy; `dotnet ef database update` chưa đủ để thiết lập database từ bản clone hiện tại.

Khi khởi động, `Program.cs` cập nhật một phần bảng `Posts`: thêm `PostType`, `GalleryImageUrls` nếu thiếu và cho phép `CategoryId` nhận NULL. Đoạn này yêu cầu bảng `Posts` đã tồn tại và tài khoản SQL có quyền sửa bảng; không tạo toàn bộ database. Danh sách tỉnh thành được khai báo bằng `HasData` trong DbContext, chưa tự chèn vào database khi khởi động.

### 3. Chạy ứng dụng

Từ thư mục gốc repository:

```powershell
cd ASPNET-dk25ttc3-maiphamhoangan-CSCTNA
dotnet restore
dotnet build
dotnet run --launch-profile http
```

Mở [http://localhost:5064](http://localhost:5064). Profile `https` sử dụng thêm [https://localhost:7194](https://localhost:7194).

### 4. Sử dụng thử

1. Đăng ký tại `/Account/Register`, đăng nhập tại `/Account/Login`.
2. Chuẩn bị danh mục và tỉnh thành trong database; đăng công thức tại `/Recipes/Create` hoặc chia sẻ góc bếp tại `/KitchenCorners/Create`.
3. Để thử quản trị, dùng tài khoản có `Role = 'Admin'` trong database. Mã nguồn không seed tài khoản quản trị mặc định.
4. Duyệt bài tại `/Admin/Posts` hoặc `/Admin/KitchenCorners` để bài xuất hiện công khai.
5. Thử gợi ý tại `/Recipes/Inspiration` và báo cáo tại `/Admin/Reports`.

## Lưu trữ nội dung và hình ảnh

- Công thức và góc bếp dùng chung bảng `Posts`, phân biệt bằng `PostType`: `Recipe` hoặc `KitchenCorner`.
- Trạng thái bài: `0` — chờ duyệt, `1` — đã duyệt, `2` — từ chối.
- Nguyên liệu công thức được ghép vào `Content` dưới phần `## Nguyên liệu` để hiển thị và phục vụ gợi ý.
- Ảnh được lưu tại `wwwroot/uploads`; ứng dụng cần quyền ghi vào thư mục này. Khi chuyển máy cần sao chép ảnh cùng dữ liệu database.
- `ImageUploadService` hỗ trợ JPG/JPEG, PNG, WEBP, GIF, tối đa 5 MB mỗi tệp và kiểm tra phần đầu tệp. Luồng góc bếp kiểm tra riêng số lượng 1–5 ảnh và phần mở rộng.

## Các mốc phát triển theo commit

| Commit | Nội dung |
| --- | --- |
| `6d3b34b`, `abab983`, `dcc53ec` | Kết nối database, đăng ký và đăng nhập |
| `9460974`, `0023125`, `e7e2a1a` | Giao diện nấu ăn, quản trị và chuyển hướng Admin |
| `56fa9f8`, `6ed5313`, `82e284a` | Đăng bài, chi tiết công thức và bình luận |
| `76514f8`, `432d8ce`, `f369469` | Khám phá tỉnh thành, danh sách công thức và đánh giá sao |
| `e6a3460`, `8754811`, `ded0e60` | Hồ sơ tác giả, chia sẻ góc bếp và liên kết với món ăn |
| `7b7cd9f`, `cedabe3` | Nguyên liệu và gợi ý công thức theo nguyên liệu có sẵn |
| `da6f3bb`, `168f85f` | Đường dẫn danh mục và báo cáo quản trị |
| `2e1d774`, `ac1db8b` | Gộp tên, đếm tần suất và điều chỉnh gợi ý nguyên liệu |
| `f6e5df9` | Chuyển nhập đường dẫn hình ảnh sang tải tệp lên |
