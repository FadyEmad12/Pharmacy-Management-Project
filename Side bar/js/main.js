
function updateAdminName() {
    const adminData =
        JSON.parse(localStorage.getItem("ADMIN_DATA")) ||
        JSON.parse(localStorage.getItem("userData"));

    console.log("Updating admin name, data:", adminData);

    if (!adminData || !adminData.username) {
        console.warn("No admin data found");
        return;
    }

    const nameElements = document.querySelectorAll(".adminName");
    console.log("Found adminName elements:", nameElements.length);

    if (nameElements.length === 0) {
        setTimeout(updateAdminName, 100);
        return;
    }

    nameElements.forEach(el => {
        el.textContent = adminData.username;
    });
}

if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", updateAdminName);
} else {
    updateAdminName();
}

window.addEventListener("load", updateAdminName);

const LOGOUT_API = "http://pharmaproject.runasp.net/api/Admin/logout";

document.addEventListener("click", async (e) => {
    const logoutBtn = e.target.closest(".logout-btn");
    if (!logoutBtn) return;

    e.preventDefault();

    const TOKEN = localStorage.getItem("TOKEN");

    try {
        if (TOKEN) {
            await fetch(LOGOUT_API, {
                method: "POST",
                headers: {
                    Authorization: `Bearer ${TOKEN}`,
                    "Content-Type": "application/json"
                }
            });
        }
    } catch (err) {
        console.warn("Logout API error (ignored)");
    } finally {
        localStorage.removeItem("ADMIN_DATA");
        localStorage.removeItem("ADMIN_ID");
        localStorage.removeItem("userData");
        localStorage.removeItem("TOKEN");

        window.location.href = "/index.html";
    }
});

document.addEventListener("click", (e) => {
    const link = e.target.closest(".sidebar-fixed ul li a, .offcanvas-body ul li a");
    if (!link) return;

    e.preventDefault();

    const pageMap = {
        "Dashboard": "../Dashboard/index.html",
        "Medicines": "../Medicine/index.html",
        "Admin": "../User management/index.html",
        "Invoices": "../Invoices/index.html",
        "Cashier": "../Cashier/index.html"
    };

    const text = link.querySelector("span")?.textContent.trim();
    const targetPage = pageMap[text];

    if (targetPage) {
        window.location.href = targetPage;
    }
});