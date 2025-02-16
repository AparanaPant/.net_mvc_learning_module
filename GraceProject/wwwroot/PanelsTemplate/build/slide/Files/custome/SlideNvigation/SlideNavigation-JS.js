function CreateAndInitiateSlideNavigationHTML(Container, ControllerParameters) {

    var ToSlideId = ControllerParameters.SlideId;
    var SlideTitle = ControllerParameters.SlideTitle;

    var ToSlideInfo = GetSlideInfoById(ToSlideId);

    var FromSlideId = GetIdofActiveSlide();
    var FromSlideInfo = GetSlideInfoById(FromSlideId);

    
    var NavigationHTML = '';
    // Iterate over the choices and create radio buttons
        NavigationHTML += '<div>';
    NavigationHTML += '<button class="SlideNavigationButton btn btn-info" FromSlideId="' + FromSlideId + '" ToSlideId="' + ToSlideId + '" title = "Navigate to Slide" >' + SlideTitle +'</button >';
        NavigationHTML += '</div>';

    $('#' + Container).append(NavigationHTML);

    $('#' + Container).find('.SlideNavigationButton').unbind().click(function () {
        debugger;
        var FromSlideId = GetIdofActiveSlide();
        var ToSlideId = $(this).attr("ToSlideId");
        NavigateToOtherSlideId(FromSlideId, ToSlideId);
        SetAcitvatedCairoselSlideById(ToSlideId);
    });
}
