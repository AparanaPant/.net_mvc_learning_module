
$(".TransformtoSlide").click(function () {
    var TransformtoSlideNum = $(this).attr("TransformtoSlideNum");
    //SetAcitvatedCairoselSlideById(TransformtoSlideNum);

    debugger;
    if (TransformtoSlideNum != null && TransformtoSlideNum != undefined) {
        var SourceSlide = $(this).closest('.active').attr('id');
        if ($(this).attr('id') != 'a_NavigateToOtherSlide')
            NavigateToOtherSlideId(SourceSlide, TransformtoSlideNum);
        SetAcitvatedCairoselSlideById(TransformtoSlideNum);
    }

});


function NavigateToOtherSlideButton(SourceSlide, SlideNum) {
    var nthItemDiv = $("div.item:eq(" + (SlideNum - 1) + ")");

    //display button
    $(nthItemDiv).find("#a_NavigateToOtherSlide").removeClass("hidden");
    $(nthItemDiv).find("#a_NavigateToOtherSlide").attr("TransformtoSlideNum", SourceSlide);
    var SourceSlideTitle = $("div.item:eq(" + (SourceSlide - 1) + ")").find("#h1_title").html();

    //set title
    if (SourceSlideTitle != undefined && SourceSlideTitle != null && SourceSlideTitle != '')
        $(nthItemDiv).find("#a_NavigateToOtherSlide").html("Click here to return to '" + SourceSlideTitle + "'");
    else
        $(nthItemDiv).find("#a_NavigateToOtherSlide").html("Click here to return back");

}
function NavigateToOtherSlideId(FromSlideId, SlideId, DisplayReturnToMainSlideButton) {
    var nthItemDiv = $('div#' + SlideId + '.item');

    //display button
    $(nthItemDiv).find("#a_NavigateToOtherSlide").removeClass("hidden");
    $(nthItemDiv).find("#a_NavigateToOtherSlide").attr("TransformtoSlideNum", FromSlideId);
    //find title of the source slide
    var SourceSlideTitle = $('div#' + FromSlideId + '.item').find("#h1_title").html();

    if (DisplayReturnToMainSlideButton == false)
        ;
    else {
        //set title
        if (SourceSlideTitle != undefined && SourceSlideTitle != null && SourceSlideTitle != '')
            $(nthItemDiv).find("#a_NavigateToOtherSlide").html("Click here to return to '" + SourceSlideTitle + "'");
        else
            $(nthItemDiv).find("#a_NavigateToOtherSlide").html("Click here to return back");
    }
}