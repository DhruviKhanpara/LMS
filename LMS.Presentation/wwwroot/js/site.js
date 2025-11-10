Chart.register(ChartDataLabels);

function toggleButtonState(buttonSelector, forDisable = true, classesToAddOrRemove)
{
    try
    {
        if (typeof buttonSelector !== 'string' || $(buttonSelector).length === 0)
        {
            return;
        }

        classesToAddOrRemove = classesToAddOrRemove || "";

        const $button = $(buttonSelector);
        if ($button.length === 0)
        {
            console.warn('No elements matched by buttonSelector.');
            return;
        }

        if (forDisable)
        {
            $button
                .addClass('btn-submit-disabled')
                .addClass('btn-submit-disabled-border')
                .removeClass(classesToAddOrRemove);
        } else
        {
            $button
                .addClass(classesToAddOrRemove)
                .removeClass('btn-submit-disabled')
                .removeClass('btn-submit-disabled-border');
        }
    }
    catch (error)
    {
        console.error('An unexpected error occurred in toggleButtonState:', error);
    }
}

function setProceedButtonHref(buttonSelector, message, parameter = null)
{
    var href = $(buttonSelector).attr('href');
    parameter != null ? href += "?" + parameter : href;
    showConfirmationModal(message, href);
};

function passwordVisibilityStart(iconElement)
{
    const inputField = iconElement.closest('.password-container').querySelector('.did-floating-input');

    inputField.type = "text";
    iconElement.innerHTML = '<i class="fa fa-eye-slash"></i>';
}

function passwordVisibilityEnd(iconElement)
{
    const inputField = iconElement.closest('.password-container').querySelector('.did-floating-input');

    inputField.type = "password";
    iconElement.innerHTML = '<i class="fa fa-eye"></i>';
}

function showConfirmationModal(message, href)
{
    $('#confirmationMessage').text(message);
    $('#proceed-btn').attr('href', href);
    $('#confirmationModal').modal('show');
}

function applyNumberInputRestrictions()
{
    document.querySelectorAll('input[type="number"]').forEach(numberInput =>
    {
        numberInput.addEventListener('drop', function (event)
        {
            event.preventDefault();
        });

        numberInput.addEventListener('keydown', function (event)
        {
            const isDotRestricted = numberInput.classList.contains('restrict-dot');
            const invalidKeys = [69, 107, 109, 187, 189, 106, 111, 38, 40];
            const additionalInvalidKeys = isDotRestricted ? [190, 110] : [];
            const combinedInvalidKeys = invalidKeys.concat(additionalInvalidKeys);

            if (combinedInvalidKeys.includes(event.which || event.keyCode))
            {
                event.preventDefault();
            }
        });
    });
}
function getDateRestrictions(inputElement)
{
    const inputType = inputElement.attr('type');
    const inputValue = inputType === 'date' ? inputElement.val() + 'T00:00:00' : inputElement.val();
    const date = new Date(inputValue);

    //const maxDate = new Date($(this).attr('max') || (inputType === 'date' ? new Date().toISOString().split('T')[0] + 'T00:00:00' : new Date().toISOString()));

    const maxDate = new Date($(this).attr('max')) ;
    const minDate = new Date($(this).attr('min') || '1753-01-01T00:00:00');

    return { date, maxDate, minDate };
}

function validateDate(inputElement, disablePast)
{
    const { date, maxDate, minDate } = getDateRestrictions(inputElement);

    if (disablePast)
    {
        const currentDate = new Date();
        if (date < currentDate)
        {
            toastr.error("Please select a date greater than today's date.", 'error');
            inputElement.val('');
            return false;
        }
    } else
    {
        if (date < minDate || (maxDate !== undefined && date > maxDate))
        {
            toastr.error("Please select a proper date.", 'error');
            inputElement.val('');
            return false;
        }
    }
    return true;
}

function applyDateInputRestrictions()
{
    $('input[type="date"], input[type="datetime-local"]').blur(function ()
    {
        const disablePast = $(this).hasClass('disable-past-date');
        validateDate($(this), disablePast);
    });
}

function removeSingleDropdownValueAttr(dropdownBlock)
{
    if ($(dropdownBlock).val() !== null && $(dropdownBlock).val() !== '' && $(dropdownBlock).val() !== 0)
    {
        $(dropdownBlock).removeAttr('value');
    }
}

function truncateText($description, fullDescription, maxLength)
{
    let truncatedDescription = fullDescription;

    if (fullDescription.length > maxLength)
    {
        truncatedDescription = fullDescription.substring(0, maxLength) + "...";
        $description.text(truncatedDescription);
    } else
    {
        $description.text(fullDescription);
    }
    return truncatedDescription;
}

function initializeDescription($description, $toggleButton, fullDescription, maxLength)
{
    const truncatedDescription = truncateText($description, fullDescription, maxLength);

    if (fullDescription.length > maxLength)
    {
        let isExpanded = false;

        $toggleButton.off("click").on("click", function ()
        {
            isExpanded = !isExpanded;
            $description.text(isExpanded ? fullDescription : truncatedDescription);
            $toggleButton.text(isExpanded ? "Read Less" : "Read More");
        });
    } else
    {
        $toggleButton.hide();
    }
}

function readonlyField(fieldSelector)
{
    if (typeof fieldSelector !== 'string' || $(fieldSelector).length === 0)
    {
        console.error(fieldSelector + ' : Invalid field selector: Ensure it matches existing elements.');
        return;
    }

    //$(fieldSelector).prop("readonly", true).css("opacity", "0.6");
    $(fieldSelector).prop("readonly", true).css("opacity", "1");
    $(fieldSelector).off('click').on('click', function (event)
    {
        event.preventDefault();
    });
}

function enableField(fieldSelector)
{
    if (typeof fieldSelector !== 'string' || $(fieldSelector).length === 0)
    {
        console.error(fieldSelector + ' : Invalid field selector: Ensure it matches existing elements.');
        return;
    }

    $(fieldSelector).prop("readonly", false).css("opacity", "1").css("pointer-events", "auto");
    $(fieldSelector).off('click');
}

function formatDate(dateStr)
{
    if (!dateStr) return '-';

    const date = new Date(dateStr);

    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();

    let hours = date.getHours();
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');

    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12 || 12;
    hours = String(hours).padStart(2, '0');

    return `${day}-${month}-${year} ${hours}:${minutes}:${seconds} ${ampm}`;
}

$(document).ready(function ()
{
    applyNumberInputRestrictions();
    applyDateInputRestrictions();
    $(".truncate-description-container").each(function ()
    {
        const maxDescriptionLength = $(this).data("max-length");
        const fullDescription = $(this).text().trim() || "";
        truncateText($(this), fullDescription, maxDescriptionLength);
    });

    $('[data-bs-toggle="tooltip"]').tooltip();
});

$(document).ajaxError(function (event, jqxhr, settings, exception)
{
    switch (jqxhr.status)
    {
        case 401:
            window.location.href = '/user/login-user';
            break;
        default:
            console.log("An error occurred: " + exception);
            if (jqxhr.responseJSON && jqxhr.responseJSON.message)
            {
                toastr.error(jqxhr.responseJSON.message, "Error");
            } else
            {
                toastr.error("An unexpected error occurred.", "Error");
            }
            break;
    }
});