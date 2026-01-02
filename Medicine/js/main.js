const API_URL = "http://pharmaproject.runasp.net/api/drugs";
const TOKEN = localStorage.getItem("TOKEN");

// -------------------- Utility Functions --------------------
function showError(message) {
    alert(message);
}

function validateMedicineForm(isEdit = false) {
    const prefix = isEdit ? "edit" : "";
    const capitalize = (str) => isEdit ? `edit${str.charAt(0).toUpperCase() + str.slice(1)}` : str;

    const fields = {
        name: document.getElementById(capitalize("medicineName")).value.trim(),
        barcode: document.getElementById(capitalize("barcode")).value.trim(),
        manufacturer: document.getElementById(capitalize("manufacturer")).value.trim(),
        drugType: document.getElementById(capitalize("drugType")).value.trim(),
        expirationDate: document.getElementById(capitalize("expirationDate")).value,
        purchasingPrice: document.getElementById(capitalize("purchasingPrice")).value,
        sellingPrice: document.getElementById(capitalize("sellingPrice")).value,
        shelfAmount: document.getElementById(capitalize("shelfAmount")).value,
        storedAmount: document.getElementById(capitalize("storedAmount")).value,
        lowAmount: document.getElementById(capitalize("lowAmount")).value,
        subAmountQuantity: document.getElementById(capitalize("subAmountQuantity")).value
    };

    // Check for empty required fields
    if (!fields.name) return "Medicine name is required";
    if (!fields.barcode) return "Barcode is required";
    if (!fields.manufacturer) return "Manufacturer is required";
    if (!fields.drugType) return "Drug type is required";
    if (!fields.expirationDate) return "Expiration date is required";
    if (!fields.purchasingPrice) return "Purchasing price is required";
    if (!fields.sellingPrice) return "Selling price is required";
    if (!fields.shelfAmount) return "Shelf amount is required";
    if (!fields.storedAmount) return "Stored amount is required";
    if (!fields.lowAmount) return "Low amount is required";
    if (!fields.subAmountQuantity) return "Sub amount quantity is required";

    // Validate numeric fields
    if (isNaN(fields.purchasingPrice) || Number(fields.purchasingPrice) < 0) {
        return "Purchasing price must be a valid positive number";
    }
    if (isNaN(fields.sellingPrice) || Number(fields.sellingPrice) < 0) {
        return "Selling price must be a valid positive number";
    }
    if (isNaN(fields.shelfAmount) || Number(fields.shelfAmount) < 0) {
        return "Shelf amount must be a valid positive number";
    }
    if (isNaN(fields.storedAmount) || Number(fields.storedAmount) < 0) {
        return "Stored amount must be a valid positive number";
    }
    if (isNaN(fields.lowAmount) || Number(fields.lowAmount) < 0) {
        return "Low amount must be a valid positive number";
    }
    if (isNaN(fields.subAmountQuantity) || Number(fields.subAmountQuantity) < 0) {
        return "Sub amount quantity must be a valid positive number";
    }

    // Validate expiration date
    const expDate = new Date(fields.expirationDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    if (expDate < today) {
        return "Expiration date cannot be in the past";
    }

    return null; // No errors
}

// -------------------- Fetch --------------------
async function fetchMedicines() {
    try {
        const res = await fetch(API_URL, {
            headers: { Authorization: `Bearer ${TOKEN}` }
        });

        if (!res.ok) {
            throw new Error("Failed to fetch medicines");
        }

        const data = await res.json();
        renderMedicineTable(data);
    } catch (err) {
        console.error("Fetch medicines error:", err);
        showError("Failed to load medicines. Please try again.");
    }
}

// -------------------- Render Table --------------------
function renderMedicineTable(medicines) {
    const tbody = document.querySelector("table tbody");
    tbody.innerHTML = "";

    medicines.forEach(med => {
        tbody.innerHTML += `
            <tr>
                <td>${med.barcode}</td> 
                <td>${med.name}</td> 
                <td>${med.shelfAmount}</td> 
                <td class="gap-lg-3 gap-2 d-flex flex-column flex-md-row px-3"> 
                    <button class="edit-btn btn w-100 w-md-50" data-id="${med.drugId}">Edit</button> 
                    <button class="delete-btn btn w-100 w-md-50" data-id="${med.drugId}">Delete</button> 
                </td>
            </tr>
        `;
    });

    attachTableEvents();
}

// -------------------- Search --------------------
document.querySelector(".search-btn").addEventListener("click", e => {
    e.preventDefault();
    const q = document.querySelector(".search").value.toLowerCase();
    document.querySelectorAll("tbody tr").forEach(row => {
        const name = row.children[1].textContent.toLowerCase();
        const barcode = row.children[0].textContent.toLowerCase();
        row.style.display = name.includes(q) || barcode.includes(q) ? "" : "none";
    });
});

// -------------------- Add Medicine --------------------
document
    .querySelector("#addMedicineModal .btn-success")
    .addEventListener("click", async () => {

        // Validate form
        const validationError = validateMedicineForm(false);
        if (validationError) {
            showError(validationError);
            return;
        }

        // Check image
        const image = document.getElementById("medicineImage");
        if (!image.files.length) {
            showError("Medicine image is required");
            return;
        }

        // Validate image type
        const validImageTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
        if (!validImageTypes.includes(image.files[0].type)) {
            showError("Please upload a valid image file (JPG, PNG, or GIF)");
            return;
        }

        const formData = new FormData();

        formData.append("Name",
            document.getElementById("medicineName").value.trim()
        );

        formData.append("Barcode",
            document.getElementById("barcode").value.trim()
        );

        formData.append("Manufacturer",
            document.getElementById("manufacturer").value.trim()
        );

        formData.append("DrugType",
            document.getElementById("drugType").value.trim()
        );

        formData.append("ExpirationDate",
            document.getElementById("expirationDate").value
        );

        formData.append("PurchasingPrice",
            document.getElementById("purchasingPrice").value
        );

        formData.append("SellingPrice",
            document.getElementById("sellingPrice").value
        );

        formData.append(
            "RequiresPrescription",
            document.getElementById("requiresPrescription").checked
        );

        formData.append("ShelfAmount",
            document.getElementById("shelfAmount").value
        );

        formData.append("StoredAmount",
            document.getElementById("storedAmount").value
        );

        formData.append("LowAmount",
            document.getElementById("lowAmount").value
        );

        formData.append("SubAmountQuantity",
            document.getElementById("subAmountQuantity").value
        );

        formData.append("DescriptionBeforeUse",
            document.getElementById("descriptionBeforeUse").value
        );

        formData.append("DescriptionHowToUse",
            document.getElementById("descriptionHowToUse").value
        );

        formData.append("DescriptionSideEffects",
            document.getElementById("descriptionSideEffects").value
        );

        formData.append("Image", image.files[0]);

        try {
            const res = await fetch(API_URL, {
                method: "POST",
                headers: { Authorization: `Bearer ${TOKEN}` },
                body: formData
            });

            if (!res.ok) {
                const errorText = await res.text();
                throw new Error(errorText || "Failed to add medicine");
            }

            bootstrap.Modal.getInstance(
                document.getElementById("addMedicineModal")
            ).hide();

            // Clear form
            document.getElementById("addMedicineModal").querySelector("form")?.reset();

            showError("Medicine added successfully!");
            fetchMedicines();
        } catch (err) {
            console.error("ADD MEDICINE ERROR:", err);
            showError("Failed to add medicine. Please check all fields and try again.");
        }
    });

// -------------------- Edit + Delete --------------------
function attachTableEvents() {

    // -------- Edit --------
    document.querySelectorAll(".edit-btn").forEach(btn => {
        btn.onclick = async () => {
            const id = btn.dataset.id;

            try {
                const res = await fetch(`${API_URL}/${id}`, {
                    headers: { Authorization: `Bearer ${TOKEN}` }
                });

                if (!res.ok) {
                    showError("Failed to fetch medicine details");
                    return;
                }

                const med = await res.json();
                console.log("EDIT MED:", med);

                // -------- Fill modal inputs --------
                document.getElementById("editMedicineName").value = med.name || "";
                document.getElementById("editBarcode").value = med.barcode || "";
                document.getElementById("editManufacturer").value = med.manufacturer || "";
                document.getElementById("editDrugType").value = med.drugType;

                document.getElementById("editExpirationDate").value =
                    med.expirationDate ? med.expirationDate.split("T")[0] : "";

                document.getElementById("editPurchasingPrice").value = med.purchasingPrice ?? "";
                document.getElementById("editSellingPrice").value = med.sellingPrice ?? "";

                document.getElementById("editRequiresPrescription").checked =
                    !!med.requiresPrescription;

                document.getElementById("editShelfAmount").value = med.shelfAmount ?? "";
                document.getElementById("editStoredAmount").value = med.storedAmount ?? "";
                document.getElementById("editLowAmount").value = med.lowAmount ?? "";
                document.getElementById("editSubAmountQuantity").value =
                    med.subAmountQuantity ?? "";

                document.getElementById("editDescriptionBeforeUse").value =
                    med.descriptionBeforeUse || "";

                document.getElementById("editDescriptionHowToUse").value =
                    med.descriptionHowToUse || "";

                document.getElementById("editDescriptionSideEffects").value =
                    med.descriptionSideEffects || "";

                const modalEl = document.getElementById("editMedicineModal");
                const modal = new bootstrap.Modal(modalEl);
                modal.show();

                // -------- Save changes --------
                document.querySelector("#editMedicineModal .btn-success").onclick =
                    async () => {

                        // Validate form
                        const validationError = validateMedicineForm(true);
                        if (validationError) {
                            showError(validationError);
                            return;
                        }

                        const updatedData = {
                            Name: document.getElementById("editMedicineName").value.trim(),
                            Barcode: document.getElementById("editBarcode").value.trim(),
                            Manufacturer: document.getElementById("editManufacturer").value.trim(),
                            DrugType: document.getElementById("editDrugType").value.trim(),
                            ExpirationDate:
                                document.getElementById("editExpirationDate").value,

                            PurchasingPrice: Number(
                                document.getElementById("editPurchasingPrice").value
                            ),
                            SellingPrice: Number(
                                document.getElementById("editSellingPrice").value
                            ),

                            RequiresPrescription:
                                document.getElementById("editRequiresPrescription").checked,

                            ShelfAmount: Number(
                                document.getElementById("editShelfAmount").value
                            ),
                            StoredAmount: Number(
                                document.getElementById("editStoredAmount").value
                            ),
                            LowAmount: Number(
                                document.getElementById("editLowAmount").value
                            ),
                            SubAmountQuantity: Number(
                                document.getElementById("editSubAmountQuantity").value
                            ),

                            DescriptionBeforeUse:
                                document.getElementById("editDescriptionBeforeUse").value,

                            DescriptionHowToUse:
                                document.getElementById("editDescriptionHowToUse").value,

                            DescriptionSideEffects:
                                document.getElementById("editDescriptionSideEffects").value
                        };

                        try {
                            const updateRes = await fetch(`${API_URL}/${id}`, {
                                method: "PUT",
                                headers: {
                                    Authorization: `Bearer ${TOKEN}`,
                                    "Content-Type": "application/json"
                                },
                                body: JSON.stringify(updatedData)
                            });

                            if (!updateRes.ok) {
                                const text = await updateRes.text();
                                throw new Error(text || "Failed to update medicine");
                            }

                            modal.hide();
                            showError("Medicine updated successfully!");
                            fetchMedicines();

                        } catch (err) {
                            console.error("Edit medicine error:", err);
                            showError("Failed to update medicine. Please check all fields and try again.");
                        }
                    };

            } catch (err) {
                console.error("Edit click error:", err);
                showError("Failed to load medicine details");
            }
        };
    });


    // -------- Delete --------
    document.querySelectorAll(".delete-btn").forEach(btn => {
        btn.onclick = async () => {
            const id = btn.dataset.id;
            if (!confirm("Are you sure you want to delete this medicine?")) return;

            try {
                const res = await fetch(`${API_URL}/${id}`, {
                    method: "DELETE",
                    headers: { Authorization: `Bearer ${TOKEN}` }
                });

                if (!res.ok) {
                    throw new Error("Failed to delete medicine");
                }

                showError("Medicine deleted successfully!");
                fetchMedicines();
            } catch (err) {
                console.error("Delete error:", err);
                showError("Failed to delete medicine. Please try again.");
            }
        };
    });
}

// -------------------- Init --------------------
fetchMedicines();