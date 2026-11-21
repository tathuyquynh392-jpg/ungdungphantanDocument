const API_URL = "http://localhost:5264/api/Documents";
// ===============================
// TOAST NOTIFICATION
// ===============================

function showToast(message, type = "success") {
    const toast = document.getElementById("toast");

    if (!toast) {
        console.error("Không tìm thấy phần tử #toast trong HTML!");
        return;
    }

    toast.textContent = message;
    toast.className = "toast";

    if (type === "error") {
        toast.classList.add("error");
    } else if (type === "info") {
        toast.classList.add("info");
    }

    toast.classList.add("show");

    setTimeout(() => {
        toast.classList.remove("show");
    }, 3000);
}
// ===============================
// SIGNALR - REALTIME CONNECTION
// ===============================
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5264/hubs/documents")
    .withAutomaticReconnect()
    .build();

// Bắt đầu kết nối SignalR
connection.start()
    .then(() => {
        console.log("SignalR: Đã kết nối thành công!");

        // Tài liệu mới được tạo
        connection.on("DocumentCreated", (document) => {
            console.log("Realtime: Tài liệu mới được tạo!", document);

            showToast(`Tài liệu "${document.title}" vừa được tạo!`);
            loadDocuments();
        });

        // Tài liệu được gửi duyệt
        connection.on("DocumentSubmitted", (document) => {
            console.log("Realtime: Tài liệu đã gửi duyệt!", document);

            showToast(`Tài liệu "${document.title}" đã được gửi duyệt!`, "info");
            loadDocuments();
        });

        // Tài liệu được phê duyệt
        connection.on("DocumentApproved", (document) => {
            console.log("Realtime: Tài liệu đã được phê duyệt!", document);

            showToast(`Tài liệu "${document.title}" đã được phê duyệt!`);
            loadDocuments();
        });

        // Tài liệu bị từ chối
        connection.on("DocumentRejected", (document) => {
            console.log("Realtime: Tài liệu đã bị từ chối!", document);

            showToast(`Tài liệu "${document.title}" đã bị từ chối!`, "error");
            loadDocuments();
        });
    })
    .catch(error => {
        console.error("SignalR: Kết nối thất bại!", error);
    });

// Chuyển trạng thái thành tiếng Việt
function getStatusText(status) {
    const statuses = {
        0: "Bản nháp",
        1: "Chờ phê duyệt",
        2: "Đã phê duyệt",
        3: "Bị từ chối"
    };

    return statuses[status] ?? "Không xác định";
}

// Lấy danh sách tài liệu từ API
async function loadDocuments() {
    try {
        const response = await fetch(API_URL);

        if (!response.ok) {
            throw new Error("Không thể tải danh sách tài liệu.");
        }

        const documents = await response.json();

        renderDocuments(documents);
        updateDashboard(documents);
    } catch (error) {
        console.error("Lỗi:", error);

        document.getElementById("documentList").innerHTML = `
            <tr>
                <td colspan="5">Không thể kết nối đến máy chủ API</td>
            </tr>
        `;
    }
}

// Hiển thị danh sách

function renderDocuments(documents) {
    const documentList = document.getElementById("documentList");

    if (documents.length === 0) {
        documentList.innerHTML = `
            <tr>
                <td colspan="6">Chưa có tài liệu</td>
            </tr>
        `;
        return;
    }

    documentList.innerHTML = documents.map(doc => {
        let actionButtons = "";

        if (doc.status === 0) {
            actionButtons = `
                <button class="btn-submit"
                    onclick="submitDocument(${doc.id})">
                    Gửi duyệt
                </button>
            `;
        }

        if (doc.status === 1) {
            actionButtons = `
                <button class="btn-approve"
                    onclick="approveDocument(${doc.id})">
                    Phê duyệt
                </button>

                <button class="btn-reject"
                    onclick="rejectDocument(${doc.id})">
                    Từ chối
                </button>
            `;
        }

        return `
            <tr>
                <td>${doc.id}</td>
                <td>${doc.title}</td>
                <td>${doc.createdBy}</td>
                <td>${getStatusText(doc.status)}</td>
                <td>${doc.rejectionReason || "-"}</td>
                <td>${actionButtons || "-"}</td>
            </tr>
        `;
    }).join("");
}



// Cập nhật thống kê
function updateDashboard(documents) {
    document.getElementById("totalDocuments").textContent =
        documents.length;

    document.getElementById("pendingDocuments").textContent =
        documents.filter(d => d.status === 1).length;

    document.getElementById("approvedDocuments").textContent =
        documents.filter(d => d.status === 2).length;

    document.getElementById("rejectedDocuments").textContent =
        documents.filter(d => d.status === 3).length;
}

// Tạo tài liệu mới
async function createDocument(event) {
    event.preventDefault();

    const formMessage = document.getElementById("formMessage");

    const documentData = {
        title: document.getElementById("title").value.trim(),
        description: document.getElementById("description").value.trim(),
        fileName: document.getElementById("fileName").value.trim(),
        createdBy: document.getElementById("createdBy").value.trim()
    };

    try {
        const response = await fetch(API_URL, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(documentData)
        });

        if (!response.ok) {
            throw new Error("Tạo tài liệu thất bại.");
        }

        formMessage.textContent = "Tạo tài liệu thành công!";
        formMessage.style.color = "green";

        document.getElementById("createDocumentForm").reset();

        // Tải lại danh sách và thống kê
        await loadDocuments();

    } catch (error) {
        console.error("Lỗi:", error);

        formMessage.textContent = "Không thể tạo tài liệu. Vui lòng thử lại.";
        formMessage.style.color = "red";
    }
}

async function submitDocument(id) {
    if (!confirm("Bạn có chắc chắn muốn gửi tài liệu này đi phê duyệt?")) {
        return;
    }

    try {
        const response = await fetch(`${API_URL}/${id}/submit`, {
            method: "POST"
        });

        if (!response.ok) {
            throw new Error("Gửi duyệt tài liệu thất bại.");
        }

        alert("Gửi duyệt tài liệu thành công!");

        await loadDocuments();

    } catch (error) {
        console.error("Lỗi:", error);
        alert("Không thể gửi duyệt tài liệu.");
    }
}

// Phê duyệt tài liệu
async function approveDocument(id) {
    if (!confirm("Bạn có chắc chắn muốn phê duyệt tài liệu này?")) {
        return;
    }

    try {
        const response = await fetch(`${API_URL}/${id}/approve`, {
            method: "POST"
        });

        if (!response.ok) {
            throw new Error("Phê duyệt thất bại.");
        }

        alert("Phê duyệt tài liệu thành công!");

        await loadDocuments();
    } catch (error) {
        console.error(error);
        alert("Không thể phê duyệt tài liệu.");
    }
}

// Từ chối tài liệu
async function rejectDocument(id) {
    const reason = prompt("Nhập lý do từ chối tài liệu:");

    if (reason === null) {
        return;
    }

    if (reason.trim() === "") {
        alert("Lý do từ chối không được để trống!");
        return;
    }

    try {
        const response = await fetch(`${API_URL}/${id}/reject`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                reason: reason.trim()
            })
        });

        if (!response.ok) {
            throw new Error("Từ chối tài liệu thất bại.");
        }

        alert("Đã từ chối tài liệu!");

        await loadDocuments();
    } catch (error) {
        console.error(error);
        alert("Không thể từ chối tài liệu.");
    }
}

// Khởi tạo ứng dụng
document
    .getElementById("createDocumentForm")
    .addEventListener("submit", createDocument);

loadDocuments();
