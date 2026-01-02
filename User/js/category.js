
// Fetch and display drugs safely
(async () => {
  try {
    const babycontainer = document.getElementById("baby-container");
    const drugscontainer = document.getElementById("drugs-container");
    const productscontainer = document.getElementById("products-container");

    if (babycontainer) babycontainer.innerHTML = "";
    if (drugscontainer) drugscontainer.innerHTML = "";
    if (productscontainer) productscontainer.innerHTML = "";

    const res = await fetch("http://pharmaproject.runasp.net/api/Drugs", {
      method: "GET",
      headers: {
        Authorization: "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJzdXBlcl9hZG1pbiIsImV4cCI6MTc2NzgwMTU5NywiaXNzIjoiUGhhcm1hY3lBcGkiLCJhdWQiOiJQaGFybWFjeUZyb250ZW5kIn0.RZ6fScSDEacNw6ADoNk-FSXeUenSHbKFLHmMibY0bnA"
      }
    });
    const data = await res.json();
    //console.log(data);
    const babyFragment = document.createDocumentFragment();
    const drugsFragment = document.createDocumentFragment();
    const productsFragment = document.createDocumentFragment();

    data.forEach(drug => {
      const imageSrc = drug.imageUrl 
        ? (drug.imageUrl.startsWith("http") 
            ? drug.imageUrl 
            : "http://pharmaproject.runasp.net" + drug.imageUrl) 
        : "imgs/placeholder.jpg";

      const drugId = drug.drugId || drug.id;

      const div = document.createElement("div");
      div.className = "col-6 col-sm-4 col-md-3 col-lg-2 items-card";
      div.innerHTML = `
        <a href="./cardItem.html?id=${drugId}">
          <div class="card d-flex flex-column text-center align-items-center">
            <img src="${imageSrc}" class="card-img-top" alt="img">
            <div class="card-body">
              <h5 class="card-title">${drug.name}</h5>
              <p class="card-text">${drug.sellingPrice} EGP</p>
            </div>
          </div>
        </a>
      `;

      if (drug.tags && drug.tags.includes("baby") && babycontainer) {
        babyFragment.appendChild(div.cloneNode(true));
      }
      if (drug.tags && drug.tags.includes("drug") && drugscontainer) {
        drugsFragment.appendChild(div.cloneNode(true));
      }
      if (drug.tags && drug.tags.includes("product") && productscontainer) {
        productsFragment.appendChild(div.cloneNode(true));
      }
    });

    if (babycontainer) babycontainer.appendChild(babyFragment);
    if (drugscontainer) drugscontainer.appendChild(drugsFragment);
    if (productscontainer) productscontainer.appendChild(productsFragment);

  } catch (err) {
    console.error("Error fetching or displaying drugs:", err);
  }
})();
// Single JS file for all pages

document.addEventListener("DOMContentLoaded", () => {

    // Define mapping of container IDs to search input IDs
    const pages = [
        { containerId: "baby-container", searchInputId: "baby-search" },
        { containerId: "drugs-container", searchInputId: "drugs-search" },
        { containerId: "products-container", searchInputId: "products-search" }
    ];

    pages.forEach(page => {
        const container = document.getElementById(page.containerId);
        const searchInput = document.getElementById(page.searchInputId);

        // Make sure both exist on this page
        if (container && searchInput) {
            searchInput.addEventListener("keyup", (e) => {
                const query = e.target.value.toLowerCase();
                const cards = container.querySelectorAll(".items-card");

                cards.forEach(card => {
                    const title = card.querySelector(".card-title").textContent.toLowerCase();
                    card.style.display = title.includes(query) ? "" : "none";
                });
            });
        }
    });

});

