let cart = [];
// API Configuration
if (typeof API_BASE_URL === 'undefined') {
    var API_BASE_URL = "http://pharmaproject.runasp.net/api";
}
if (typeof AUTH_TOKEN === 'undefined') {
    var AUTH_TOKEN = localStorage.getItem("TOKEN");
}

async function loadCashierProducts() {
    try {
        const res = await fetch("http://pharmaproject.runasp.net/api/Drugs", {
            headers: {
                Authorization: `Bearer ${AUTH_TOKEN}`
            }
        });
        const data = await res.json();
        console.log("API DATA:", data);
        
        const productsRow = document.querySelector(".col-lg-9 .row.g-3");
        productsRow.innerHTML = "";
        
        data.forEach(item => {
            // Check if item is out of stock using shelfAmount (available for sale)
            const stockAmount = item.shelfAmount ?? 0;
            const isOutOfStock = stockAmount === 0;
            const isLowStock = !isOutOfStock && stockAmount <= (item.lowAmount || 10);
            
            productsRow.innerHTML += `
                <div class="col-lg-3 col-md-4 col-6 items-card" 
                    data-id="${item.drugId}" 
                    data-name="${item.name.toLowerCase()}"
                    data-barcode="${item.barcode || ''}"
                    data-stock="${stockAmount}">
                    <div class="card pt-2 d-flex ${isOutOfStock ? 'opacity-100' : ''}">
                        <div class="d-flex justify-content-center item-img">
                            <img src="http://pharmaproject.runasp.net${item.imageUrl}"
                                class="card-img-top"
                                alt="${item.name}"
                                onerror="this.src='https://via.placeholder.com/150?text=No+Image'">
                        </div>
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title item-name">${item.name}</h5>
                            
                            ${isOutOfStock ? 
                                '<p class="text-danger fw-bold">Out of Stock!</p>' : 
                                (isLowStock ? `<p class="text-warning">Only ${stockAmount} in stock!</p>` : '')
                            }
                            
                            <div class="d-flex justify-content-between align-items-center item-price-btn pt-2 qty-wrapper mt-auto">
                                <p class="item-price mb-0">${item.sellingPrice} EGP</p>
                                <button class="btn btn-primary card-btn" 
                                    onclick="addItem(this)"
                                    ${isOutOfStock ? 'disabled' : ''}>
                                    ${isOutOfStock ? 'Out of Stock' : 'Add'}
                                </button>
                                <div class="counter align-items-center gap-2" style="display:none;">
                                    <button class="btn btn-outline-secondary btn-sm" onclick="decrease(this)">-</button>
                                    <span class="qty">1</span>
                                    <button class="btn btn-outline-secondary btn-sm" onclick="increase(this)">+</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            `;
        });

    } catch (error) {
        console.error("Error loading cashier products:", error);
    }
}
/* ================== SEARCH PRODUCTS ================== */
function searchProducts() {
    const searchInput = document.querySelector('.search').value.toLowerCase().trim();
    const allCards = document.querySelectorAll('.items-card');
    allCards.forEach(card => {
        const id = card.dataset.id;
        const name = card.dataset.name;
        const barcode = card.dataset.barcode;
        // Check if search matches ID, name, or barcode
        const matchesSearch = 
            id.includes(searchInput) ||
            name.includes(searchInput) ||
            barcode.includes(searchInput);
        if (matchesSearch || searchInput === '') {
            card.style.display = '';
        } else {
            card.style.display = 'none';
        }
    });
}

/* ================== ADD ITEM TO CART ================== */
function addItem(button) {
    const card = button.closest('.items-card');
    const id = card.dataset.id;
    const name = card.querySelector('.item-name').textContent;
    const price = parseFloat(card.querySelector('.item-price').textContent.replace('$', ''));
    const img = card.querySelector('img').src;

    console.log('ADD ITEM CLICKED');
    console.log('ID:', id);
    console.log('Name:', name);
    console.log('Price:', price);

    // Check if item already exists in cart
    const existingItem = cart.find(item => item.id === id);
    console.log('Existing item?', existingItem);

    if (!existingItem) {
        // Add new item to cart
        cart.push({
            id: id,
            name: name,
            price: price,
            quantity: 1,
            img: img
        });
        console.log('NEW ITEM ADDED TO CART');
    } else {
        console.log('ITEM ALREADY EXISTS - NOT ADDING');
    }

    console.log('Current cart:', JSON.stringify(cart, null, 2));
    console.log('Cart length:', cart.length);

    // Hide Add button, show counter
    const qtyWrapper = card.querySelector('.qty-wrapper');
    button.style.display = 'none';
    const counter = card.querySelector('.counter');
    counter.style.display = 'flex';
    counter.querySelector('.qty').textContent = '1';
    
    // Ensure wrapper maintains proper layout
    if (qtyWrapper) {
        qtyWrapper.style.display = 'flex';
        qtyWrapper.style.justifyContent = 'space-between';
        qtyWrapper.style.alignItems = 'center';
    }

    updateCartDisplay();
}

/* ================== INCREASE QUANTITY ================== */
function increase(button) {
    const counter = button.closest('.counter');
    const qtySpan = counter.querySelector('.qty');
    let qty = parseInt(qtySpan.textContent);
    qty++;
    qtySpan.textContent = qty;

    const card = button.closest('.items-card');
    if (card) {
        const id = card.dataset.id;
        const cartItem = cart.find(item => item.id === id);
        if (cartItem) {
            cartItem.quantity = qty;
            updateCartDisplay();
        }
    }
}

/* ================== DECREASE QUANTITY ================== */
function decrease(button) {
    const counter = button.closest('.counter');
    const qtySpan = counter.querySelector('.qty');
    let qty = parseInt(qtySpan.textContent);

    const card = button.closest('.items-card');

    if (card) {
        if (qty > 1) {
            qty--;
            qtySpan.textContent = qty;

            const id = card.dataset.id;
            const cartItem = cart.find(item => item.id === id);
            if (cartItem) {
                cartItem.quantity = qty;
                updateCartDisplay();
            }
        } else {
            // Remove from cart
            const id = card.dataset.id;
            cart = cart.filter(item => item.id !== id);

            // Show Add button, hide counter
            const addButton = card.querySelector('.card-btn');
            const qtyWrapper = card.querySelector('.qty-wrapper');
            addButton.style.display = '';
            counter.style.display = 'none';
            qtySpan.textContent = '1';
            
            // Reset wrapper layout
            if (qtyWrapper) {
                qtyWrapper.style.display = '';
                qtyWrapper.style.justifyContent = '';
                qtyWrapper.style.alignItems = '';
            }

            updateCartDisplay();
        }
    }
}

/* ================== REMOVE ITEM FROM CART ================== */
function removeItem(id) {
    // Remove from cart array
    cart = cart.filter(item => item.id !== id);

    // Reset left card
    const leftCard = document.querySelector(`.items-card[data-id="${id}"]`);
    if (leftCard) {
        const addButton = leftCard.querySelector('.card-btn');
        const counter = leftCard.querySelector('.counter');
        const qtyWrapper = leftCard.querySelector('.qty-wrapper');
        addButton.style.display = '';
        counter.style.display = 'none';
        counter.querySelector('.qty').textContent = '1';
        
        // Reset wrapper layout
        if (qtyWrapper) {
            qtyWrapper.style.display = '';
            qtyWrapper.style.justifyContent = '';
            qtyWrapper.style.alignItems = '';
        }
    }

    updateCartDisplay();
}

/* ================== CHANGE QUANTITY FROM CART ================== */
function changeQty(id, delta) {
    const item = cart.find(i => i.id === id);
    if (!item) return;

    item.quantity += delta;

    if (item.quantity <= 0) {
        removeItem(id);
    } else {
        // Update left card counter
        const leftCard = document.querySelector(`.items-card[data-id="${id}"]`);
        if (leftCard) {
            const leftQty = leftCard.querySelector('.counter .qty');
            if (leftQty) leftQty.textContent = item.quantity;
        }

        updateCartDisplay();
    }
}

/* ================== UPDATE CART DISPLAY ================== */
function updateCartDisplay() {
    const containers = document.querySelectorAll('.cart-cards, #cartOffcanvas .offcanvas-body');

    let cartHTML = '';
    cart.forEach(item => {
        cartHTML += `
        <div class="card mb-3 cart-item" data-id="${item.id}">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-center mb-2">
                    <h6 class="mb-0">${item.name}</h6>
                    <i class="bi bi-trash text-danger" style="cursor: pointer;"
                        onclick="removeItem('${item.id}')">
                    </i>
                </div>
                <div class="d-flex justify-content-between align-items-center">
                    <div class="d-flex gap-2 align-items-center">
                        <button class="btn btn-outline-secondary btn-sm" onclick="changeQty('${item.id}', -1)">-</button>
                        <span>${item.quantity}</span>
                        <button class="btn btn-outline-secondary btn-sm" onclick="changeQty('${item.id}', 1)">+</button>
                    </div>
                    <p class="mb-0 fw-bold">${(item.price * item.quantity).toFixed(2)}$</p>
                </div>
            </div>
        </div>`;
    });

    // Set HTML to all containers at once
    containers.forEach(container => {
        container.innerHTML = cartHTML;
    });

    // Update badge count
    const badge = document.querySelector('.badge');
    if (badge) {
        badge.textContent = cart.length;
    }

    updateTotal();
}

/* ================== TOTAL & CHANGE ================== */
function updateTotal() {
    const total = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);

    document.querySelectorAll('.total-box span')
        .forEach(el => el.textContent = `${total.toFixed(2)} EGP`);

    // Update both desktop and mobile cash inputs
    const cashInputs = document.querySelectorAll('.cash-input input');
    const changeSpans = document.querySelectorAll('.change-box span');

    cashInputs.forEach((cashInput, index) => {
        const changeSpan = changeSpans[index];
        if (cashInput && changeSpan) {
            const cash = parseFloat(cashInput.value) || 0;
            const change = cash - total;
            changeSpan.textContent = `${(change > 0 ? change : 0).toFixed(2)} EGP`;
        }
    });
}
/* ================== CHECKOUT - CREATE INVOICE ================== */
async function checkout() {
    if (cart.length === 0) {
        alert('Cart is empty! Please add items before checkout.');
        return;
    }

    const cashInput = document.querySelector('.cash-input input');
    const cashPaid = parseFloat(cashInput?.value) || 0;
    const total = cart.reduce((sum, item) => sum + (item.price * item.quantity), 0);

    if (cashPaid < total) {
        alert('Insufficient payment! Please enter cash amount greater than or equal to total.');
        return;
    }

    try {
        // Get the logged-in admin's ID from localStorage
        const adminData = JSON.parse(localStorage.getItem("ADMIN_DATA")) || 
                        JSON.parse(localStorage.getItem("userData"));
        
        if (!adminData || !adminData.adminId) {
            alert('Error: Admin ID not found. Please log in again.');
            console.error('Admin data:', adminData);
            return;
        }

        const currentAdminId = adminData.adminId;
        console.log('Creating invoice for Admin ID:', currentAdminId);

        // Prepare invoice data
        const invoiceData = {
            adminId: currentAdminId,
            items: cart.map(item => ({
                drugId: parseInt(item.id),
                quantity: parseInt(item.quantity),
                unitPrice: parseFloat(item.price)
            })),
            totalAmount: parseFloat(total.toFixed(2)),
            discountAmount: 0,
            taxAmount: 0,
            amountPaid: parseFloat(cashPaid.toFixed(2))
        };

        console.log('=== CREATING INVOICE ===');
        console.log('Cart contents:', cart);
        console.log('Invoice Data:', JSON.stringify(invoiceData, null, 2));
        console.log('API URL:', `${API_BASE_URL}/Invoice`);

        const response = await fetch(`${API_BASE_URL}/Invoice`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                Authorization: `Bearer ${AUTH_TOKEN}`
            },
            body: JSON.stringify(invoiceData)
        });

        console.log('Response Status:', response.status);
        
        // Get response text first
        const responseText = await response.text();
        console.log('Response Text:', responseText);

        if (!response.ok) {
            let errorMessage = `Failed to create invoice (Status: ${response.status})`;
            try {
                const errorData = JSON.parse(responseText);
                console.error('API Error Data:', errorData);
                
                if (errorData.message) {
                    errorMessage += `\n${errorData.message}`;
                } else if (errorData.errors) {
                    errorMessage += `\n${JSON.stringify(errorData.errors, null, 2)}`;
                } else if (errorData.title) {
                    errorMessage += `\n${errorData.title}`;
                }
            } catch (e) {
                errorMessage += `\n${responseText}`;
            }
            throw new Error(errorMessage);
        }

        // Parse successful response
        const result = JSON.parse(responseText);
        console.log('Invoice created successfully:', result);

        const change = cashPaid - total;
        alert(`Invoice created successfully!\n\nInvoice ID: ${result.invoiceId || result.id || 'N/A'}\nTotal: ${total.toFixed(2)} EGP\nPaid: ${cashPaid.toFixed(2)} EGP\nChange: ${change.toFixed(2)} EGP`);

        cancelCart(true);

        // Redirect to view invoice
        if (result.invoiceId !== undefined) {
            if (confirm('Would you like to view the invoice?')) {
                window.location.href = `../Invoices/viewInvoice.html?id=${result.invoiceId}`;
            }
        }

    } catch (error) {
        console.error('=== CHECKOUT ERROR ===');
        console.error('Error:', error);
        alert('Failed to create invoice.\n\n' + error.message + '\n\nCheck browser console (F12) for details.');
    }
}

/* ================== CANCEL CART ================== */
function cancelCart(skipConfirmation = false) {
    if (cart.length === 0 && !skipConfirmation) {
        alert('Cart is already empty!');
        return;
    }

    if (!skipConfirmation) {
        const confirmed = confirm('Are you sure you want to clear all items from the cart?');
        if (!confirmed) return;
    }

    // Clear the cart
    cart = [];

    // Reset all product cards
    const allCards = document.querySelectorAll('.items-card');
    allCards.forEach(card => {
        const addButton = card.querySelector('.card-btn');
        const counter = card.querySelector('.counter');
        const qtyWrapper = card.querySelector('.qty-wrapper');
        if (addButton && counter) {
            addButton.style.display = '';
            counter.style.display = 'none';
            counter.querySelector('.qty').textContent = '1';
            
            // Reset wrapper layout
            if (qtyWrapper) {
                qtyWrapper.style.display = '';
                qtyWrapper.style.justifyContent = '';
                qtyWrapper.style.alignItems = '';
            }
        }
    });

    // Reset cash input
    const cashInputs = document.querySelectorAll('.cash-input input');
    cashInputs.forEach(input => input.value = '');

    // Update display
    updateCartDisplay();

    if (!skipConfirmation) {
        console.log('Cart cleared successfully');
    }
}

// ================= Update Admin Name =================
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


/* ================== INIT ================== */
document.addEventListener("DOMContentLoaded", () => {
    loadCashierProducts();
    updateAdminName();

    // Update admin name periodically for 2 seconds
    const interval = setInterval(updateAdminName, 200);
    setTimeout(() => clearInterval(interval), 2000);

    // Handle search
    const searchForm = document.querySelector('form');
    const searchInput = document.querySelector('.search');
    
    if (searchForm) {
        searchForm.addEventListener('submit', (e) => {
            e.preventDefault();
            searchProducts();
        });
    }
    
    if (searchInput) {
        searchInput.addEventListener('input', searchProducts);
    }

    // Handle all cash inputs (desktop and mobile)
    const cashInputs = document.querySelectorAll('.cash-input input');
    cashInputs.forEach(input => {
        input.addEventListener('input', updateTotal);
    });

    // Handle all cancel buttons (desktop and mobile)
    const cancelButtons = document.querySelectorAll('.cancel');
    cancelButtons.forEach(button => {
        button.addEventListener('click', () => cancelCart(false));
    });
    // Handle all checkout buttons (desktop and mobile)
    const checkoutButtons = document.querySelectorAll('.check-out');
    checkoutButtons.forEach(button => {
        button.addEventListener('click', checkout);
    });
});