let slideStartTime; // Variable to store slide entry time

// Call this function when the slide is loaded
function startSlideTimer() {
    slideStartTime = null;
    slideStartTime = new Date().getTime(); // Capture current timestamp
}

function StoreSlideReading(slideId) {
    debugger;
    //get the sidetime
    let currentTime = new Date().getTime();
    let durationSeconds = Math.floor((currentTime - slideStartTime) / 1000); // Convert milliseconds to seconds
    if (!slideStartTime || !(durationSeconds>0)) {
        return;
    }
    var jsonData = {
        "SlideId": slideId,
        "DurationSeconds": durationSeconds
    };
    // Convert JSON object to string
    var jsonString = JSON.stringify(jsonData);

    $.ajax({
        url: "/Student/SlideReadTrackingController/StoreSlideReading",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(jsonString),
        success: function (response) {
            console.log("Slide read entry added successfully:", response);
        },
        error: function (xhr, status, error) {
            console.error("Error adding slide read entry:", xhr.responseText);
        }
    });
}