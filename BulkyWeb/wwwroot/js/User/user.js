$(document).ready(function () {
    // Debounce function to limit rapid calls
    function debounce(func, wait) {
        let timeout;
        return function (...args) {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this, args), wait);
        };
    }

    // Function to load users
    function loadUsers(pageIndex, pageSize, searchTerm) {
        $('#loadingSpinner').removeClass('d-none');
        let url = window.appSettings.getUsersUrl + `?pageIndex=${pageIndex}&pageSize=${pageSize}`;
        if (searchTerm) {
            url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
        }
        console.log('Sending AJAX request to:', url);
        $.ajax({
            url: url,
            type: 'GET',
            headers: { 'Accept': 'text/html' },
            success: function (data) {
                console.log('AJAX success, updating user container');
                $('#userContainer').html(data);
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
        loadUsers(pageIndex, pageSize, searchTerm);
    });

    // Handle search input with debounce
    $(document).on('input', '#searchInput', debounce(function () {
        console.log('Search input changed');
        const searchTerm = $('#searchInput').val().trim();
        loadUsers(1, 10, searchTerm);
    }, 300));

    // Handle lock/unlock button clicks
    $(document).on('click', '.lock-unlock-btn', function (e) {
        e.preventDefault();
        const userId = $(this).data('user-id');
        const action = $(this).data('action');
        const button = $(this);

        // Disable button to prevent multiple clicks
        button.prop('disabled', true);

        // Send AJAX request to lock/unlock user
        $.ajax({
            url: window.appSettings.lockUnlockUrl,
            type: 'POST',
            data: {
                id: userId,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (data) {
                console.log('Lock/Unlock successful');
                // Reload the current page with current search term
                const currentPageIndex = $('.page-item.active .page-link').data('page') || 1;
                const currentPageSize = $('.page-item.active .page-link').data('size') || 10;
                const currentSearchTerm = $('#searchInput').val().trim();
                loadUsers(currentPageIndex, currentPageSize, currentSearchTerm);
            },
            error: function (xhr, status, error) {
                console.error('Lock/Unlock error:', { status: xhr.status, response: xhr.responseText, error });
                alert(`Error ${action}ing user (Status ${xhr.status}): ${xhr.responseText || 'An error occurred. Please try again.'}`);
                button.prop('disabled', false);
            }
        });
    });
});