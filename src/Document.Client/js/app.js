
// Dữ liệu mẫu ban đầu
const documents = [];

// Hiển thị danh sách tài liệu
function renderDocuments() {
    const documentList = document.getElementById("documentList");

    if (documents.length === 0) {
        documentList.innerHTML = `
            <tr>
                <td colspan="4">Chưa có tài liệu</td>
            </tr>
        `;
        return;
    }

    documentList.innerHTML = documents.map(document => `
        <tr>
            <td>${document.id}</td>
            <td>${document.title}</td>
            <td>${document.creator}</td>
            <td>${document.status}</td>
        </tr>
    `).join("");
}

// Cập nhật số liệu thống kê
function updateDashboard() {
    document.getElementById("totalDocuments").textContent =
        documents.length;

    document.getElementById("pendingDocuments").textContent =
        documents.filter(d => d.status === "Chờ phê duyệt").length;

    document.getElementById("approvedDocuments").textContent =
        documents.filter(d => d.status === "Đã phê duyệt").length;

    document.getElementById("rejectedDocuments").textContent =
        documents.filter(d => d.status === "Bị từ chối").length;
}

// Khởi tạo giao diện
function initializeApp() {
    renderDocuments();
    updateDashboard();
}

initializeApp();
