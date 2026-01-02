document.addEventListener("click", function (e) {
    const link = e.target.closest("#nav-links .nav-link");
    if (!link) return;

    const navLinks = document.querySelectorAll("#nav-links .nav-link");
    const contactBox = document.querySelector(".contact");
    navLinks.forEach(l => l.classList.remove("active"));
    contactBox?.classList.remove("active-contact");
    
    // Contact
    if (link.parentElement.parentElement.classList.contains("contact")) {
        contactBox?.classList.add("active-contact");
    } else {
        link.classList.add("active");
    }
});
