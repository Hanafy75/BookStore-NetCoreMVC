$(document).ready(function () {
    // Initialize delete modal handlers
    function initializeDeleteHandlers() {
        console.log('Initializing delete handlers');

        $(document).off('click', '.delete-btn').on('click', '.delete-btn', function (e) {
            e.preventDefault();
            console.log('Delete button clicked');

            const $button = $(this);
            const $form = $button.closest('form');
            const productName = $button.data('product-name');

            if (!$form.length) {
                console.error('Form not found for delete button');
                return;
            }

            $('#confirmDeleteBtn').data('form', $form);
            $('#productNameToDelete').text(productName || 'Unknown');
            $('#deleteConfirmationModal').modal('show');
        });

        $(document).off('click', '#confirmDeleteBtn').on('click', '#confirmDeleteBtn', function () {
            console.log('Confirm delete button clicked');

            const $form = $(this).data('form');
            if ($form && $form.length) {
                $form.submit();
            } else {
                console.error('No form to submit for deletion');
            }

            $('#deleteConfirmationModal').modal('hide');
        });
    }

    // Initialize on page load
    initializeDeleteHandlers();

    // Debounce function to limit rapid calls
    function debounce(func, wait) {
        let timeout;
        return function (...args) {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this, args), wait);
        };
    }

    // Function to load products
    function loadProducts(pageIndex, pageSize, searchTerm) {
        $('#loadingSpinner').removeClass('d-none');

        let url = window.appSettings.getProductsUrl + `?pageIndex=${pageIndex}&pageSize=${pageSize}`;
        if (searchTerm) {
            url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
        }
        console.log('Sending AJAX request to:', url);

        $.ajax({
            url: url,
            type: 'GET',
            headers: { 'Accept': 'text/html' },
            success: function (data) {
                console.log('AJAX success, updating product container');
                $('#productContainer').html(data);
                initializeDeleteHandlers();
                $('#loadingSpinner').addClass('d-none');
            },
            error: function (xhr, status, error) {
                console.error('AJAX error:', { status: xhr.status, response: xhr.responseText, error, url });
                alert(`Error loading page (Status ${xhr.status}): ${xhr.responseText || 'Could not find the requested resource. Please check the URL or try again.'}`);
                $('#loadingSpinner').addClass('d-none');
            }
        });
    }

    // Handle pagination clicks
    $(document).on('click', '.page-link', function (e) {
        e.preventDefault();
        console.log('Pagination link clicked');

        const pageIndex = $(this).data('page');
        const pageSize = $(this).data('size');
        const searchTerm = $(this).data('search') || $('#searchInput').val();

        if (!pageIndex || !pageSize || pageIndex < 1 || pageSize < 1) {
            console.error('Invalid pageIndex or pageSize:', { pageIndex, pageSize });
            alert('Invalid pagination parameters. Please refresh the page.');
            return;
        }

        loadProducts(pageIndex, pageSize, searchTerm);
    });

    // Handle search input with debounce
    $(document).on('input', '#searchInput', debounce(function () {
        console.log('Search input changed');
        const searchTerm = $('#searchInput').val().trim();
        loadProducts(1, 10, searchTerm);
    }, 300));
});