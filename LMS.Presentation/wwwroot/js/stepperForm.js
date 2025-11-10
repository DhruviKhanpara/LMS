var step = 1;

$(".step-next").on("click", function ()
{
    $(".step").hide();
    $("#step" + step).hide();
    step++;
    $("#step" + step).show();
    stepProgress(step);
    hideButtons(step);
});

$(".step-back").on("click", function ()
{
    $(".step").hide();
    step--;
    $("#step" + step).show();
    stepProgress(step);
    hideButtons(step);
});

function stepProgress(currStep)
{
    var percent = (100 / $(".step").length) * currStep;
    $(".progress-bar").css("width", percent + "%");
}

function hideButtons(step)
{
    $(".action").hide();
    if (step > 1)
    {
        $(".step-back").show();
    }
    if (step < $(".step").length)
    {
        $(".step-next").show();
    } else
    {
        $(".submit").show();
    }
}

$(document).ready(function ()
{
    stepProgress(step);
    hideButtons(step);
});