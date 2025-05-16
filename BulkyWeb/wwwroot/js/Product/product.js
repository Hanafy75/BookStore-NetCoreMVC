$(document).ready(function () {
    // Initialize delete modal handlers
    function initializeDeleteHandlers() {

        $(document).off('click', '.delete-btn').on('click', '.delete-btn', function (e) {
            e.preventDefault();

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

    // Handle pagination clicks
    $(document).on('click', '.page-link', function (e) {
        e.preventDefault();

        const pageIndex = $(this).data('page');
        const pageSize = $(this).data('size');

        if (!pageIndex || !pageSize || pageIndex < 1 || pageSize < 1) {
            console.error('Invalid pageIndex or pageSize:', { pageIndex, pageSize });
            alert('Invalid pagination parameters. Please refresh the page.');
            return;
        }

        $('#loadingSpinner').removeClass('d-none');

        // Use server-generated URL
        const url = window.appSettings.getProductsUrl + `?pageIndex=${pageIndex}&pageSize=${pageSize}`;

        $.ajax({
            url: url,
            type: 'GET',
            headers: { 'Accept': 'text/html' },
            success: function (data) {
                $('#productContainer').html(data);
                initializeDeleteHandlers();
                $('#loadingSpinner').addClass('d-none');
            },
            error: function (xhr, status, error) {
                alert(`Error loading page (Status ${xhr.status}): ${xhr.responseText || 'Could not find the requested resource. Please check the URL or try again.'}`);
                $('#loadingSpinner').addClass('d-none');
            }
        });
    });
});