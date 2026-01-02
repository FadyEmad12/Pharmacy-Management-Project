// API Configuration
if (typeof API_BASE_URL === 'undefined') {
    var API_BASE_URL = "http://pharmaproject.runasp.net/api";
}
if (typeof AUTH_TOKEN === 'undefined') {
    var AUTH_TOKEN = localStorage.getItem("TOKEN");
}

/* ================== LOAD ALL INVOICES ================== */
async function loadInvoices() {
    try {
        const response = await fetch(`${API_BASE_URL}/Invoice`, {
            headers: {
                Authorization: `Bearer ${AUTH_TOKEN}`
            }
        });

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Error response:', errorText);
            throw new Error(`Failed to load invoices (${response.status})`);
        }

        const invoices = await response.json();
        const tbody = document.querySelector('.invoices-table-body');
        tbody.innerHTML = '';

        if (!invoices || invoices.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center">No invoices found</td>
                </tr>
            `;
            return;
        }

        invoices.forEach((invoice, index) => {
            const invoiceId = invoice.invoiceId;
            const adminUsername = invoice.adminUsername || 'Unknown Admin';
            const totalAmount = invoice.totalAmount || 0;
            
            // Parse invoiceTime correctly with date AND time
            let dateString = 'N/A';
            if (invoice.invoiceTime) {
                try {
                    const dateObj = new Date(invoice.invoiceTime);
                    if (!isNaN(dateObj.getTime())) {
                        const date = dateObj.toLocaleDateString('en-GB');
                        const time = dateObj.toLocaleTimeString('en-GB', { 
                            hour: '2-digit', 
                            minute: '2-digit',
                            hour12: false 
                        });
                        dateString = `${date} ${time}`;
                    }
                } catch (e) {
                    console.error('Error parsing date:', e);
                }
            }
            
            tbody.innerHTML += `
                <tr>
                    <td>${index + 1}</td>
                    <td>${adminUsername}</td>
                    <td>${totalAmount.toFixed(2)} EGP</td>
                    <td>${dateString}</td>
                    <td class="invoices-btn text-center">
                        <a href="./viewInvoice.html?id=${invoiceId}" class="btn btn-primary btn-sm">View</a>
                    </td>
                </tr>
            `;
        });

    } catch (error) {
        console.error('Error loading invoices:', error);
        const tbody = document.querySelector('.invoices-table-body');
        tbody.innerHTML = `
            <tr>
                <td colspan="5" class="text-center text-danger">
                    Failed to load invoices. ${error.message}<br>
                    <small>Check console (F12) for details</small>
                </td>
            </tr>
        `;
    }
}
/* ================== SEARCH INVOICES ================== */
function searchInvoices() {
    const searchInput = document.querySelector('.search').value.toLowerCase();
    const rows = document.querySelectorAll('.invoices-table-body tr');
    
    rows.forEach(row => {
        if (row.cells.length < 5) return;
        
        const adminName = row.cells[1].textContent.toLowerCase();
        const invoiceId = row.cells[0].textContent.toLowerCase();
        
        if (adminName.includes(searchInput) || invoiceId.includes(searchInput)) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}

/* ================== INIT ================== */
document.addEventListener('DOMContentLoaded', () => {
    loadInvoices();
    
    // Setup search
    const searchForm = document.querySelector('form');
    if (searchForm) {
        searchForm.addEventListener('submit', (e) => {
            e.preventDefault();
            searchInvoices();
        });
    }
    
    const searchInput = document.querySelector('.search');
    if (searchInput) {
        searchInput.addEventListener('input', searchInvoices);
    }
});