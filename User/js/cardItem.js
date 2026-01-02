const params = new URLSearchParams(window.location.search);
const drugId = params.get('id');
if (!drugId) {
    console.error("No drug ID in URL");
} else {
    const container = document.getElementById("item-container");

    if (!container) {
    console.error("Item container not found");
    } 
    else {
    fetch(`http://pharmaproject.runasp.net/api/Drugs/${drugId}`, {
        method: "GET",
        headers: {
        Authorization: "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJzdXBlcl9hZG1pbiIsImV4cCI6MTc2NzgwMTU5NywiaXNzIjoiUGhhcm1hY3lBcGkiLCJhdWQiOiJQaGFybWFjeUZyb250ZW5kIn0.RZ6fScSDEacNw6ADoNk-FSXeUenSHbKFLHmMibY0bnA"
    }
    })
    .then(res => res.json())
    .then(drug => {
        const imageSrc = drug.imageUrl
        ? (drug.imageUrl.startsWith("http")
            ? drug.imageUrl
            : "http://pharmaproject.runasp.net" + drug.imageUrl)
        : "imgs/mcdn.jpg";
        container.innerHTML += `
        <div class="item-big-card">
            <!-- Desktop Card -->
            <div class="card mb-3 d-none d-lg-flex align-items-center col-lg-9 mx-auto col-12">
                <div class="row g-0">
                    <div class="col-md-3 d-flex justify-content-center">
                        <img src="${imageSrc}" class="img-fluid rounded-start w-100 ps-4 item-big-card-img" alt="${drug.name}">
                    </div>
                    <div class="col-md-9 ps-4">
                        <div class="card-body">
                            <h5 class="card-title">${drug.name}</h5>
                            <div class="card-text">
                                <p class="description">${drug.descriptionBeforeUse}</p>
                                <p class="how-use"><strong>How to use:</strong> ${drug.descriptionHowToUse}</p>
                                <p class="effects"><strong>Side effects:</strong> ${drug.descriptionSideEffects}</p>
                            </div>
                            <p class="card-text mt-5 price">${drug.sellingPrice} EGP</p>
                        </div>
                    </div>
                </div>
            </div>
            <!-- Mobile Card -->
            <div class="card d-lg-none d-flex align-items-center">
                <img src="${imageSrc}" class="card-img-top w-25 pt-3 item-big-card-img" alt="${drug.name}">
                <div class="card-body pt-5">
                    <h5 class="card-title text-center">${drug.name}</h5>
                    <div class="card-text">
                        <p class="description">${drug.descriptionBeforeUse}</p>
                        <p class="how-use"><strong>How to use:</strong> ${drug.descriptionHowToUse}</p>
                        <p class="effects"><strong>Side effects:</strong> ${drug.descriptionSideEffects}</p>
                    </div>
                    <p class="card-text text-center price">${drug.sellingPrice} EGP</p>
                </div>
            </div>
        </div>
        `;
    })
    .catch(err => console.error("Error fetching drug:", err));}
}
