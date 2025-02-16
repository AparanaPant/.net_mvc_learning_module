$(document).ready(function() {
        // Function to toggle the "blink" class every 500 milliseconds
        setInterval(function() {
            $('.blink').toggleClass('blink-active');
        }, 1000);
    });