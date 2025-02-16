function CreateClickableImageandAreasHTML(ControllerParameters) {
    var Path = ControllerParameters.Path;
    var Alt = ControllerParameters.Alt;
    var areas = ControllerParameters.areas;

    var areaElements = "";
    $.each(areas, function (index, area) {
        var areaHtml = '<area alt="" title="" href="#" shape="' + area.shape + '" name="' + area.area + '" data-tooltip="' + area["data-tooltip"] + '" coords="' + area.coords + '" class="TransformtoSlide" TransformtoSlideNum="' + area.TransformtoSlideNum + '">';
        areaElements += areaHtml;
    });



    ControllerHTML = '<div>' +
        '<div id="imagemap" style="text-align:center;">' +
        '<img id="image_Professional_Clickable" usemap="#plant_map" src="' + Path + '" alt="' + Alt + '">' +
        '</div>' +
        '<map name="plant_map" id="plant_map">' +
        areaElements +
        '</map>' +
        '</div>';

    return ControllerHTML;

}

/**
 * 
 * @param {any} Container
 * @param {any} Action: display or edit
 */
function InitiateClickableImageandAreas(Container, Action) {



    //make an image clickable
    var $image = $("#" + Container).find('#image_Professional_Clickable'),
        imageOptions = {
            mapKey: 'name',
            fill: false,
            fillOpacity: 0.4,
            fillColor: 'd42e16',
            strokeColor: 'FF0000',
            strokeOpacity: 0.8,
            staticTooltip: true,
            strokeWidth: 3,
            render_select: {
                strokeWidth: 3
            },
            render_highlight: {
                strokeWidth: 3
            },
            stroke: true,
            clickNavigate: false,
            onClick: function (e) {
                //TransformToSlide
                var TransformtoSlideNum = $("#" + Container).find('area[name="' + e.key + '"]').attr('TransformtoSlideNum');
                var SourceSlide = GetIndexofSpecialItem($image.closest('.item'));
                NavigateToOtherSlideId((SourceSlide + 1), TransformtoSlideNum);

                var title = $("#" + Container).find('area[name="' + e.key + '"]').attr('data-tooltip');

                var span;
                $("#" + Container).find('[id="span_map_title"]').each(function () {
                    if ($(this).text() == title)
                        span = $(this);
                })
                $(span).html('');
                $(span).html('<i class="glyphicon glyphicon-ok"></i>' + title);

                //opening popup
                /*$("#ClickableModal").find("#title").html(title);
                $("#ClickableModal").modal("show");
                $(".modal-backdrop").remove();*/

                return true;
            },
            showToolTip: true,
            toolTip: function (data) {
                return $(data.target).data('tooltip');
            },
            /*areas: [
                {
                    key: 'area1',
                    staticState: true,
                    fill: false,
                    strokeWidth: 1
                },
                {
                    key: 'area2',
                    staticState: true,
                    strokeWidth: 1
                },
                {
                    key: 'area3',
                    staticState: true,
                    strokeWidth: 1
                },
                {
                    key: 'area4',
                    staticState: true,
                    strokeWidth: 1
                }
            ]*/
        };

    $image.mapster(imageOptions);


    //ADD LABEL FOR EACH AREA : firstly we should change the position of "imagemap" div to relative, then we add a span for each area and change the position to absolute
    $("#" + Container).find("div[id=imagemap]").css({ "position": "relative" });
    $("#" + Container).find("#plant_map").find('area').each(function () {
        var txt = $(this).attr('data-tooltip');
        var coor = $(this).attr('coords');
        var coorA = coor.split(',');
        var left = parseInt(coorA[0]) + 5;
        var top = parseInt(coorA[1]);
        var transformtoslidenum = $(this).attr('transformtoslidenum'); 


        var coords = coor.split(',').map(Number);
        CreateAreaDesign({ 'x': left, 'y': top }, coords, Container, txt, { 'DeleteButton': (Action == 'edit' ? true : false) }, transformtoslidenum);
    })

}


var areas = [];
function AddandDeleteAreas(Container) {
    $('#' + Container).find('#image_Professional_Clickable').click(function (event) {
        //debugger;
        var imgOffset = $(this).offset();
        var x = event.pageX - imgOffset.left;
        var y = event.pageY - imgOffset.top;
        var radius = 15; // Default radius value, can be changed as needed

        var label = prompt("Enter label for this area:");
        if (label) {
            var coords = [x.toFixed(2), y.toFixed(2), radius];
            areas.push({ coords: coords, label: label });

            CreateAreaDesign({ 'x': x, 'y': y }, coords, Container, label, { 'DeleteButton': true },null);

            //...............add it as a new record of table...............
            AddNewRowToProfessionalClickableTable(Container, (x + ',' + y + ',' + radius), label)
        }
    });
}

function CreateAreaDesign(DisplayingCoord, coords, Container, AreaLabel, Buttons, TransformtoSlideNum) {

    // Create a marker for the clicked area
    var $span = $('<span id="span_map_title" class="map_title"></span>').css({
        position: 'absolute',
        top: DisplayingCoord.y + 20 + 'px',
        left: DisplayingCoord.x + 'px',
        color: 'black',
        fontSize: '14px'
    }).appendTo('#' + Container + ' #imagemap');

    $('<i class="glyphicon glyphicon-arrow-right"></i>').appendTo($span);
    $span.append(AreaLabel);

    // Create the marker inside the span
    $('<div class="marker TransformtoSlide" ' + (TransformtoSlideNum != null && TransformtoSlideNum>0 ? ('TransformtoSlideNum="' + TransformtoSlideNum+'"'): '') +'></div>').css({
        position: 'absolute',
        top: '-35px',
        left: '-20px',
        width: '35px',
        height: '35px',
        border: '4px solid red',
        borderRadius: '50%'
    }).appendTo($span);

    //debugger;
    if (Buttons.DeleteButton == true) {
        // Create delete button inside the span
        $('<button class="delete_marker btn btn-danger">Delete</button>').css({
            position: 'absolute',
            top: '-5px',
            left: '100%',
            marginLeft: '5px',
            fontSize: '10px'
        }).appendTo($span).data('index', areas.length - 1);
    }

    // Create the area element
    $('<area />', {
        alt: "",
        title: "",
        href: "#",
        shape: "circle",
        name: "area" + areas.length,
        'data-tooltip': AreaLabel,
        coords: coords.join(","),
        class: "TransformtoSlide",
        transformtoslidenum: areas.length + 4 // Example slide number, can be changed
    }).appendTo('#' + Container + ' #plant_map');

    if (Buttons.DeleteButton == true) {
        $('#' + Container).find('#imagemap').on('click', '.delete_marker', function () {
            ////debugger;
            var index = $(this).data('index');
            areas.splice(index, 1);

            // Remove the marker, span, area and delete button
            $(this).parents('span').remove();
            $('#' + Container).find('#plant_map area').eq(index).remove();

            // Update area names and data-index
            $('#' + Container).find('#plant_map area').each(function (i) {
                $(this).attr('name', 'area' + (i + 1));
            });
            $('#' + Container).find('.delete_marker').each(function (i) {
                $(this).data('index', i);
            });

            //remove from table of clickable images
            var text = $(this).parents('#span_map_title').contents().filter(function () {
                return this.nodeType === 3; // Node type 3 is a text node
            }).text().trim();
            RemoveRowOfProfessionalClickableTable(Container, text)
        });
    }
}
function ChangeImageofClickableImageCntroller(Container, NewImageURL) {
    $('#'+Container).find('#imagemap').find('img').attr('src', NewImageURL);
}
function GetImageofClickableImageCntroller(Container) {
    var imageurl = $('#' + Container).find('#imagemap').find('#image_Professional_Clickable').attr('src');
    return imageurl;
}