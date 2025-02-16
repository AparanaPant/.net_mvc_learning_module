//parameters should be set on the controller(image, div , ...) you want to move it:
//movementDirection="horizontal" or "vertical" 
//movementDistance="230" or "-230"
//movementType="goback" or "go"

//classes should be set on the controller(image, div , ...) you want to move it:
//class="blinking-border movableElement"

function moveElement(direction, distance, controller, callback) {
    if (direction != "vertical") {
        $(controller).css('z-index', 999).animate({
            left: "+=" + distance + "px"
        }, 'slow', function () {
            // Reset z-index when animation completes
            $(this).css('z-index', '1000');
        });
        if (callback != null && callback != undefined)
            return callback(true);
    }
    else {
        $(controller).css('z-index', 999).animate({
            top: "+=" + distance + "px"
        }, 'slow', function () {
            // Reset z-index when animation completes
            $(this).css('z-index', '1000');
        });
        if (callback != null && callback != undefined)

            return callback(true);
    }
}
var intervalTimer_MoveableElementDescription;
// Example: Move the element 200 pixels to the right when the document is ready
$(document).ready(function () {
    $(".movableElement").click(function () {
        var controller = $(this);
        var type = $(controller).attr("movementType");
        var Distance = $(controller).attr("movementDistance");
        var direction = $(controller).attr("movementDirection");
        moveElement(direction, Distance, this, function (result) {
            if (result == true) {
                if (type == "goback")
                    moveElement(direction, -1 * Distance, controller, null);



                //Display a description when clicking on each image
                var Description = $(controller).attr("movementDescription");
                $("#div_MoveableElementDescription").html(Description);
                //display the div
                $("#div_MoveableElementDescription").fadeIn();
                //reset the timer
                clearInterval(intervalTimer_MoveableElementDescription);
                // Hide the description
                intervalTimer_MoveableElementDescription = setTimeout(function () {
                    $("#div_MoveableElementDescription").fadeOut();
                }, 7000);
            }
        });
    });
});