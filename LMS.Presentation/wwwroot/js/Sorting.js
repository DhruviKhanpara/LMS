function callApiForSorting(sortStates)
{
    let currentUrl = window.location.href;
    const url = new URL(currentUrl);

    const orderBy = Object.entries(sortStates)
        .map(([field, state]) => `${field} ${state === 1 ? 'asc' : 'desc'}`)
        .join(',');

    const existingOrderBy = url.searchParams.get("orderBy");

    if (orderBy !== existingOrderBy)
    {
        if (orderBy)
            url.searchParams.set("orderBy", orderBy);
        else
            url.searchParams.delete("orderBy");

        window.location.href = url.toString();
    }
}

function addOrderByIcon()
{
    $(".table th.sortable-header").each(function ()
    {
        $(this).append(`
            <i class="bi bi-sort-up" style="display: none;"></i>
            <i class="bi bi-sort-down" style="display: none;"></i>
        `);
    });
}

function parseOrderByFromUrl()
{
    const params = new URLSearchParams(window.location.search);
    const orderBy = params.get("orderBy");
    let sortStates = {};

    if (orderBy)
    {
        orderBy.split(",").forEach(part =>
        {
            const [field, direction] = part.trim().split(" ");
            const validDirection = direction ?? "asc";

            if (field && ["asc", "desc"].includes(validDirection))
            {
                sortStates[field] = validDirection === "asc" ? 1 : 2;
            }
        });
    }

    return sortStates;
}

function restoreSortingUI(sortStates)
{
    Object.entries(sortStates).forEach(([field, state]) =>
    {
        const $header = $(`.table th.sortable-header[data-field='${field}']`);
        if ($header.length)
        {
            setSortIcons($header, state);
        }
    });
}

function setSortIcons($header, state)
{
    const $sortUpIcon = $header.find(".bi-sort-up");
    const $sortDownIcon = $header.find(".bi-sort-down");

    if (state === 1)
    {
        $sortUpIcon.show();
        $sortDownIcon.hide();
    }
    else if (state === 2)
    {
        $sortUpIcon.hide();
        $sortDownIcon.show();
    }
    else
    {
        $sortUpIcon.hide();
        $sortDownIcon.hide();
    }
}

function handleSortableHeaderClick(sortStates)
{
    $(".table").off("click", ".sortable-header").on("click", ".sortable-header", function ()
    {
        const $header = $(this);
        const fieldName = $header.data("field");

        if (!sortStates[fieldName]) sortStates[fieldName] = 0;

        if (sortStates[fieldName] === 0)
            sortStates[fieldName] = 1; // Ascending
        else if (sortStates[fieldName] === 1)
            sortStates[fieldName] = 2; // Descending
        else
            delete sortStates[fieldName]; // Reset sorting

        setSortIcons($header, sortStates[fieldName] || 0);

        callApiForSorting(sortStates);
    });
}

$(document).ready(function ()
{
    addOrderByIcon();

    let sortStates = parseOrderByFromUrl();
    restoreSortingUI(sortStates);

    handleSortableHeaderClick(sortStates);
});