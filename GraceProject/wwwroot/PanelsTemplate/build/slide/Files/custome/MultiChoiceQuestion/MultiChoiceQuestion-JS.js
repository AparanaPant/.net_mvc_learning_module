
function CreateAndInitiateMultiChoiceQuestionHTML(Container,ControllerParameters) {

    var Question = ControllerParameters.Question;
    //list of choices and the description for choices : Title and Description
    var Choices = ControllerParameters.Choices;
    var CorrectAnswers = ControllerParameters.CorrectAnswers;


    var questionHTML = '<label>' + Question + '</label>';

    // Iterate over the choices and create radio buttons
    $.each(Choices, function (index, choice) {
        questionHTML += '<div class="RadioButtonChoice">';
        questionHTML += '<input type="radio" name="RadioButtonChoice" value="' + choice.text + '" id="choice' + index + '">';
        questionHTML += '<label for="choice' + index + '">&nbsp;' + choice.text + '</label>';
        questionHTML += '<label name="Choice_Description" style="display:none;">' + choice.description + '</label>';
        questionHTML += '</div>';
    });
    questionHTML+='<div id="MultiChoiceQuestion_Description" class="hide alert alert-info text-center" role="alert" >---</div>';

    $('#' + Container).append(questionHTML);

    $('#' + Container).find('input[name="RadioButtonChoice"]').unbind().on('change', function () {

        // Get the selected radio button value (its associated title/label)
        var selectedValue = $("#" + Container).find('input[name="RadioButtonChoice"]:checked').val();

        // Find the label associated with the selected radio button
        var selectedText = $("#" + Container).find('label[for="' + $('input[name="RadioButtonChoice"]:checked').attr('id') + '"]').text();



        var Description = '';
        $("#" + Container).find('input[name="RadioButtonChoice"]:checked').each(function () {
            // Get the description of each checked radio button
            var _description = $(this).parent('.RadioButtonChoice').find('[name="Choice_Description"]').text();

            // Append each description with a <br> at the end
            Description += _description + '<br>';
        });


        $('#' + Container).find('#MultiChoiceQuestion_Description').html('');
        $('#' + Container).find('#MultiChoiceQuestion_Description').html(Description);
        $('#' + Container).find('#MultiChoiceQuestion_Description').removeClass('hide');

        if (checkSelectedAnswers(CorrectAnswers) == true) {
            AllowChangingSlideAction(true, null);
            alert("Congratulation, your answer is correc!");
            return;
        }
        else {
            AllowChangingSlideAction(false, "Please provide a correct answer to the question and try again later!");
            return;
        }
    });
}
function checkSelectedAnswers(CorrectAnswers) {

    debugger;

    var selectedAnswers = $('input[name="RadioButtonChoice"]:checked').map(function () {
        return $(this).val().trim();
    }).get();

    var allCorrect = true;
    for (var i = 0; i < selectedAnswers.length; i++) {
        var found = false;
        for (var j = 0; j < CorrectAnswers.length; j++) {
            if (CorrectAnswers.length == selectedAnswers.length && selectedAnswers[i].trim() === CorrectAnswers[j].text.trim()) {
                found = true;
                break;
            }
        }
        if (!found) {
            allCorrect = false;
            break;
        }
    }
    if (allCorrect == true && selectedAnswers.length > 0)
        return allCorrect;
    else
        return false;
}