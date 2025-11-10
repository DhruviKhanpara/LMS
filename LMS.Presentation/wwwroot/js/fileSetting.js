var flags = {};

function showDocumentModal(fileUrl)
{
    const isDataUrl = fileUrl.startsWith("data:");
    let fileExtension = "";

    if (isDataUrl)
    {
        const mime = fileUrl.split(";")[0].split(":")[1];
        fileExtension = mime.split("/").pop().toLowerCase();
    } else
    {
        fileExtension = fileUrl.split('.').pop().toLowerCase();
    }

    var modalBody = document.querySelector("#transparentModal .modal-body");

    modalBody.innerHTML = "";

    if (["jpg", "jpeg", "png", "gif", "bmp"].includes(fileExtension))
    {
        modalBody.innerHTML = `<img id='modalImage' class='img-fluid rounded' src='${fileUrl}' />`;

        $('#transparentModal').modal('show');
    }
    else if (fileExtension === "pdf")
    {
        modalBody.innerHTML = `<iframe id='modalPdfViewer' frameborder='0' class='w-100 h-100' src='${fileUrl}#zoom=100'></iframe>`;

        $('#transparentModal').modal('show');
    }

}

function handleFileInputChange(supportedTypes, maxSizeInMB)
{
    $("input[type='file']").off("change").on("change", function (event)
    {
        const flagName = $(this).data("flag");
        if (flagName)
            flags[flagName] = true;

        let previewContainer = $(this).closest(".col-md").find(".file-preview");
        previewContainer.empty();

        handleFileInputDisplay(event.target, previewContainer, supportedTypes, maxSizeInMB);
    });
}

function handleFileInputDisplay(input, previewContainer, supportedTypes, maxSizeInMB)
{
    var file = input.files[0];
    
    if (file)
    {
        const reader = new FileReader();
        reader.onload = function (e)
        {
            var previewContent = "";

            if (file.type.includes("pdf"))
            {
                if (file.size > maxSizeInMB * 1024 * 1024)
                {
                    previewContent = `
                            <div class="file-container d-flex gap-2">
                                <div class="image-preview position-relative">
                                    <div class="img-thumbnail">
                                        <i class="bi bi-file-earmark-pdf-fill fs-1"></i>
                                    </div>
                                    <span class="remove-file img-overlay"><i class="bi bi-x-lg"></i></span>
                                </div>
                                <small class="text-muted align-content-center">Preview disabled (file too large)</small>
                            </div>`;
                } else
                {
                    previewContent = `
                            <div class="file-container d-flex gap-2">
                                <div class="image-preview position-relative">
                                    <div class="img-thumbnail" onclick="showDocumentModal('${e.target.result}')">
                                        <i class="bi bi-file-earmark-pdf-fill fs-1"></i>
                                    </div>
                                    <span class="remove-file img-overlay"><i class="bi bi-x-lg"></i></span>
                                </div>
                            </div>`;
                }
            } else if (supportedTypes.includes(file.type))
            {
                previewContent = `
                        <div class="file-container d-flex gap-2">
                            <div class="image-preview position-relative">
                                <img src="${e.target.result}" class="img-thumbnail" width="100" alt="Preview" onclick="showDocumentModal('${e.target.result}')" />
                                <span class="remove-file img-overlay"><i class="bi bi-x-lg"></i></span>
                            </div>
                        </div>`;
            }
            previewContainer.html(previewContent);
        };
        reader.readAsDataURL(file);
    }
}

function handleRemoveFileClick()
{
    $(document).off("click", ".remove-file").on("click", ".remove-file", function ()
    {
        let previewContainer = $(this).closest(".file-preview");
        let inputField = previewContainer.closest(".col-md").find("input[type='file']");

        if (!previewContainer.length || !inputField.length)
        {
            console.warn("Unable to locate required elements.");
            return;
        }

        const flagName = $(inputField).data("flag");
        if (flagName)
            flags[flagName] = true;

        inputField.val("");
        previewContainer.find("*").off();
        previewContainer.empty();
    });
}

$(document).ready(function ()
{
    const supportedTypes = ["image/jpeg", "image/png", "image / jpg"];
    const maxSizeInMB = 5;

    handleFileInputChange(supportedTypes, maxSizeInMB)
    handleRemoveFileClick();
});