$(document).ready(function () {
    initializeSelect2();

    let modalBody = document.querySelector('.modal-body');

    if (modalBody)
    {
        let observer = new MutationObserver(function (mutations)
        {
            let shouldUpdate = false;

            mutations.forEach(function (mutation)
            {
                mutation.addedNodes.forEach(function (node)
                {
                    if ($(node).find(".select2-single, .select2-multi").length)
                    {
                        shouldUpdate = true;
                    }
                });
            });

            if (shouldUpdate)
            {
                initializeSelect2();
            }
        });

        observer.observe(modalBody, { childList: true, subtree: true });
    }
});

function initializeSelect2()
{
    $(".select2-multi:not(.initialized)").each(function ()
    {
        let selectElement = $(this);
        selectElement.prepend(`<option value="-1" style="display: none;">Select All</option>`);

        selectElement.select2({
            width: '100%',
            closeOnSelect: false
        });
        selectElement.on("select2:select", function (e)
        {
            var data = e.params.data.text;
            if (data == 'Select All')
            {
                selectElement.find('option[value="-1"]').prop("selected", "").attr('disabled', true);
                selectElement.find('option:not([value="-1"])').prop("selected", "selected");
                selectElement.trigger("change").select2("close");
            }
        });
        selectElement.on("select2:unselect", function (e)
        {
            selectElement.find('option[value="-1"]').prop("selected", "").removeAttr('disabled');
            var dropdown = $(this).data('select2').$dropdown;
            var optionToEnable = dropdown.find("li:contains('Select All')");
            optionToEnable.removeAttr('aria-disabled').attr('aria-selected', "false");
        });

        selectElement.addClass("initialized");
    });

    $(".select2-single:not(.initialized)").each(function ()
    {
        let selectElement = $(this);
        let parentModal = selectElement.closest('.modal');

        selectElement.select2({
            width: '100%',
            closeOnSelect: true,
            dropdownParent: parentModal.length ? parentModal : $('body')
        });

        selectElement.addClass("initialized");
    });
}

function readonlySelect2(select2selector)
{
    $(select2selector).addClass('readonly-select2');

    $(select2selector).off('select2:opening').on('select2:opening', function (event)
    {
        event.preventDefault();
    });
}

function setSelect2OptionAccessibility(select2selector, blockedValues, forDisable = true)
{
    if (typeof select2selector !== 'string' || $(select2selector).length === 0)
    {
        console.error(select2selector + ' : Invalid select2 selector: Ensure it matches existing elements.');
        return;
    }

    blockedValues = blockedValues || [];

    // Prevent selection of blocked options
    $(select2selector).on("select2:selecting", function (e)
    {
        if (blockedValues.includes(e.params.args.data.id))
        {
            if (forDisable)
                $(this).addClass("readonly-option");
            else
                $(this).removeClass("readonly-option");
        }
    });

    // Add a class to visually indicate readonly options
    $(select2selector).on("select2:open", function ()
    {
        setTimeout(() =>
        {
            $(".select2-results__option").each(function ()
            {
                let select2Id = $(this).attr("data-select2-id");

                if (select2Id && select2Id.includes("select2"))
                {
                    let optionValue = select2Id.split("-").pop();

                    if (blockedValues.includes(optionValue))
                    {
                        if (forDisable)
                            $(this).addClass("readonly-option");
                        else
                            $(this).removeClass("readonly-option");
                    }
                }
            });
        }, 100);
    });
}
