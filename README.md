# HỆ THỐNG PHÊ DUYỆT TÀI LIỆU PHÂN TÁN
## Distributed Document Approval System

### 1. Giới thiệu
Dự án xây dựng hệ thống quản lý và phê duyệt tài liệu, hỗ trợ người dùng tạo, theo dõi và xử lý quy trình phê duyệt tài liệu.

### 2. Công nghệ sử dụng
- Backend: ASP.NET Core Web API (.NET 10)
- Frontend: HTML, CSS, JavaScript
- Database: Microsoft SQL Server
- Công cụ phát triển: Visual Studio 2022

### 3. Kiến trúc dự án
DocumentApprovalSystem/
├── DocumentApprovalSystem.slnx
├── global.json
├── README.md
└── src/
    ├── Document.API/
    ├── Notification.Service/
    └── Document.Client/
        ├── index.html
        ├── css/
        │   └── style.css
        └── js/
            └── app.js 
### 4. Cấu trúc chức năng

Hệ thống hiện triển khai các chức năng chính:

- Tạo tài liệu mới.
- Hiển thị danh sách tài liệu.
- Xem thông tin chi tiết tài liệu thông qua API.
- Gửi tài liệu để chờ phê duyệt.
- Phê duyệt tài liệu.
- Từ chối tài liệu kèm lý do.
- Theo dõi trạng thái xử lý tài liệu.
- Thống kê số lượng tài liệu theo trạng thái.

### 5. Quy trình phê duyệt

Tài liệu được xử lý theo quy trình:

```text
       Tạo tài liệu
            |
            v
         Draft
            |
            v
       Gửi duyệt
            |
            v
         Pending
          /   \
         /     \
        v       v
    Approved  Rejected
```

Các trạng thái:
- `Draft`: Tài liệu mới tạo, chưa gửi duyệt.
- `Pending`: Tài liệu đang chờ phê duyệt.
- `Approved`: Tài liệu đã được phê duyệt.
- `Rejected`: Tài liệu bị từ chối.

### 6. Các API đã triển khai

| Phương thức | Endpoint | Chức năng |
|---|---|---|
| POST | `/api/Documents` | Tạo tài liệu |
| GET | `/api/Documents` | Lấy danh sách tài liệu |
| GET | `/api/Documents/{id}` | Lấy chi tiết tài liệu |
| POST | `/api/Documents/{id}/submit` | Gửi duyệt |
| POST | `/api/Documents/{id}/approve` | Phê duyệt |
| POST | `/api/Documents/{id}/reject` | Từ chối tài liệu |

### 7. Cơ sở dữ liệu

Hệ thống sử dụng Microsoft SQL Server với cơ sở dữ liệu:

`DocumentApprovalDB`

Bảng dữ liệu chính:
- `Documents`: Lưu thông tin tài liệu, người tạo, trạng thái và thời gian xử lý.
- `__EFMigrationsHistory`: Lưu lịch sử migration của Entity Framework Core.

Entity Framework Core được sử dụng để ánh xạ đối tượng C# với cơ sở dữ liệu và quản lý cấu trúc bảng thông qua Migration.

### 8. Hướng dẫn chạy dự án

**Bước 1: Mở dự án**

Mở `DocumentApprovalSystem.slnx` bằng Visual Studio 2022.

**Bước 2: Kiểm tra SQL Server**

Đảm bảo SQL Server đang chạy và cơ sở dữ liệu `DocumentApprovalDB` đã được tạo.

Kiểm tra chuỗi kết nối trong:

`src/Document.API/appsettings.json`

**Bước 3: Chạy Backend**

Mở Terminal tại thư mục gốc dự án và chạy:

```powershell
dotnet run --project src/Document.API --urls http://localhost:5264
```

API được truy cập tại:

`http://localhost:5264`

Tài liệu OpenAPI:

`http://localhost:5264/openapi/v1.json`

**Bước 4: Chạy Frontend**

Mở Terminal thứ hai tại thư mục gốc dự án:

```powershell
python -m http.server 5500 --directory src/Document.Client
```

Truy cập giao diện:

`http://localhost:5500`

### 9. Kết quả kiểm thử

Các chức năng đã được kiểm thử:

- Tạo tài liệu và lưu dữ liệu vào SQL Server.
- Lấy danh sách và thông tin chi tiết tài liệu.
- Chuyển trạng thái từ `Draft` sang `Pending`.
- Phê duyệt tài liệu từ `Pending` sang `Approved`.
- Từ chối tài liệu từ `Pending` sang `Rejected`.
- Lưu và hiển thị lý do từ chối.
- Kiểm tra điều kiện chuyển trạng thái không hợp lệ.
- Kết nối giao diện HTML/CSS/JavaScript với Backend Web API.

### 10. Trạng thái phát triển

Dự án đã hoàn thành phần khởi tạo hệ thống, xây dựng REST API, kết nối SQL Server và tích hợp giao diện người dùng với quy trình phê duyệt tài liệu cơ bản.

Các thành phần mở rộng của kiến trúc phân tán sẽ được triển khai trong những giai đoạn tiếp theo.