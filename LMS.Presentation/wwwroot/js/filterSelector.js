function callApiForDataView(params, viewOption)
{
    let currentUrl = window.location.href;
    const url = new URL(currentUrl);

    const existingViewOption = url.searchParams.get(params);

    if (viewOption !== existingViewOption)
    {
        if (viewOption)
            url.searchParams.set(params, viewOption);
        else
            url.searchParams.delete(params);

        window.location.href = url.toString();
    }
}

function restoreDataViewSelectorInUI(filterSelector)
{
    let params = $(filterSelector).data("params");
    let currentUrl = new URL(window.location.href);
    let activeDataParam = currentUrl.searchParams.get(params);

    let fallbackValue = $(filterSelector).attr("value") || "";

    let finalValue = (activeDataParam !== null) ? activeDataParam : fallbackValue;

    $(filterSelector).val(finalValue).trigger('change');
}

$(document).ready(function ()
{
    $('.filter-options').each(function ()
    {
        restoreDataViewSelectorInUI($(this));
    });

    $('.filter-options').off("change").on("change", function ()
    {
        let selectedValue = $(this).val();
        let params = $(this).data("params");

        let viewOption = selectedValue === "" ? null : selectedValue;
        callApiForDataView(params, viewOption);
    });
});