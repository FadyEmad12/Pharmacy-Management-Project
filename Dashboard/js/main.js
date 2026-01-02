
const AUTH_TOKEN = localStorage.getItem("TOKEN");
// dashboard cards data

const DASHBOARD_API_URL = "http://pharmaproject.runasp.net/api/dashboard/cards";
async function getDashboardData() {
  try {
    const response = await fetch(DASHBOARD_API_URL, {
      headers: {
        Authorization: `Bearer ${AUTH_TOKEN}`
      }
    });

    const data = await response.json();
    console.log("Dashboard Data:", data);

    return data;

  } catch (error) {
    console.log("Error fetching dashboard data:", error);
  }
}

function renderDashboard(data) {
  if (!data) return;

  // Monthly Sales
  document.getElementById("monthlySales").textContent =
    data.monthlyIncome.toLocaleString() + " EGP";

  // Annual Sales
  document.getElementById("annualSales").textContent =
    data.annualIncome.toLocaleString() + " EGP";

  // Total Products
  document.getElementById("totalProducts").textContent =
    data.totalDrugsInStorage;

  // Close To Expiration
  document.getElementById("closeToExpiration").textContent =
    data.nearExpirationStorageAmount;
}

document.addEventListener("DOMContentLoaded", async () => {
  const dashboardData = await getDashboardData();
  renderDashboard(dashboardData);
});

const refreshBtn = document.getElementById("refreshBtn");

refreshBtn.addEventListener("click", async (e) => {
  e.preventDefault();

  refreshBtn.classList.add("disabled");
  refreshBtn.innerHTML = `
    <i class="fa-solid fa-spinner fa-spin"></i>
    Refreshing...
  `;

  const dashboardData = await getDashboardData();
  renderDashboard(dashboardData);

  refreshBtn.classList.remove("disabled");
  refreshBtn.innerHTML = `
    <i class="fa-solid fa-arrow-rotate-right"></i>
    Refresh Data
  `;
});

async function getBestSelling() {
  try {
    const response = await fetch("http://pharmaproject.runasp.net/api/dashboard/most-sold", {
      headers: { Authorization: `Bearer ${AUTH_TOKEN}` }
    });
    const data = await response.json();
    renderBestSelling(data);
  } catch (error) {
    console.log("Error fetching best-selling:", error);
  }
}
// best-selling products
const BEST_SELLING_API_URL = "http://pharmaproject.runasp.net/api/dashboard/most-sold";

async function getBestSelling() {
  try {
    const response = await fetch(BEST_SELLING_API_URL, {
      headers: { Authorization: `Bearer ${AUTH_TOKEN}` }
    });
    const data = await response.json();
    renderBestSelling(data);
  } 
  catch (error) {
    console.log("Error fetching best-selling products:", error);
    document.getElementById("bestSellingList").innerHTML = "<p class='text-muted small'>Error loading data.</p>";
  }
}

function renderBestSelling(products) {
  const container = document.getElementById("bestSellingList");
  container.innerHTML = "";

  products.slice(0, 4).forEach((product, index) => {
    const item = document.createElement("div");
    item.className = "list-item";

    item.innerHTML = `
      <div>
        <strong>${product.name}</strong>
        <div class="small text-muted">Barcode: ${product.barcode}</div>
      </div>
      <span class="badge bg-secondary">
        ${index === 0 ? "The Best" :
          index === 1 ? "Second" :
          index === 2 ? "Third" : "Fourth"}
      </span>
    `;

    container.appendChild(item);
  });
}


document.addEventListener("DOMContentLoaded", getBestSelling);

// low stock products
const LOW_STOCK_API_URL = "http://pharmaproject.runasp.net/api/dashboard/low-stock";

async function getLowStock() {
  try {
    const response = await fetch(LOW_STOCK_API_URL, {
      headers: { Authorization: `Bearer ${AUTH_TOKEN}` }
    });
    const data = await response.json();
    renderLowStock(data);

    console.log(data);
    
  } catch (error) {
    console.log("Error fetching low-stock products:", error);
    document.getElementById("lowStockList").innerHTML = "<p class='text-muted small'>Error loading data.</p>";
  }
}

function renderLowStock(products) {
  const container = document.getElementById("lowStockList");
  container.innerHTML = "";

  products.slice(0, 4).forEach(product => {
    const item = document.createElement("div");
    item.className = "list-item";

    item.innerHTML = `
      <div>
        <strong>${product.name}</strong>
        <div class="small text-muted">Barcode: ${product.barcode}</div>
      </div>
      <span class="badge bg-secondary">${product.shelfAmount} Left</span>
    `;

    container.appendChild(item);
  });
}


document.addEventListener("DOMContentLoaded", getLowStock);