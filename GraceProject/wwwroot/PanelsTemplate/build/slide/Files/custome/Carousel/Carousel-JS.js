
//..................................................Change Slide..............................................................
var ChangeSlideAction = true;
var ChangeSlideAction_Error = "";


LoadActiveSlideWithURLInfo();
function LoadActiveSlideWithURLInfo() {
    var hash = window.location.hash;
    if (hash) {
        var slide = hash.replace('#', '');
        debugger;
        if (!isNaN(slide)) {
            SetAcitvatedCairoselSlideById(slide);
        }
    }
}
function SetActiveSlideInURL() {
    //debugger;
    var ActiveId = GetIdofActiveSlide();
    window.location.hash = ActiveId;
}

//.................Update The active slide number and total number of slide............................
$('#carousel').find('.carousel-control').unbind().click(function () {
    debugger;
    if ($('#carousel').find(".item.active").find(".QusetionSection").length > 0 &&
        GetDragAndDropItemsResult($('#carousel').find(".item.active"), $('#carousel').find(".item.active").attr('id')) == false) {
        AllowChangingSlideAction(false, "Please provide a correct answer to the question and try again later!");
        alert(ChangeSlideAction_Error);
        return false;
    }
    else {
        if (ChangeSlideAction != false) {
            UpdatecarouselIndicatorText();
            UpdateStyleOfSelectedCauroselThumbItem();
            DisplayForwardAndBackwardButtons();
        }
        else {
            alert(ChangeSlideAction_Error);
            return false;
        }
    }
});
$('#thumbcarousel').find(".carousel-inner").find(".thumb").click(function () {


    if ($('#carousel').find(".item.active").find(".QusetionSection").length > 0 &&
        GetDragAndDropItemsResult($('#carousel').find(".item.active"), $('#carousel').find(".item.active").attr('id')) == false) {
        AllowChangingSlideAction(false, "Please provide a correct answer to the question and try again later!");
        alert(ChangeSlideAction_Error);
        return false;
    }
    else {
        if (ChangeSlideAction != false) {
            UpdatecarouselIndicatorText();
            UpdateStyleOfSelectedCauroselThumbItem();
            DisplayForwardAndBackwardButtons();

            
        }
        else {
            alert(ChangeSlideAction_Error);
            return false;
        }
    }
});

function AllowChangingSlideAction(Enable, Error) {
    ChangeSlideAction = Enable;
    ChangeSlideAction_Error = Error;
}

function GetIndexofActiveSlide() {
    return $("div.item.active").index();
}
function GetIdofActiveSlide() {
    return $("div.item.active").attr("id");
}
function GetIndexofSpecialItem(item) {
    return itemIndex = $("div.item").index($(item));
}
function GetSlideInfoById(id) {
    //debugger;
    var SlideTitle = $("div.item#" + id).find("#h1_title").text();
    var SlideInfo = { 'SlideTitle': SlideTitle };
    return SlideInfo;
}


//.................Display Slide content in Thumb items............................
function CreateThumbnailSlidesBasedonSlides() {
    var carouselItems = $("#carousel").find(".item");

    // Add thumbnails to the thumbnail carousel
    for (var i = 0; i < carouselItems.length; i++) {

        var html = '<div data-target="#carousel" data-slide-to="' + i + '" class="thumb justified-text-container text-left ' + (i == 0 ? 'active' : '') + '" >';
        html += carouselItems.eq(i).html();
        html += '</div>';
        $(html).css({
            'width': '50%',
            'height': 'auto',
            'transform': 'translate(-50%, -50%) scale(0.5)' // Apply the scaling
        });

        $("#thumbcarousel .carousel-inner .item").append(html);

        //change font size
        $("#thumbcarousel").find(".thumb").find("h1").css({ "font-size": "1px", "margin": "3px" });
        $("#thumbcarousel").find(".thumb").find("h2").css({ "font-size": "1px", "margin": "3px" });
        $("#thumbcarousel").find(".thumb").find("h3").css({ "font-size": "1px", "margin": "3px" });
        $("#thumbcarousel").find(".thumb").find("h4").css({ "font-size": "1px", "margin": "3px" });
        $("#thumbcarousel").find(".thumb").find("h5").css({ "font-size": "1px", "margin": "3px" });
        $("#thumbcarousel").find(".thumb").find("label").css({ "font-size": "1px", "margin": "3px" });
        $("#thumbcarousel").find(".thumb").find("div").css({ "font-size": "1px", "margin": "3px" });
        $("#thumbcarousel").find(".thumb").find("img").css({ "width": "10px", "height": "10px" });

        $("#thumbcarousel .carousel-inner .item").css("font-size", "1px");

    }
}

function UpdatecarouselIndicatorText() {
    setTimeout(function () {
        var totalItems = $('#carousel').find('.item').length;
        currentIndex = $('#carousel').find('div.active').index() + 1;
        $('#carouselIndicatorText').html('' + currentIndex + '/' + totalItems + '');

    }, 1000);
}

function UpdateStyleOfSelectedCauroselThumbItem() {
    setTimeout(function () {
        currentIndex = $('#carousel').find('div.active').index();
        $('.carousel-inner .thumb').removeClass('active');
        $('.carousel-inner .thumb').eq(currentIndex).addClass('active');

        
        SetActiveSlideInURL();

    }, 1000);

}

function SetAcitvatedCairoselSlide(slideNum) {
    $("#carousel .item").removeClass("active");
    $("#carousel .item").eq(slideNum - 1).addClass("active");
    UpdatecarouselIndicatorText();
    UpdateStyleOfSelectedCauroselThumbItem();
    DisplayForwardAndBackwardButtons();

}
function SetAcitvatedCairoselSlideById(slideId) {
    var slide = $("#carousel").find("#" + slideId);
    if (slide.length > 0) {
        $("#carousel .item").removeClass("active");
        $(slide).addClass("active");
        UpdatecarouselIndicatorText();
        UpdateStyleOfSelectedCauroselThumbItem();
        DisplayForwardAndBackwardButtons();
    }

}

function ChangingSlideActionIsFired() {
    var _ChangingSlideIsFired;
    $('#carousel').find('.carousel-control').unbind().click(function () {
        _ChangingSlideIsFired = true;
    });
    $('#thumbcarousel').find(".carousel-inner").find(".thumb").click(function () {
        _ChangingSlideIsFired = true;
    });
    return _ChangingSlideIsFired;
}

//in each slide, we have 2 hidden parameters for dispalying the ForwardButton and BackwardButton
function DisplayForwardAndBackwardButtons() {
    setTimeout(function () {
        var ForwardButton = $(".item.active").find("#hidden_ForwardButton").val();
        var BackwardButton = $(".item.active").find("#hidden_BackwardButton").val();
        if (ForwardButton == 'true')
            $("#a_ForwardButton").removeClass("hidden");
        else
            $("#a_ForwardButton").addClass("hidden");
        if (BackwardButton == 'true')
            $("#a_BackwardButton").removeClass("hidden");
        else
            $("#a_BackwardButton").addClass("hidden");
    }, 1000);
}