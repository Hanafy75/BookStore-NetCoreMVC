// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Initialize and show all toasts
    var toastElList = [].slice.call(document.querySelectorAll('.toast'));
    var toastList = toastElList.map(function (toastEl) {
        var toast = new bootstrap.Toast(toastEl, {
            autohide: true,
            delay: 5000 // Auto-hide after 5 seconds
        });
        toast.show();
        return toast;
    });
});