const API_URL = "http://pharmaproject.runasp.net/api/Admin";
const API_ADD_URL = "http://pharmaproject.runasp.net/api/Admin/signup";
const LOGS_API_URL = "http://pharmaproject.runasp.net/api/Admin/logs";
const TOKEN = localStorage.getItem("TOKEN");
let userData = [];
// ================= Fetch Users =================
async function fetchUsers() {
    try {
        const res = await fetch(`${API_URL}`, {
            headers: { Authorization: `Bearer ${TOKEN}` }
        });
        const result = await res.json();
        console.log("FETCH USERS RESPONSE:", result);

        const users = result.data; 

        userData = users;

        const tbody = document.querySelector("table tbody");
        tbody.innerHTML = "";

        users.forEach(user => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${user.adminId}</td>
                <td>${user.username}</td>
                <td>${user.role}</td>
                <td>${new Date(user.createdAt).toLocaleString()}</td>
                <td class="gap-lg-3 gap-2 d-flex flex-column flex-md-row px-3">
                    <a href="#"
                        class="edit-btn btn w-100 w-md-50"
                        data-id="${user.adminId}"
                        data-bs-toggle="modal"
                        data-bs-target="#editUserModal">Edit</a>
                    <a href="#"
                        class="delete-btn btn w-100 w-md-50"
                        data-id="${user.adminId}">Delete</a>
                </td>
            `;
            tbody.appendChild(tr);
        });

        attachTableEvents();
    } catch (err) {
        console.error("Fetch users error:", err);
    }
}

// ================= Attach Table Events =================
function attachTableEvents() {
// Edit User
document.querySelectorAll(".edit-btn").forEach(btn => {
        btn.addEventListener("click", () => {
            const id = Number(btn.dataset.id);
            const user = userData.find(u => u.adminId === id);
            if (!user) return alert("User not found");

            // Fill modal inputs
            document.getElementById("editUserName").value = user.username;
            document.getElementById("editUserEmail").value = user.email;
            document.getElementById("editUserRole").value = user.role;
            document.getElementById("editUserPassword").value = "";

            // Store the user ID in the modal for later use
            document.getElementById("editUserModal").dataset.userId = id;
        });
    });

    // Delete User
    document.querySelectorAll(".delete-btn").forEach(btn => {
        btn.addEventListener("click", async () => {
            if (!confirm("Are you sure you want to delete this user?")) return;
            const id = btn.dataset.id;
            try {
                await fetch(`${API_URL}/${id}`, {
                    method: "DELETE",
                    headers: { Authorization: `Bearer ${TOKEN}` }
                });
                fetchUsers();
            } catch (err) {
                console.error("Delete user error:", err);
            }
        });
    });
}

// ================= Handle Edit User Save (ONE TIME) =================
document.addEventListener("DOMContentLoaded", () => {
    const saveBtn = document.querySelector("#editUserModal .btn-success");
    
    saveBtn.addEventListener("click", async (e) => {
        e.preventDefault(); // Prevent form submission
        
        const editModalEl = document.getElementById("editUserModal");
        const id = editModalEl.dataset.userId;
        
        const username = document.getElementById("editUserName").value.trim();
        const email = document.getElementById("editUserEmail").value.trim();
        const role = document.getElementById("editUserRole").value;
        const passwordInput = document.getElementById("editUserPassword").value.trim();

        if (!username || !email || !role) {
            return alert("Username, Email and Role are required");
        }

        const updatedUser = { username, email, role };
        if (passwordInput) updatedUser.password = passwordInput;

        console.log("Updating user:", updatedUser);

        try {
            const res = await fetch(`${API_URL}/${id}`, {
                method: "PUT",
                headers: {
                    "Authorization": `Bearer ${TOKEN}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(updatedUser)
            });

            if (!res.ok) {
                const errorText = await res.text();
                throw new Error(errorText);
            }

            // Hide modal
            const modalInstance = bootstrap.Modal.getInstance(editModalEl);
            modalInstance.hide();
            
            // Refresh users
            fetchUsers();
            
        } catch (err) {
            console.error("Update user error:", err);
            alert("Error updating user: " + err.message);
        }
    });




// Delete User
    document.querySelectorAll(".delete-btn").forEach(btn => {
        btn.addEventListener("click", async () => {
            if (!confirm("Are you sure you want to delete this user?")) return;
            const id = btn.dataset.id;
            try {
                await fetch(`${API_URL}/${id}`, {
                    method: "DELETE",
                    headers: { Authorization: `Bearer ${TOKEN}` }
                });
                fetchUsers();
            } catch (err) {
                console.error("Delete user error:", err);
            }
        });
    });
});

// ================= Search for User =================
document.querySelector(".search-btn").addEventListener("click", e => {
    e.preventDefault();
    const q = document.querySelector(".search").value.toLowerCase();
    document.querySelectorAll("tbody tr").forEach(row => {
        const name = row.children[1].textContent.toLowerCase();
        const id = row.children[0].textContent.toLowerCase();
        row.style.display = name.includes(q) || id.includes(q) ? "" : "none";
    });
});

// ================= Add User =================
document.addEventListener("DOMContentLoaded", () => {
    const addBtn = document.querySelector("#addUserModal .btn-success");
    const userNameInput = document.getElementById("addUserName");
    const userEmailInput = document.getElementById("addUserEmail");
    const userPasswordInput = document.getElementById("addUserPassowrd");
    const userRoleSelect = document.getElementById("addUserRole");
    
    if (!addBtn || !userNameInput || !userEmailInput || !userPasswordInput || !userRoleSelect) {
        return console.error("One or more Add User elements not found!");
    }

    addBtn.addEventListener("click", async () => {
        const username = userNameInput.value.trim();
        const email = userEmailInput.value.trim();
        const password = userPasswordInput.value.trim();
        const role = userRoleSelect.value;

        if (!username || !email || !password || !role) {
            return alert("Please fill all fields");
        }

        const data = { username, email, password, role };

        try {
            const res = await fetch(API_ADD_URL, {
                method: "POST",
                headers: {
                    "Authorization": `Bearer ${TOKEN}`,
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(data)
            });

            if (!res.ok) {
                const text = await res.text();
                throw text;
            }

            bootstrap.Modal.getInstance(document.getElementById("addUserModal")).hide();
            fetchUsers();

        } catch (err) {
            console.error("ADD USER ERROR:", err);
            alert("Error adding user: " + err);
        }
    });
});




// ================= Initialize =================
document.addEventListener("DOMContentLoaded", () => {
    fetchUsers();
});

// ================= Fetch Logs =================
async function fetchLogs() {
    try {
        const res = await fetch(`${LOGS_API_URL}`, {
            headers: { "Authorization": "Bearer " + TOKEN }
        });
        const data = await res.json();
        console.log("FETCH LOGS RESPONSE:", data);

        const logs = data.records; 
        console.log(logs);
        
        const tbody = document.getElementById("userLogsTbody");
        tbody.innerHTML = "";

        logs.forEach(log => {
            const tr = document.createElement("tr");
            tr.innerHTML = `
                <td>${log.adminUsername}</td>
                <td>${log.adminId}</td>
                <td>${log.actionType}</td>
                <td>${log.actionTime}</td>
            `;
            tbody.appendChild(tr);
        });

    } catch (err) {
        console.error("Fetch logs error:", err);
    }
}


// ================== INIT ==================
fetchLogs();