// MicroFinance Application JavaScript
$(document).ready(function() {
    // Hide View buttons from all grids
    hideViewButtons();
    
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

    // Apply uniform date format across all date inputs
    setUniformDateFormat();

    // Convert all date inputs to use dd-MM-yyyy format
    $('input[type="date"]').each(function() {
        var input = $(this);
        
        // Remove HTML5 date type and convert to text input with datepicker
        input.attr('type', 'text');
        input.addClass('date-input');
        
        // Convert existing value to dd-MM-yyyy format
        var currentValue = input.val();
        if (currentValue) {
            var formattedDate = formatDateToDDMMYYYY(currentValue);
            input.val(formattedDate);
        }
        
        // Set placeholder to show expected format
        input.attr('placeholder', 'DD-MM-YYYY');
        
        if (!input.val()) {
            input.addClass('text-muted');
        }
    }).on('change blur', function() {
        $(this).removeClass('text-muted');
        
        // Validate and format the entered date
        var input = $(this);
        var value = input.val();
        if (value) {
            var formattedDate = formatDateToDDMMYYYY(value);
            input.val(formattedDate);
        }
    });

    // Initialize jQuery UI datepicker for all date inputs
    if ($.fn.datepicker) {
        // Set global default date format for jQuery UI datepicker
        $.datepicker.setDefaults({
            dateFormat: 'dd-mm-yy',
            changeYear: true,
            changeMonth: true,
            yearRange: '-100:+10',
            showButtonPanel: true,
            constrainInput: true
        });
        
        // Apply datepicker to all date-related inputs
        $('input.date-input, input[type="text"]').each(function() {
            var input = $(this);
            var placeholder = input.attr('placeholder') || '';
            var className = input.attr('class') || '';
            var inputId = input.attr('id') || '';
            
            if (placeholder.toLowerCase().includes('date') || 
                className.toLowerCase().includes('date') ||
                inputId.toLowerCase().includes('date') ||
                input.hasClass('date-input')) {
                
                input.datepicker({
                    dateFormat: 'dd-mm-yy',
                    changeYear: true,
                    changeMonth: true,
                    yearRange: '-100:+10',
                    showButtonPanel: true,
                    constrainInput: true,
                    onSelect: function(dateText) {
                        $(this).val(dateText).trigger('change');
                    }
                });
            }
        });
    }
    
    // Convert date values in forms before submission to ensure consistent format
    $('form').on('submit', function() {
        var form = $(this);
        
        // Find all date inputs and ensure they send dd-MM-yyyy format
        form.find('input[type="date"], input.hasDatepicker').each(function() {
            var input = $(this);
            var value = input.val();
            
            if (value) {
                // If it's in YYYY-MM-DD format (HTML5 date input default), convert to dd-MM-yyyy
                if (value.match(/^\d{4}-\d{2}-\d{2}$/)) {
                    var parts = value.split('-');
                    var newValue = parts[2] + '-' + parts[1] + '-' + parts[0];
                    input.val(newValue);
                }
                // If it's already in dd-MM-yyyy or dd/MM/yyyy, standardize to dd-MM-yyyy
                else if (value.match(/^\d{2}[\/\-]\d{2}[\/\-]\d{4}$/)) {
                    var standardizedValue = value.replace(/\//g, '-');
                    input.val(standardizedValue);
                }
            }
        });
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

// Date formatting utilities
function formatDateToDDMMYYYY(dateString) {
    if (!dateString) return '';
    
    // Handle various input formats and convert to dd-MM-yyyy
    var date;
    
    // YYYY-MM-DD format (HTML5 date input)
    if (dateString.match(/^\d{4}-\d{2}-\d{2}$/)) {
        var parts = dateString.split('-');
        return parts[2] + '-' + parts[1] + '-' + parts[0];
    }
    // MM/DD/YYYY format (US format) - convert to DD-MM-YYYY
    else if (dateString.match(/^\d{1,2}\/\d{1,2}\/\d{4}$/)) {
        var parts = dateString.split('/');
        var month = parts[0].padStart(2, '0');
        var day = parts[1].padStart(2, '0');
        var year = parts[2];
        return day + '-' + month + '-' + year;
    }
    // DD/MM/YYYY format (European) - convert to DD-MM-YYYY
    else if (dateString.match(/^\d{1,2}\/\d{1,2}\/\d{4}$/)) {
        var parts = dateString.split('/');
        var day = parts[0].padStart(2, '0');
        var month = parts[1].padStart(2, '0');
        var year = parts[2];
        return day + '-' + month + '-' + year;
    }
    // DD-MM-YYYY format - already correct
    else if (dateString.match(/^\d{1,2}-\d{1,2}-\d{4}$/)) {
        var parts = dateString.split('-');
        var day = parts[0].padStart(2, '0');
        var month = parts[1].padStart(2, '0');
        var year = parts[2];
        return day + '-' + month + '-' + year;
    }
    // Try to parse as date object and format properly
    else {
        try {
            // Assume MM/DD/YYYY if it's a US formatted date string
            if (dateString.match(/^\d{1,2}\/\d{1,2}\/\d{4}$/)) {
                var parts = dateString.split('/');
                date = new Date(parts[2], parts[0] - 1, parts[1]); // Year, Month (0-indexed), Day
            } else {
                date = new Date(dateString);
            }
            
            if (!isNaN(date.getTime())) {
                var day = date.getDate().toString().padStart(2, '0');
                var month = (date.getMonth() + 1).toString().padStart(2, '0');
                var year = date.getFullYear();
                return day + '-' + month + '-' + year;
            }
        } catch (e) {
            // If parsing fails, return original
        }
    }
    
    return dateString; // Return original if no pattern matches
}

function formatDateToYYYYMMDD(dateString) {
    if (!dateString) return '';
    
    // Handle dd-MM-yyyy format and convert to yyyy-MM-dd for HTML5 date inputs
    if (dateString.match(/^\d{2}-\d{2}-\d{4}$/)) {
        var parts = dateString.split('-');
        return parts[2] + '-' + parts[1] + '-' + parts[0];
    }
    // Handle dd/MM/yyyy format
    else if (dateString.match(/^\d{2}\/\d{2}\/\d{4}$/)) {
        var parts = dateString.split('/');
        return parts[2] + '-' + parts[1] + '-' + parts[0];
    }
    
    return dateString;
}

// Global date format setter for all date inputs
function setUniformDateFormat() {
    // Convert all date inputs to use dd-MM-yyyy format
    $('input[type="date"], input.hasDatepicker, input[placeholder*="date" i], input[id*="date" i], input.date-input').each(function() {
        var input = $(this);
        var currentValue = input.val();
        
        // Convert HTML5 date inputs to text inputs
        if (input.attr('type') === 'date') {
            input.attr('type', 'text');
            input.addClass('date-input');
            input.attr('placeholder', 'DD-MM-YYYY');
        }
        
        if (currentValue) {
            var formattedValue = formatDateToDDMMYYYY(currentValue);
            input.val(formattedValue);
        }
        
        // Add a data attribute to track the format
        input.attr('data-date-format', 'dd-MM-yyyy');
        
        // Apply datepicker if jQuery UI is available
        if ($.fn.datepicker && !input.hasClass('hasDatepicker')) {
            input.datepicker({
                dateFormat: 'dd-mm-yy',
                changeYear: true,
                changeMonth: true,
                yearRange: '-100:+10',
                showButtonPanel: true,
                constrainInput: true
            });
        }
    });
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

// Dynamic date format enforcement for dynamically added elements
$(document).on('focus', 'input[type="date"], input.hasDatepicker, input.date-input', function() {
    var input = $(this);
    
    // Convert HTML5 date input to text input with datepicker
    if (input.attr('type') === 'date') {
        var currentValue = input.val();
        input.attr('type', 'text');
        input.addClass('date-input');
        input.attr('placeholder', 'DD-MM-YYYY');
        
        if (currentValue) {
            var formattedValue = formatDateToDDMMYYYY(currentValue);
            input.val(formattedValue);
        }
    }
    
    if (!input.attr('data-date-format')) {
        input.attr('data-date-format', 'dd-MM-yyyy');
        
        // Apply datepicker if jQuery UI is available
        if ($.fn.datepicker && !input.hasClass('hasDatepicker')) {
            input.datepicker({
                dateFormat: 'dd-mm-yy',
                changeYear: true,
                changeMonth: true,
                yearRange: '-100:+10',
                showButtonPanel: true,
                constrainInput: true,
                onSelect: function(dateText) {
                    $(this).val(dateText).trigger('change');
                }
            });
        }
    }
});

// Global function to apply date format to any new elements
window.applyDateFormatting = function(container) {
    var $container = container ? $(container) : $(document);
    
    $container.find('input[type="date"], input[placeholder*="date" i], input[id*="date" i], input.date-input').each(function() {
        var input = $(this);
        var currentValue = input.val();
        
        // Convert HTML5 date input to text input
        if (input.attr('type') === 'date') {
            input.attr('type', 'text');
            input.addClass('date-input');
            input.attr('placeholder', 'DD-MM-YYYY');
        }
        
        if (currentValue) {
            var formattedValue = formatDateToDDMMYYYY(currentValue);
            input.val(formattedValue);
        }
        
        input.attr('data-date-format', 'dd-MM-yyyy');
        
        // Apply datepicker if available and not already applied
        if ($.fn.datepicker && !input.hasClass('hasDatepicker')) {
            input.datepicker({
                dateFormat: 'dd-mm-yy',
                changeYear: true,
                changeMonth: true,
                yearRange: '-100:+10',
                showButtonPanel: true,
                constrainInput: true,
                onSelect: function(dateText) {
                    $(this).val(dateText).trigger('change');
                }
            });
        }
    });
};

// Function to hide View buttons from all grids
function hideViewButtons() {
    // Use multiple selectors to catch different button patterns
    var viewButtonSelectors = [
        'button[title="View"]',
        'button[title="View Details"]', 
        'button[onclick*="viewStaff"]',
        'button[onclick*="viewBranch"]',
        'button[onclick*="viewGroup"]',
        'button[onclick*="viewMember"]',
        'button[onclick*="viewLoan"]',
        'button:has(.fa-eye)',
        '.btn:has(.fa-eye)'
    ];
    
    // Hide buttons using each selector
    viewButtonSelectors.forEach(function(selector) {
        try {
            $(selector).hide();
        } catch(e) {
            // Ignore errors from unsupported selectors like :has() in older browsers
        }
    });
    
    // Additional fallback - find buttons with fa-eye icon
    $('.fa-eye').closest('button').hide();
    $('.fa-eye').parent('button').hide();
    
    // Log for debugging
    console.log('View buttons hidden from grids');
    
    // Re-run after a short delay to catch dynamically loaded content
    setTimeout(function() {
        $('.fa-eye').closest('button').hide();
        $('.fa-eye').parent('button').hide();
        console.log('View buttons hidden (delayed execution)');
    }, 500);
}