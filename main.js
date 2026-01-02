
const LOGIN_API = "http://pharmaproject.runasp.net/api/Admin/login";
const ADMINS_API = "http://pharmaproject.runasp.net/api/Admin";

document.addEventListener("DOMContentLoaded", () => {

    const loginBtn = document.querySelector(".login-btn");
    const usernameInput = document.querySelector("input[placeholder='Username']");
    const passwordInput = document.querySelector("input[placeholder='Password']");

    // Error message
    let errorMsg = document.getElementById("error-msg");
    if (!errorMsg) {
        errorMsg = document.createElement("div");
        errorMsg.id = "error-msg";
        errorMsg.style.color = "red";
        errorMsg.style.margin = "10px 0";
        errorMsg.style.display = "none";
        loginBtn.parentNode.insertBefore(errorMsg, loginBtn);
    }

    // Loader
    let loader = document.getElementById("loader");
    if (!loader) {
        loader = document.createElement("div");
        loader.className = "spinner-border text-light";
        loader.style.width = "1.5rem";
        loader.style.height = "1.5rem";
        loader.style.display = "none";
        loader.style.marginLeft = "10px";
        loginBtn.parentNode.appendChild(loader);
    }

    async function login(username, password) {
        try {
            errorMsg.style.display = "none";
            loader.style.display = "inline-block";

            // ================= LOGIN =================
            const loginRes = await fetch(LOGIN_API, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ username, password })
            });

            if (!loginRes.ok) {
                throw new Error("Invalid username or password");
            }

            const loginData = await loginRes.json();
            console.log("LOGIN RESPONSE:", loginData);

            const token = loginData.token;
            if (!token) throw new Error("Token not returned");

            localStorage.setItem("TOKEN", token);

            // ================= GET ADMINS =================
            const adminsRes = await fetch(ADMINS_API, {
                headers: {
                    Authorization: `Bearer ${token}`
                }
            });

            if (!adminsRes.ok) {
                throw new Error("Failed to fetch admins");
            }

            const adminsResponse = await adminsRes.json();
            console.log("ADMINS:", adminsResponse);

            const admins = adminsResponse.data;

            const currentAdmin = admins.find(
                admin => admin.username === username
            );

            if (!currentAdmin) {
                throw new Error("Admin not found");
            }

            // ================= SAVE DATA =================
            localStorage.setItem("ADMIN_ID", currentAdmin.adminId);
            localStorage.setItem("ADMIN_DATA", JSON.stringify(currentAdmin));

            // ================= REDIRECT =================
            window.location.href = "../Dashboard/index.html";

        } catch (err) {
            console.error("LOGIN ERROR:", err);
            errorMsg.textContent = err.message || "Login failed";
            errorMsg.style.display = "block";
        } finally {
            loader.style.display = "none";
        }
    }

    loginBtn.addEventListener("click", e => {
        e.preventDefault();

        const username = usernameInput.value.trim();
        const password = passwordInput.value.trim();

        if (!username || !password) {
            errorMsg.textContent = "Please enter username and password";
            errorMsg.style.display = "block";
            return;
        }

        login(username, password);
    });

});
