// MicroFinance Application JavaScript
$(document).ready(function() {
    // Initialize tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Form validation enhancement
    $('form').on('submit', function(e) {
        var form = $(this);
        var isValid = true;

        // Clear previous validation
        form.find('.is-invalid').removeClass('is-invalid');
        form.find('.invalid-feedback').remove();

        // Basic validation for required fields
        form.find('[required]').each(function() {
            var field = $(this);
            if (!field.val().trim()) {
                field.addClass('is-invalid');
                field.after('<div class="invalid-feedback">This field is required.</div>');
                isValid = false;
            }
        });

        // Email validation
        form.find('input[type="email"]').each(function() {
            var field = $(this);
            var email = field.val().trim();
            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (email && !emailRegex.test(email)) {
                field.addClass('is-invalid');
                field.after('<div class="invalid-feedback">Please enter a valid email address.</div>');
                isValid = false;
            }
        });

        // Phone validation
        form.find('input[type="tel"]').each(function() {
            var field = $(this);
            var phone = field.val().trim();
            var phoneRegex = /^[\d\s\-\+\(\)]+$/;
            if (phone && (!phoneRegex.test(phone) || phone.length < 10)) {
                field.addClass('is-invalid');
                field.after('<div class="invalid-feedback">Please enter a valid phone number.</div>');
                isValid = false;
            }
        });

        if (!isValid) {
            e.preventDefault();
            // Scroll to first error
            var firstError = form.find('.is-invalid').first();
            if (firstError.length) {
                $('html, body').animate({
                    scrollTop: firstError.offset().top - 100
                }, 500);
            }
        }
    });

    // Enhanced table features
    $('.table').each(function() {
        var table = $(this);
        
        // Add sorting functionality to table headers
        table.find('thead th').each(function(index) {
            var header = $(this);
            if (header.text().trim() && !header.hasClass('no-sort')) {
                header.addClass('sortable');
                header.css('cursor', 'pointer');
                header.append(' <i class="fas fa-sort text-muted"></i>');
                
                header.on('click', function() {
                    sortTable(table, index);
                });
            }
        });
    });

    // Modal enhancement
    $('.modal').on('show.bs.modal', function() {
        $('body').addClass('modal-open');
    }).on('hidden.bs.modal', function() {
        $('body').removeClass('modal-open');
    });

    // Auto-close alerts
    $('.alert').each(function() {
        var alert = $(this);
        if (alert.hasClass('auto-dismiss')) {
            setTimeout(function() {
                alert.fadeOut();
            }, 5000);
        }
    });

    // Number formatting for currency fields
    $('.currency').on('input', function() {
        formatCurrency($(this));
    });

    // Date picker enhancement
    $('input[type="date"]').each(function() {
        var input = $(this);
        if (!input.val()) {
            input.addClass('text-muted');
        }
    }).on('change', function() {
        $(this).removeClass('text-muted');
    });
});

// Table sorting function
function sortTable(table, columnIndex) {
    var rows = table.find('tbody tr').toArray();
    var isNumeric = true;
    var isDate = true;
    
    // Detect data type
    table.find('tbody tr').slice(0, 3).each(function() {
        var cellText = $(this).find('td').eq(columnIndex).text().trim();
        if (cellText) {
            if (isNaN(cellText.replace(/[,$\s]/g, ''))) {
                isNumeric = false;
            }
            if (isNaN(Date.parse(cellText))) {
                isDate = false;
            }
        }
    });
    
    rows.sort(function(a, b) {
        var aText = $(a).find('td').eq(columnIndex).text().trim();
        var bText = $(b).find('td').eq(columnIndex).text().trim();
        
        if (isNumeric) {
            return parseFloat(aText.replace(/[,$\s]/g, '')) - parseFloat(bText.replace(/[,$\s]/g, ''));
        } else if (isDate) {
            return new Date(aText) - new Date(bText);
        } else {
            return aText.localeCompare(bText);
        }
    });
    
    table.find('tbody').empty().append(rows);
    
    // Update sort icons
    table.find('thead th .fas').removeClass('fa-sort-up fa-sort-down').addClass('fa-sort');
    table.find('thead th').eq(columnIndex).find('.fas')
        .removeClass('fa-sort').addClass('fa-sort-up');
}

// Currency formatting function
function formatCurrency(input) {
    var value = input.val().replace(/[^\d.]/g, '');
    if (value) {
        var formatted = parseFloat(value).toLocaleString('en-IN', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
        input.val('₹' + formatted);
    }
}

// Show loading spinner
function showLoading(button) {
    if (button) {
        button.prop('disabled', true);
        button.html('<span class="spinner-border spinner-border-sm me-2" role="status"></span>Loading...');
    }
}

// Hide loading spinner
function hideLoading(button, originalText) {
    if (button) {
        button.prop('disabled', false);
        button.html(originalText || 'Submit');
    }
}

// Show toast notification
function showToast(message, type) {
    type = type || 'info';
    var bgClass = 'bg-' + (type === 'error' ? 'danger' : type);
    
    var toast = $(`
        <div class="toast align-items-center text-white ${bgClass} border-0 position-fixed" 
             style="top: 20px; right: 20px; z-index: 9999;" role="alert">
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" 
                        data-bs-dismiss="toast"></button>
            </div>
        </div>
    `);
    
    $('body').append(toast);
    var bsToast = new bootstrap.Toast(toast[0]);
    bsToast.show();
    
    toast.on('hidden.bs.toast', function() {
        toast.remove();
    });
}

// Confirmation dialog
function confirmAction(message, callback) {
    if (confirm(message)) {
        if (typeof callback === 'function') {
            callback();
        }
        return true;
    }
    return false;
}

// Print function
function printElement(elementId) {
    var printContents = document.getElementById(elementId);
    if (printContents) {
        var originalContents = document.body.innerHTML;
        document.body.innerHTML = printContents.innerHTML;
        window.print();
        document.body.innerHTML = originalContents;
        location.reload();
    }
}

// Export table to CSV
function exportTableToCSV(tableId, filename) {
    var csv = [];
    var table = document.getElementById(tableId);
    var rows = table.querySelectorAll('tr');
    
    for (var i = 0; i < rows.length; i++) {
        var row = [];
        var cols = rows[i].querySelectorAll('td, th');
        
        for (var j = 0; j < cols.length; j++) {
            row.push(cols[j].innerText.replace(/"/g, '""'));
        }
        csv.push('"' + row.join('","') + '"');
    }
    
    var csvFile = new Blob([csv.join('\n')], { type: 'text/csv' });
    var downloadLink = document.createElement('a');
    downloadLink.download = filename || 'export.csv';
    downloadLink.href = window.URL.createObjectURL(csvFile);
    downloadLink.style.display = 'none';
    document.body.appendChild(downloadLink);
    downloadLink.click();
    document.body.removeChild(downloadLink);
}

// Form field highlighting
$(document).on('focus', '.form-control', function() {
    $(this).closest('.form-group, .mb-3').addClass('focused');
}).on('blur', '.form-control', function() {
    $(this).closest('.form-group, .mb-3').removeClass('focused');
});