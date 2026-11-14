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

```text
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