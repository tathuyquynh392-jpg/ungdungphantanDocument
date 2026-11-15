# HỆ THỐNG PHÊ DUYỆT TÀI LIỆU PHÂN TÁN
## Distributed Document Approval System

### 1. Giới thiệu

Dự án xây dựng hệ thống quản lý và phê duyệt tài liệu phân tán, hỗ trợ người dùng tạo, theo dõi và xử lý tài liệu theo quy trình phê duyệt.

Hệ thống được phát triển theo định hướng kiến trúc ứng dụng phân tán, trong đó Backend cung cấp các API để giao tiếp với giao diện người dùng và cơ sở dữ liệu.

### 2. Công nghệ sử dụng

- Backend: ASP.NET Core Minimal API (.NET 10).
- Frontend: HTML, CSS, JavaScript.
- Database: Microsoft SQL Server.
- ORM: Entity Framework Core 10.
- API Documentation: OpenAPI.
- Công cụ phát triển: Visual Studio 2022.
- Quản lý mã nguồn: Git và GitHub.

### 3. Kiến trúc và cấu trúc dự án

```text
DocumentApprovalSystem/
├── DocumentApprovalSystem.slnx
├── global.json
├── README.md
└── src/
    ├── Document.API/
    │   ├── Data/
    │   ├── DTOs/
    │   ├── Endpoints/
    │   ├── Migrations/
    │   ├── Models/
    │   ├── Program.cs
    │   └── appsettings.json
    ├── Notification.Service/
    └── Document.Client/
        ├── index.html
        ├── css/
        │   └── style.css
        └── js/
            └── app.js
```

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
- Kiểm tra tính hợp lệ của các thao tác chuyển trạng thái.

Backend sử dụng ASP.NET Core Minimal API để định nghĩa và xử lý các HTTP endpoint. Các endpoint được tổ chức tập trung trong `Endpoints/DocumentsEndpoints.cs`.

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

Hệ thống kiểm tra điều kiện chuyển trạng thái nhằm đảm bảo tài liệu chỉ được phê duyệt hoặc từ chối khi đang ở trạng thái `Pending`.

### 6. Các API đã triển khai

| Phương thức | Endpoint | Chức năng |
|---|---|---|
| POST | `/api/Documents` | Tạo tài liệu |
| GET | `/api/Documents` | Lấy danh sách tài liệu |
| GET | `/api/Documents/{id}` | Lấy chi tiết tài liệu |
| POST | `/api/Documents/{id}/submit` | Gửi duyệt |
| POST | `/api/Documents/{id}/approve` | Phê duyệt |
| POST | `/api/Documents/{id}/reject` | Từ chối tài liệu |

Các API được xây dựng bằng Minimal API, sử dụng Entity Framework Core để truy vấn và cập nhật dữ liệu trong SQL Server.

### 7. Cơ sở dữ liệu

Hệ thống sử dụng Microsoft SQL Server với cơ sở dữ liệu:

`DocumentApprovalDB`

Các bảng dữ liệu chính:

- `Documents`: Lưu thông tin tài liệu, người tạo, trạng thái và thời gian xử lý.
- `__EFMigrationsHistory`: Lưu lịch sử migration của Entity Framework Core.

Entity Framework Core được sử dụng để ánh xạ đối tượng C# với cơ sở dữ liệu và quản lý cấu trúc bảng thông qua Migration.

### 8. Hướng dẫn chạy dự án

**Bước 1: Mở dự án**

Mở file `DocumentApprovalSystem.slnx` bằng Visual Studio 2022 hoặc mở Terminal tại thư mục gốc dự án.

**Bước 2: Kiểm tra SQL Server**

Đảm bảo SQL Server đang chạy và cơ sở dữ liệu `DocumentApprovalDB` đã được tạo.

Kiểm tra chuỗi kết nối trong:

`src/Document.API/appsettings.json`

**Bước 3: Build dự án**

```powershell
dotnet build
```

**Bước 4: Chạy Backend**

```powershell
dotnet run --project src/Document.API --urls http://localhost:5264
```

API được truy cập tại:

`http://localhost:5264`

Tài liệu OpenAPI:

`http://localhost:5264/openapi/v1.json`

**Bước 5: Chạy Frontend**

Mở Terminal thứ hai tại thư mục gốc dự án:

```powershell
python -m http.server 5500 --directory src/Document.Client
```

Truy cập giao diện:

`http://localhost:5500`

### 9. Kết quả kiểm thử

Các chức năng đã được kiểm thử thực tế:

- Tạo tài liệu và lưu dữ liệu vào SQL Server.
- Lấy danh sách và thông tin chi tiết tài liệu.
- Chuyển trạng thái từ `Draft` sang `Pending`.
- Phê duyệt tài liệu từ `Pending` sang `Approved`.
- Từ chối tài liệu từ `Pending` sang `Rejected`.
- Lưu và hiển thị lý do từ chối.
- Kiểm tra điều kiện chuyển trạng thái không hợp lệ.
- Kiểm tra API trả về `400 Bad Request` khi cố gắng phê duyệt tài liệu đã bị từ chối.
- Kiểm tra các endpoint thông qua OpenAPI.
- Kiểm tra giao diện HTML/CSS/JavaScript kết nối với Backend Minimal API.
- Build toàn bộ solution thành công sau khi loại bỏ Controller cũ.

### 10. Trạng thái phát triển

Dự án đã hoàn thành các phần:

- Khởi tạo Solution và các project thành phần.
- Xây dựng Backend bằng ASP.NET Core Minimal API.
- Xây dựng các API quản lý và phê duyệt tài liệu.
- Kết nối SQL Server bằng Entity Framework Core.
- Tích hợp giao diện người dùng HTML, CSS và JavaScript.
- Kiểm thử các luồng nghiệp vụ phê duyệt và từ chối tài liệu.

Các giai đoạn tiếp theo sẽ tập trung phát triển kiến trúc phân tán, tích hợp RabbitMQ để truyền nhận sự kiện, triển khai Notification Service, Docker và Docker Compose, đồng thời bổ sung cơ chế xử lý lỗi và khả năng phục hồi của hệ thống.