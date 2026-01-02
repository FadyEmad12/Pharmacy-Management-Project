// API Configuration
if (typeof API_BASE_URL === 'undefined') {
    var API_BASE_URL = "http://pharmaproject.runasp.net/api";
}
if (typeof AUTH_TOKEN === 'undefined') {
    var AUTH_TOKEN = localStorage.getItem("TOKEN");
}

/* ================== LOAD SPECIFIC INVOICE ================== */
async function loadInvoiceDetails() {
    // Get invoice ID from URL
    const urlParams = new URLSearchParams(window.location.search);
    const invoiceId = urlParams.get('id');

    console.log('Loading invoice ID:', invoiceId);

    if (!invoiceId || invoiceId === 'N/A' || invoiceId === 'undefined' || invoiceId === 'null') {
        alert('Invalid invoice ID provided');
        window.location.href = 'invoices.html';
        return;
    }

    try {
        console.log('Fetching from:', `${API_BASE_URL}/Invoice/${invoiceId}`);
        
        const response = await fetch(`${API_BASE_URL}/Invoice/${invoiceId}`, {
            headers: {
                Authorization: `Bearer ${AUTH_TOKEN}`
            }
        });

        console.log('Response status:', response.status);

        if (!response.ok) {
            const errorText = await response.text();
            console.error('Error response:', errorText);
            throw new Error(`Failed to load invoice details (${response.status})`);
        }

        const invoice = await response.json();
        console.log('Invoice details loaded:', invoice);
        const adminUsername = invoice.adminUsername || 'Admin';
        document.querySelector('.casher-name').textContent = adminUsername;
        
        // Parse invoiceTime correctly with date AND time
        let dateString = 'N/A';
        if (invoice.invoiceTime) {
            try {
                const dateObj = new Date(invoice.invoiceTime);
                if (!isNaN(dateObj.getTime())) {
                    const date = dateObj.toLocaleDateString('en-GB');
                    const time = dateObj.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' });
                    dateString = `${date} ${time}`;
                }
            } catch (e) {
                console.error('Error parsing date:', e);
            }
        }
        document.querySelector('.invc-date').textContent = dateString;

        const items = invoice.items || [];
        console.log('Items:', items);

        // Populate table
        const tbody = document.querySelector('.table-group-divider');
        tbody.innerHTML = '';

        if (!items || items.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center">No items in this invoice</td>
                </tr>
            `;
        } else {
            // If items don't have drug names, we need to fetch them
            // For now, we'll show what we have and fetch drug names
            await populateInvoiceItems(items, tbody);
        }

        // Update total using correct field name
        const totalAmount = invoice.totalAmount || 0;
        document.querySelector('.invc-total').textContent = `${totalAmount.toFixed(2)} EGP`;

    } catch (error) {
        console.error('Error loading invoice details:', error);
        alert(`Failed to load invoice details.\n\n${error.message}\n\nYou will be redirected to the invoices page.`);
        setTimeout(() => {
            window.location.href = 'invoices.html';
        }, 2000);
    }
}

/* ================== POPULATE INVOICE ITEMS WITH DRUG NAMES ================== */
async function populateInvoiceItems(items, tbody) {
    let itemNumber = 1;

    for (const item of items) {
        const drugId = item.drugId;
        const quantity = item.quantity || 0;

        let drugName = `Drug #${drugId}`;
        let unitPrice = 0;

        try {
            const drugResponse = await fetch(`${API_BASE_URL}/Drugs/${drugId}`, {
                headers: {
                Authorization: `Bearer ${AUTH_TOKEN}`
                }
            });

            if (drugResponse.ok) {
                const drugData = await drugResponse.json();
                drugName = drugData.name || drugData.drugName || drugName;
                unitPrice = drugData.sellingPrice || 0; 
            }
        } catch (error) {
            console.error('Error fetching drug data:', error);
        }

        const totalPrice = unitPrice * quantity;

        tbody.innerHTML += `
            <tr>
                <th scope="row">${itemNumber++}</th>
                <td>${drugName}</td>
                <td>${quantity}</td>
                <td>${unitPrice.toFixed(2)} EGP</td>
                <td>${totalPrice.toFixed(2)} EGP</td>
            </tr>
        `;
    }
}

/* ================== INIT ================== */
document.addEventListener('DOMContentLoaded', () => {
    loadInvoiceDetails();
});