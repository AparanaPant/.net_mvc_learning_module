$('.fullscreen-btn').click(function () {
    toggleFullscreen("carousel");
});

function toggleFullscreen(elementId) {
    var elem = document.getElementById(elementId);

    if (!document.fullscreenElement) {
        if (elem.requestFullscreen) {
            elem.requestFullscreen();
        }
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        }
    }
}