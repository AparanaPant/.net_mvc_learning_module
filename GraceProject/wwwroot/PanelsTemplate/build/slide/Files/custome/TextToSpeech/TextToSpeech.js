//...................Text to Speech.......................
const playButton = document.getElementById('playButton');
const stopButton = document.getElementById('stopButton');
const volumeRange = document.getElementById('volumeRange');
const rateRange = document.getElementById('rateRange');
let currentUtterance = null;

if (playButton != undefined && playButton != null) {
    playButton.addEventListener('click', () => {
        var textToSpeechContainer = $("#carousel").find(".item.active"); //container
        const textToSpeakArray = $(textToSpeechContainer).find(".Text-To-Speech");
        for (var i = 0; i < textToSpeakArray.length; i++) {
            var textToSpeak = textToSpeakArray[i].innerText;
            if (textToSpeak != '' && textToSpeak != null && textToSpeak.trim() !== '') {
                currentUtterance = new SpeechSynthesisUtterance(textToSpeak);
                currentUtterance.volume = parseFloat(volumeRange.value);
                currentUtterance.rate = parseFloat(rateRange.value);

                // Attempt to set the voice to "Microsoft Mark - English (United States)"
                const voices = window.speechSynthesis.getVoices();
                const microsoftMarkVoice = voices.find(voice => voice.name === 'Microsoft Mark - English (United States)');

                if (microsoftMarkVoice) {
                    currentUtterance.voice = microsoftMarkVoice;
                    speechSynthesis.speak(currentUtterance);
                }
                else {
                    playButton.click();
                }
            }
        }
    });

    //if I do not use the below code, the first click of play button will not be fired
    playButton.click();
}

if (stopButton != undefined && stopButton != null) {
    stopButton.addEventListener('click', () => {
        if (currentUtterance) {
            speechSynthesis.cancel();
        }
    });
}

if (volumeRange != undefined && volumeRange != null) {
    volumeRange.addEventListener('input', () => {
        if (currentUtterance) {
            currentUtterance.volume = parseFloat(volumeRange.value);
        }
    });
}
if (rateRange != undefined && rateRange != null) {
    rateRange.addEventListener('input', () => {
        if (currentUtterance) {
            currentUtterance.rate = parseFloat(rateRange.value);
        }
    });
}