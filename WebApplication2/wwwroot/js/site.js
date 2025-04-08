// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.getElementById("registerForm").addEventListener("submit", (e) => {
    e.preventDefault(); // Prevent actual form submission

    const isRegistrationSuccessful = true; // Simulate success
    if (isRegistrationSuccessful) {
        const popup = document.getElementById("successPopup");
        popup.classList.remove("hidden");

        // Hide the pop-up automatically after 3 seconds
        setTimeout(() => {
            popup.classList.add("hidden");
        }, 3000);
    }
});
