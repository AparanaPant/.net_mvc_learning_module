function DisplaySlides(Slides, SlideContainer, ThumbSlideContainer) {
    for (var i = 0; i < Slides.length; i++) {

        //slide sections
        var slidesections = "";

        var div_Section1_Id = "div_" + (i + 1) + "_Section1";
        var div_Section2_Id = "div_" + (i + 1) + "_Section2";
        var div_Section3_Id = "div_" + (i + 1) + "_Section3";
        var div_Section4_Id = "div_" + (i + 1) + "_Section4";


        //vertical
        if (Slides[i].SlideSectionsType == "vertical") {
            if (Slides[i].NumSlideSections === 1) {
                slidesections = '<div id="' + div_Section1_Id + '" class="col-lg-12" style="margin-top:5px;margin-bottom:5px;"></div>';
            } else if (Slides[i].NumSlideSections === 2) {
                slidesections = '<div id="' + div_Section1_Id + '" class="col-lg-6" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section2_Id + '" class="col-lg-6" style="margin-top:5px;margin-bottom:5px;"></div>';
            } else if (Slides[i].NumSlideSections === 3) {
                slidesections = '<div id="' + div_Section1_Id + '" class="col-lg-4" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section2_Id + '" class="col-lg-4" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section3_Id + '" class="col-lg-4" style="margin-top:5px;margin-bottom:5px;"></div>';
            } else if (Slides[i].NumSlideSections === 4) {
                slidesections = '<div id="' + div_Section1_Id + '" class="col-lg-3" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section2_Id + '" class="col-lg-3" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section3_Id + '" class="col-lg-3" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section4_Id + '" class="col-lg-3" style="margin-top:5px;margin-bottom:5px;"></div>';
            }
        }
        //horizontal
        else {
            if (Slides[i].NumSlideSections === 1) {
                slidesections = '<div id="' + div_Section1_Id + '" class="col-lg-12" style="margin-top:5px;margin-bottom:5px;"></div>';
            } else if (Slides[i].NumSlideSections === 2) {
                slidesections = '<div id="' + div_Section1_Id + '" class="col-lg-12" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section2_Id + '" class="col-lg-12" style="margin-top:5px;margin-bottom:5px;"></div>';
            } else if (Slides[i].NumSlideSections === 3) {
                slidesections = '<div id="' + div_Section1_Id + '" class="col-lg-12" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section2_Id + '" class="col-lg-12" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section3_Id + '" class="col-lg-12" style="margin-top:5px;margin-bottom:5px;"></div>';
            } else if (Slides[i].NumSlideSections === 4) {
                slidesections = '<div id="' + div_Section1_Id + '" class="col-lg-12" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section2_Id + '" class="col-lg-12" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section3_Id + '" class="col-lg-12" style="margin-top:5px;margin-bottom:5px;"></div><div id="' + div_Section4_Id + '" class="col-lg-12" style="margin-top:5px;margin-bottom:5px;"></div>';
            }
        }



        //add slide
        var slideHTML = '<div class="item ' + (i == 0 ? 'active' : '') + '" id="' + Slides[i].Id + '">' +
            //forward and backward buttons
            '<input type="hidden" id="hidden_ForwardButton" value="' + Slides[i].ForwardButton + '" /><input type="hidden" id="hidden_BackwardButton" value="' + Slides[i].BackwardButton + '"/> ' +
            //Navigate To another slide
            '<a id="a_NavigateToOtherSlide" class="hidden TransformtoSlide col-xs-12 blinking-border" TransformtoSlideNum="" style="text-align:center;font-size: 16px;font-weight: bold;padding:5px;margin-bottom:20px;"></a>' +
            '<div id="div_slidetitle" class="Text-To-Speech">' +
            //if the title should be displayed
            ((Slides[i].DisplayTitle == true) ? '<h1 id="h1_title">' + Slides[i].Title + '</h1>' : '') +
            '</div>' +
            '<div id="div_slidebody">' +
            slidesections +
            '</div>' +
            '</div>';
        $("#" + SlideContainer).append(slideHTML);

        //add thumbslide
        var thumbslideHTML = '<div data-target="#carousel" data-slide-index="' + Slides[i].Id + '" data-slide-to="' + i + '" class="thumb justified-text-container text-center ' + (i == 0 ? ' active ' : '') + (Slides[i].DisplayThumbnailSlide != true ? ' hide ' : '') + '">' +
            '<i id="i_thumbslide" class="' + Slides[i].ThumbIcon + ' fa-3x"></i>' +
            '<br>' +
            '<label id="label_thumbslide">' + Slides[i].ShortTitle + '</label>' +
            '</div>';
        $("#" + ThumbSlideContainer).append(thumbslideHTML);

        //Adding sections
        if (Slides[i].NumSlideSections >= 1)
            SlideSectionHTML(Slides[i].Id, Slides[i].SlideSections[0], div_Section1_Id);
        if (Slides[i].NumSlideSections >= 2)
            SlideSectionHTML(Slides[i].Id, Slides[i].SlideSections[1], div_Section2_Id);
        if (Slides[i].NumSlideSections >= 3)
            SlideSectionHTML(Slides[i].Id, Slides[i].SlideSections[2], div_Section3_Id);
        if (Slides[i].NumSlideSections >= 4)
            SlideSectionHTML(Slides[i].Id, Slides[i].SlideSections[3], div_Section4_Id);



    }
    DisplayForwardAndBackwardButtons();

}
function SlideSectionHTML(SlideId, SlideSection, Container) {
    if (SlideSection == undefined)
        return '';
    //controller html: img, clickable image,moveable items, Drag and Drop items
    let ControllerHTML = '';
    let ControllerName = $.trim(SlideSection.ControllerName);
    let ControllerParameters = $.trim(SlideSection.ControllerParameters) != '' ? JSON.parse($.trim(SlideSection.ControllerParameters)) : null;

    if (ControllerName != null && ControllerName != '') {

        debugger;
        
            //Display slide navigation button
            if (ControllerName == 'SlideNavigation') {
                CreateAndInitiateSlideNavigationHTML(Container, ControllerParameters);
            }
            else
                //Display multi choice question
                if (ControllerName == 'MultiChoiceQuestion') {
                    CreateAndInitiateMultiChoiceQuestionHTML(Container, ControllerParameters);
                }
                else
                    //Display an image and some arrows on it
                    if (ControllerName == 'TransportProcess') {
                        ////debugger;
                        var NormalizedParameters = DisplayTransportProcess(ControllerParameters, 'display');

                        // Append elements to container
                        $('#' + Container).append(
                            NormalizedParameters.leftButton,
                            NormalizedParameters.canvas,
                            NormalizedParameters.rightButton);

                        InitiateDisplayTransportProcess(
                            'display',
                            Container,
                            NormalizedParameters
                        );

                    }
                    else if (ControllerName == 'Video') {
                        ////debugger;
                        var URL = ControllerParameters.URL;


                        /* Check if the URL is a kind of Youtube URL */
                        var youtubeUrlPattern = /^(https?:\/\/)?(www\.)?(youtube\.com|youtu\.be)\/.+$/;
                        var iframeSrc = "";
                        if (youtubeUrlPattern.test(URL)) {

                            var standardUrlMatch = URL.match(/(?:https?:\/\/)?(?:www\.)?youtube\.com\/watch\?v=([^&]+)/);
                            var embedUrlMatch = URL.match(/(?:https?:\/\/)?(?:www\.)?youtube\.com\/embed\/([^&]+)/);

                            if (standardUrlMatch && standardUrlMatch[1]) {
                                videoId = standardUrlMatch[1];
                            } else if (embedUrlMatch && embedUrlMatch[1]) {
                                videoId = embedUrlMatch[1];
                            } else {
                                alert("Please enter a valid YouTube URL");
                                return;
                            }
                            iframeSrc = "https://www.youtube.com/embed/" + videoId;
                        }



                        ControllerHTML = '<div style="text-align:center;position: relative;width: 100%;padding-top: 56.25%; overflow: hidden; ">' +
                            '<iframe style="position: absolute;top: 0;left: 0;width: 100%;height: 100%;border: 0;" id="youtubeVideo" src="' + iframeSrc + '" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>' +
                            '</div>';
                    }
                    else
                        if (ControllerName == 'Image') {
                            var Path = ControllerParameters.Path;
                            var TransformtoSlide = ControllerParameters.TransformtoSlide;
                            var TransformtoSlideNum = ControllerParameters.TransformtoSlideNum;
                            var BlinkingBorder = ControllerParameters.BlinkingBorder;
                            var Title = ControllerParameters.Title;
                            var Alt = ControllerParameters.Alt;
                            var Width = ControllerParameters.Width;
                            var Align = ControllerParameters.Align;

                            ControllerHTML = '<div style="text-align:' + Align + '">' +
                                '<img src="' + Path + '" alt="' + Alt + '" class="' + (TransformtoSlide == 'true' ? ' TransformtoSlide  blinking-border ' : '') + (BlinkingBorder == 'true' ? ' ' : '') + '"' + (TransformtoSlide == 'true' ? 'TransformtoSlideNum="' + TransformtoSlideNum + '"' : '') + '  title="' + Title + '" style="width:' + Width + 'px;">' +
                                '</div>';
                        }
                        else
                            if (ControllerName == 'ClickableImages') {
                                ControllerHTML = CreateClickableImageandAreasHTML(ControllerParameters);
                            }
                            else
                                if (ControllerName == 'DragandDropItems') {
                                    debugger;

                                    var Items = ControllerParameters.Items;
                                    var ItemsColumnCaption = ControllerParameters.ItemsColumnCaption;
                                    var AnswerColumnCaption = ControllerParameters.AnswerColumnCaption;
                                    var CheckAnswerByChangingSlide = ControllerParameters.CheckAnswerByChangingSlide;
                                    var DragAndDropItemDescription = ControllerParameters.Description;
                                    var CorrectItems = ControllerParameters.CorrectItems;
                                    var BackgroundImageOfTank = ControllerParameters.BackgroundImageOfTank;
                                    var BackgroundImageOfAnswer = ControllerParameters.BackgroundImageOfAnswer;
                                    var DescriptionLocation = ControllerParameters.DescriptionLocation;


                                    //Get the height of BackgroundImageOfTank
                                    var img_BackgroundImageOfTank = new Image();
                                    img_BackgroundImageOfTank.src = BackgroundImageOfTank;
                                    var height_BackgroundImageOfTank;
                                    $(img_BackgroundImageOfTank).on('load', function () {
                                        height_BackgroundImageOfTank = $(this).height();
                                    });

                                    //Get the height of BackgroundImageOfAnswer
                                    var img_BackgroundImageOfAnswer = new Image();
                                    img_BackgroundImageOfAnswer.src = BackgroundImageOfAnswer;
                                    var height_BackgroundImageOfAnswer;
                                    $(img_BackgroundImageOfAnswer).on('load', function () {
                                        height_BackgroundImageOfAnswer = $(this).height();
                                    });


                                    //items
                                    var ItemsHTML = Items.map(function (item, index) {
                                        var br = (index + 1) % 2 === 0 && index + 1 !== Items.length ? '<br>' : ''; // Add <br> after every 3 items except for the last set of items
                                        return '<div description="' + item.description + '" style="display: inline-block;">' + item.text + '</div>' + br;
                                    }).join('');

                                    var DragandDropItemsHTML = '<div class="QusetionSection parent">' +
                                        '<div style="font-size: 14px;">' + DragAndDropItemDescription + '</div>' +
                                        (DescriptionLocation == "top" ? '<div id = "customAlert" class="alert alert-info text-center" role = "alert" style = "display: none;opacity:1;" ></div>' : '') +
                                        '<div class="wrapper" style="font-size: 14px;width:100%;">' +
                                        '<div id="left-rm-spill' + SlideId + '" class="container text-center" style="background-color:unset; padding: 10px;' + (BackgroundImageOfTank != '' && BackgroundImageOfTank != null && BackgroundImageOfTank != undefined ? 'background: url(' + BackgroundImageOfTank + ') no-repeat center center; background-size: cover;height:500px;' : '') + '">' +

                                        '<b>' + ItemsColumnCaption + '</b>' +
                                        (ItemsColumnCaption != null && ItemsColumnCaption != undefined && ItemsColumnCaption != '' ? '<hr style="margin: 7px;"><br/><br/><br/>' : '') +
                                        ItemsHTML +
                                        '</div>' +
                                        '<div id="right-rm-spill' + SlideId + '" class="container text-center" style="background-color:unset;padding: 10px;' + (BackgroundImageOfAnswer != '' && BackgroundImageOfAnswer != null && BackgroundImageOfAnswer != undefined ? 'background: url(' + BackgroundImageOfAnswer + ') no-repeat center center; background-size: cover;height:500px;' : '') + ' ">' +
                                        '<b>' + AnswerColumnCaption + '</b>' +
                                        (AnswerColumnCaption != null && AnswerColumnCaption != undefined && AnswerColumnCaption != '' ? '<hr style="margin: 7px;"><br/><br/><br/>' : '') +
                                        '</div>' +
                                        '</div>' +
                                        '<br><input type="hidden" id="hidden_result' + SlideId + '"/>' +
                                        (DescriptionLocation == "bottom" ? '<div id = "customAlert" class="alert alert-info text-center" role = "alert" style = "display: none;opacity:1;" ></div>' : '');

                                    //display a notification
                                    var NotificationHTML = '<div class="container mt-3"><div class="alert alert-success" role = "alert" id = "Div_DragAndDropItems_Message' + SlideId + '" style = "display: none;position: fixed;left:50%;top: 20px;left: 50 %;transform: translateX(-50 %);z-index:1000;" ></div ></div > ';

                                    DragandDropItemsHTML += NotificationHTML;

                                    $('#' + Container).append(DragandDropItemsHTML);

                                    var drake = dragula([document.getElementById('left-rm-spill' + SlideId), document.getElementById('right-rm-spill' + SlideId)]);

                                    // Define variables to store original positions
                                    var originalPositions = {};
                                    // Function to store original positions before dragging starts
                                    drake.on('drag', function (el, source) {
                                        var itemId = el.getAttribute('id');
                                        originalPositions[itemId] = el;
                                    });

                                    drake.on('drop', function (el, target, source, sibling) {
                                        openCustomAlert(Container, $(el).attr("description"));

                                        //Selected Item
                                        var draggedText = $.trim($(el).html());

                                        var correctItem = false;
                                        for (var j = 0; j < CorrectItems.length; j++) {
                                            var CorrectItem = $.trim($(CorrectItems[j])[0].text);
                                            if (CorrectItem != '' && CorrectItem != null && (draggedText == CorrectItem)) {
                                                correctItem = true;
                                                break;
                                            }
                                        }
                                        if ($(target).attr("id") == 'right-rm-spill' + SlideId) {
                                            if (correctItem == true) {
                                                $('#Div_DragAndDropItems_Message' + SlideId).removeClass("alert-danger").addClass("alert-success");
                                                $('#Div_DragAndDropItems_Message' + SlideId).html("Correct!");
                                                $('#Div_DragAndDropItems_Message' + SlideId).fadeIn().delay(2000).fadeOut(); // Show message for 2 seconds

                                                $(el).css('color', 'green'); // Change color to red
                                            }
                                            else {
                                                $('#Div_DragAndDropItems_Message' + SlideId).removeClass("alert-success").addClass("alert-danger");
                                                $('#Div_DragAndDropItems_Message' + SlideId).html("'" + draggedText + "' is not correct, try again!");
                                                $('#Div_DragAndDropItems_Message' + SlideId).fadeIn().delay(4000).fadeOut(); // Show message for 2 seconds


                                                // Move uncorrected item back to its original position
                                                var itemId = el.getAttribute('id');
                                                var originalPosition = originalPositions[itemId];
                                                if (originalPosition) {
                                                    // If original position is defined, move the item back
                                                    source.insertBefore(el, originalPosition.nextSibling);
                                                }

                                                $(el).css('color', 'red'); // Change color to red
                                            }
                                        }
                                        debugger;
                                        CheckAnswer(SlideId, Container, CorrectItems, function (result) {
                                            debugger;
                                            if (result == false) {
                                                AllowChangingSlideAction(false, "Please provide a correct answer to the question and try again later!");
                                            }
                                            else {
                                                AllowChangingSlideAction(true, null);
                                                alert("Congratulation, your answer is correc!");
                                            }
                                        });

                                    })

                                }
                                else
                                    if (ControllerName == 'Animate') {
                                        debugger;
                                        //display Description
                                        if (ControllerParameters.DescriptionLocation == "top") {
                                            ControllerHTML += '<div class="col-lg-12">';
                                            ControllerHTML += '<br/><div class="col-lg-12 alert alert-info " role="alert" style="display:none;" id="div_MoveableElementDescription"></div>';
                                            ControllerHTML += '</div>';
                                        }

                                        //if the direction of the objects( that images fly to) is left, movementDistance is a negative number
                                        if (ControllerParameters.Location == "left")
                                            ControllerHTML += '<div class="col-lg-6"><img src="' + ControllerParameters.FlyingToImage + '" style="max-width:500px;"></div>';

                                        ControllerHTML += '<div class="col-lg-6">';
                                        $.each(ControllerParameters.MovableElements, function (index, element) {
                                            ControllerHTML += '<img class="' + (element.BlinkingBorder == 'true' ? ' blinking-border ' : '') + ' movableElement" ' +
                                                'movementDirection="' + element.MovementDirection + '" ' +
                                                'movementDescription="' + element.MovementDescription + '" ' +
                                                'movementDistance="' + element.MovementDistance + '" ' +
                                                'movementType="' + element.MovementType + '" ' +
                                                'src="' + element.Path + '"' +
                                                'style="z-idex:1000;width:100px;">' +
                                                '<br><br><br><br><br>';
                                        });
                                        ControllerHTML += '</div>';

                                        //if the direction of the objects( that images fly to) is right, movementDistance is a positive number
                                        if (ControllerParameters.Location == "right")
                                            ControllerHTML += '<div class="col-lg-6"><img src="' + ControllerParameters.FlyingToImage + '" style="max-width:500px;"></div>';

                                        //display Description
                                        if (ControllerParameters.DescriptionLocation == "bottom") {
                                            ControllerHTML += '<div class="col-lg-12">';
                                            ControllerHTML += '<br/><div class="col-lg-12 alert alert-info " role="alert" style="display:none;" id="div_MoveableElementDescription"></div>';
                                            ControllerHTML += '</div>';
                                        }
                                    }
                                    else
                                        if (ControllerName == 'PieChart') {

                                            $('#' + Container).append('<div id="pie_chart"></div>');

                                            var series = ControllerParameters.series;
                                            let intseries = $.map(series, function (value) {
                                                return parseInt(value, 10);
                                            });
                                            var labels = ControllerParameters.labels;
                                            var colors = ControllerParameters.colors;
                                            var type = ControllerParameters.type;
                                            var TransformtoSlideNum = ControllerParameters.TransformtoSlideNum;
                                            var options = {
                                                series: intseries,
                                                chart: {
                                                    width: 500,
                                                    type: type,
                                                    events: {
                                                        click: function (event, chartContext, config) {
                                                            var label = config.config.labels[config.dataPointIndex];;

                                                            // Find the index of the special label in the first array
                                                            let index = $.inArray(label, labels);

                                                            if (TransformtoSlideNum != undefined) {
                                                                // Get the corresponding value from the second array
                                                                let ToSlideId = (index !== -1) ? TransformtoSlideNum[index] : null;
                                                                let FromSlideId = GetIdofActiveSlide();

                                                                if (SlideId > 0) {
                                                                    //debugger;
                                                                    //var SourceSlide = GetIndexofSpecialItem($(this).closest('.item'));
                                                                    NavigateToOtherSlideId(FromSlideId, ToSlideId);
                                                                    SetAcitvatedCairoselSlideById(ToSlideId);
                                                                }
                                                            }

                                                            //display modal
                                                            //$('#div_Modal_Information').modal('show');
                                                            //$(".modal-backdrop").remove();

                                                        }
                                                    }
                                                },
                                                labels: labels,
                                                responsive: [{
                                                    breakpoint: 480,
                                                    options: {
                                                        chart: {
                                                            width: 200
                                                        },
                                                        legend: {
                                                            position: 'bottom'
                                                        }
                                                    }
                                                }],
                                                colors: colors // Specify your custom colors here
                                            };
                                            var chart = new ApexCharts(document.getElementById(Container).querySelector("#pie_chart"), options);
                                            chart.render();
                                        }
    }
    //description
    let Description = $.trim(SlideSection.Description);
    //we use QillEditor and it uses corresponding css. we should adjust the container classname to see the effect of the its classes
    var QillEditorContainerClassName = GetQillEditorJSContainerClassName();
    //Slide HTML
    var slideHTML =
        '<div ' + ((SlideSection.TextToVoice == true) ? ' class="Text-To-Speech ' + QillEditorContainerClassName + '"' : ' class="' + QillEditorContainerClassName + '"') + ' style="overflow-y: unset">' +
        //if the title should be displayed
        ((SlideSection.Title != null && $.trim(SlideSection.Title) != '') ? '<h1>' + Slides[i].Title + '</h1>' : '') +
        '<div>' +
        ((Description != null && Description != '') ? Description : ControllerHTML) +
        '</div>' +
        '</div>';

    $('#' + Container).append(slideHTML);

    //initiate Clickable Images
    if (ControllerName == 'ClickableImages') {
        InitiateClickableImageandAreas(Container, 'display', ControllerParameters);
    }
}

var CustomAlertTimer;
function openCustomAlert(container, alerttext) {
    // Show the custom alert
    $('#' + container).find('#customAlert').fadeIn();
    $('#' + container).find('#customAlert').html(alerttext);

    clearInterval(CustomAlertTimer);
    // Close the custom alert after 2 seconds
    /*CustomAlertTimer=setTimeout(function(){
        $('#customAlert').fadeOut();
    }, 2000);*/
}
function CheckAnswer(SlideId, Container, CorrectAnswers, result) {
    var items = $('#right-rm-spill' + SlideId + ' div');


    var correctitems = 0;
    for (i = 0; i < items.length; i++) {
        for (var j = 0; j < CorrectAnswers.length; j++) {

            var draggedText = $(items[i]).text();

            var tempElement = $('<div>').html($(CorrectAnswers[j])[0].text);
            var CorrectItem = tempElement.text();


            if (CorrectItem != '' && CorrectItem != null && draggedText == CorrectItem) {
                //////debugger;
                correctitems++;
            }
        }
    }
    if (CorrectAnswers.length == correctitems && items.length == CorrectAnswers.length) {
        $("#" + Container).find('#hidden_result' + SlideId).val("correct");
        return result(true);
    }
    else {
        $("#" + Container).find('#hidden_result' + SlideId).val("incorrect");
        return result(false);
    }
}
function GetDragAndDropItemsResult(Container, id) {
    if ($(Container).find("#hidden_result" + id).val() == "correct")
        return true;
    else
        return false;
}

//As we used ql js for editor, we should use its classes for displaying the content
function GetQillEditorJSContainerClassName() {
    return 'ql-editor';
}
