var arrowPaths = [];

function DisplayTransportProcess(ControllerParameters, Action) {

    var MainImagePath = ControllerParameters.MainImagePath;
    var Button1ImagePath = ControllerParameters.Button1ImagePath;
    var Button1Title = ControllerParameters.Button1Title;

    var Button2ImagePath = ControllerParameters.Button2ImagePath;
    var Button2Title = ControllerParameters.Button2Title;

    //////debugger;
    var Arrows = ControllerParameters.Arrows;
    if (Arrows != undefined)
        for (var i = 0; i < Arrows.length; i++) {
            if (Arrows[i].color == undefined)
                Arrows[i].color = "#cccccc";
        }

    var startPoints = [];
    var endPoints = [];
    if (Arrows != null && Arrows.length > 0)
        for (var i = 0; i < Arrows.length; i++) {
            // Split the path string by spaces
            var pathParts = Arrows[i].position.split(' ');
            startPoints.push(new fabric.Point(parseFloat(pathParts[1]), parseFloat(pathParts[2])));//start point

            endPoints.push(new fabric.Point(parseFloat(pathParts[5]), parseFloat(pathParts[6]))); // End point
        }


    // Define the quadratic curve definitions
    var curveDefinitions = Arrows;

    var randomint = Math.floor(Math.random() * (999999 - 1)) + 1;
    var canvasId = 'canvas_' + randomint;
    //html
    var $leftButton = $('<div class="text-center col-lg-4 col-lg-4 col-lg-4' + (Action == 'edit' ? ' hide ' : '') + '" style="margin-top:50px;"><label class="pull-right col-lg-12">' + Button1Title + '</label><img class="pull-right col-lg-12" id="img_TransportProcess_Button1" src="" alt="" style="padding: 0 100px 0 100px; height: auto;display: inline-block;"/></div>');
    var $rightButton = $('<div class="text-center col-lg-4 col-lg-4 col-lg-4' + (Action == 'edit' ? ' hide ' : '') + '" style="margin-top:50px;"><label class="pull-left col-lg-12">' + Button2Title + '</label><img class="pull-left col-lg-12"  id="img_TransportProcess_Button2" src="" alt="" style="padding: 0 100px 0 100px; height: auto;display: inline-block;"/></div>');
    var $canvas = $('<div class="text-center col-lg-4 col-lg-4 col-lg-4"><canvas id="' + canvasId + '" width="400" height="430" style="display: block;"></canvas></div>');

    //debugger;
    return {
        'canvasId': canvasId,
        'leftButton': $leftButton,
        'canvas': $canvas,
        'rightButton': $rightButton,
        'MainImagePath': MainImagePath,
        'Button1ImagePath': Button1ImagePath,
        'Button1CorrespondingArrowsColor': ControllerParameters.Button1CorrespondingArrowsColor /*!= undefined ? ControllerParameters.Button1CorrespondingArrowsColor : "#FF0000"*/,
        'Button2ImagePath': Button2ImagePath,
        'Button2CorrespondingArrowsColor': ControllerParameters.Button2CorrespondingArrowsColor /*!= undefined ? ControllerParameters.Button2CorrespondingArrowsColor : "#0000FF"*/,
        'startPoints': startPoints,
        'endPoints': endPoints,
        'curveDefinitions': curveDefinitions
    };
}

var CanvasList = [];
//action: display or edit
function InitiateDisplayTransportProcess(Action, Container, NormalizedParameters) {

    CanvasId = NormalizedParameters.canvasId;
    BackgroundImage = NormalizedParameters.MainImagePath;
    LeftImage = NormalizedParameters.Button1ImagePath;
    LeftImageCorrespondingArrowsColor = NormalizedParameters.Button1CorrespondingArrowsColor != undefined ? NormalizedParameters.Button1CorrespondingArrowsColor : $("#" + Container).find('#input_color_picker_Left').val() /*'#FF0000'*/;
    RightImage = NormalizedParameters.Button2ImagePath;
    RightImageCorrespondingArrowsColor = NormalizedParameters.Button2CorrespondingArrowsColor != undefined ? NormalizedParameters.Button2CorrespondingArrowsColor : $("#" + Container).find('#input_color_picker_Right').val() /*'#0000FF'*/;
    StartPoints = NormalizedParameters.startPoints;
    EndPoints = NormalizedParameters.endPoints;
    CurveDefinitions = NormalizedParameters.curveDefinitions;


    ////debugger;

    //Div1Image = 'url(' + Div1Image +')';
    //Set the background of the right button
    $('#' + Container).find('#img_TransportProcess_Button1').attr('src', LeftImage);

    //Div2Image = 'url(' + Div2Image + ')';
    //Set the background of the left button
    $('#' + Container).find('#img_TransportProcess_Button2').attr('src', RightImage);

    ////////debugger;
    // Create a Fabric.js canvas
    var canvas = new fabric.Canvas(CanvasId);
    CanvasList.push({ id: CanvasId, canvas: canvas });

    canvas.selection = false;
    canvas.renderAll();

    // Load the background image
    fabric.Image.fromURL(BackgroundImage, function (img) {
        if (canvas != undefined && canvas != null) {
            canvas.setBackgroundImage(img, canvas.renderAll.bind(canvas), {
                scaleX: canvas.width / img.width,
                scaleY: canvas.height / img.height
            });
        }
    });

    AddArrows(Action, Container, canvas, StartPoints, EndPoints, CurveDefinitions, LeftImageCorrespondingArrowsColor, RightImageCorrespondingArrowsColor);



    if (Action == 'edit') {
        //.......................mouse click.......................
        var startPoint = null;
        var endPoint = null;
        var curveDefinitions = [];
        var pointMarkers = [];
        ////////debugger;



        canvas.on('mouse:down', function (options) {
            var target = options.target;

            // Check if the target is not a delete button
            if (target && target.class === 'btn_deletearrow') {
                return;
            }

            var pointer = canvas.getPointer(options.e);
            if (!endPoint) {
                endPoint = new fabric.Point(pointer.x, pointer.y);
                var endMarker = new fabric.Circle({
                    left: endPoint.x,
                    top: endPoint.y,
                    radius: 3,
                    fill: 'red',
                    selectable: false
                });
                canvas.add(endMarker);
                pointMarkers.push(endMarker);
                console.log('End Point:', endPoint);
            } else {
                startPoint = new fabric.Point(pointer.x, pointer.y);
                var startMarker = new fabric.Circle({
                    left: startPoint.x,
                    top: startPoint.y,
                    radius: 3,
                    fill: 'green',
                    selectable: false
                });
                canvas.add(startMarker);
                pointMarkers.push(startMarker);
                console.log('Start Point:', startPoint);

                var arrowImage, _LeftImageCorrespondingArrowsColor, _RightImageCorrespondingArrowsColor;
                if (confirm('Will the arrow flash when clicking on the left image? \n - Yes or Ok: "Left Image" \n - No or Cancel: "Right Image"')) {
                    ArrowCorrespondingImage = "left";
                    //maybe the user changed the color in UI
                    _LeftImageCorrespondingArrowsColor = $("#" + Container).find('#input_color_picker_Left').val();
                } else {
                    ArrowCorrespondingImage = "right";
                    //maybe the user changed the color in UI
                    _RightImageCorrespondingArrowsColor = $("#" + Container).find('#input_color_picker_Right').val();
                }

                var curveDefinition = {
                    position: `M ${startPoint.x} ${startPoint.y} Q ${(startPoint.x + endPoint.x) / 2} ${(startPoint.y + endPoint.y) / 2 - 50} ${endPoint.x} ${endPoint.y}`,
                    color: ArrowCorrespondingImage == 'left' ? _LeftImageCorrespondingArrowsColor : _RightImageCorrespondingArrowsColor,
                    ArrowCorrespondingImage: ArrowCorrespondingImage
                };
                curveDefinitions.push(curveDefinition);

                AddArrows(
                    Action,
                    Container,
                    canvas,
                    [startPoint], [endPoint], [curveDefinition],
                    _LeftImageCorrespondingArrowsColor,
                    _RightImageCorrespondingArrowsColor,
                    ArrowCorrespondingImage
                );
                startPoint = null; // reset for the next arrow
                endPoint = null;   // reset for the next arrow

                // Remove point markers after drawing
                pointMarkers.forEach(marker => canvas.remove(marker));
                pointMarkers = [];
            }
        });
    }
}
function SetCanvasBackgroundImage(Container, BackgroundImage) {
    ////////debugger;
    var _canvas = $("#" + Container).find("canvas").get(0);

    var canvas;
    for (var i = 0; i < CanvasList.length; i++) {
        if (_canvas != undefined && _canvas != null)
            if (CanvasList[i].id == _canvas.id) {
                canvas = CanvasList[i].canvas;
            }
    }

    fabric.Image.fromURL(BackgroundImage, function (img) {
        if (canvas != undefined && canvas != null) {
            canvas.setBackgroundImage(img, canvas.renderAll.bind(canvas), {
                scaleX: canvas.width / img.width,
                scaleY: canvas.height / img.height
            });
        }
        else {
            //////debugger;
            var NormalizedParameters = DisplayTransportProcess({ MainImagePath: BackgroundImage }, 'edit');
            // Append elements to container
            $('#' + Container).find("#div_TransportProcess_MainImage").append(
                NormalizedParameters.leftButton,
                NormalizedParameters.canvas,
                NormalizedParameters.rightButton);
            InitiateDisplayTransportProcess(
                'edit',
                Container,
                {
                    canvasId: NormalizedParameters.canvasId,
                    MainImagePath: NormalizedParameters.MainImagePath,
                    Button1ImagePath: NormalizedParameters.Button1ImagePath,
                    Button1CorrespondingArrowsColor: $("#" + Container).find('#input_color_picker_Left').val(),
                    //NormalizedParameters.Button1CorrespondingArrowsColor,
                    Button2ImagePath: NormalizedParameters.Button2ImagePath,
                    //NormalizedParameters.Button2CorrespondingArrowsColor,
                    Button2CorrespondingArrowsColor: $("#" + Container).find('#input_color_picker_Right').val(),
                    startPoints: NormalizedParameters.startPoints,
                    endPoints: NormalizedParameters.endPoints,
                    CurveDefinitions: NormalizedParameters.curveDefinitions
                }
            );



            //debugger;
            $("#" + Container).find('#input_color_picker_Left').unbind().on('change', function (event) {
                //new color of corresponding arrows of the left image
                const newColorofCorrespondingArrowsofLeftImage = event.target.value;
                // color of corresponding arrows of the right image
                const ColorofCorrespondingArrowsofRightImage = $("#" + Container).find("#div_displaying_process").find("#input_color_picker_Right").val();

                ChangeArrowColor(Container, NormalizedParameters.canvasId, 'left', newColorofCorrespondingArrowsofLeftImage);

            });

            $("#" + Container).find('#input_color_picker_Right').unbind().on('change', function (event) {
                //debugger;
                //new color of corresponding arrows of the left image
                const newColorofCorrespondingArrowsofRightImage = event.target.value;
                // color of corresponding arrows of the right image
                const ColorofCorrespondingArrowsofLeftImage = $("#" + Container).find("#div_displaying_process").find("#input_color_picker_Left").val();

                ChangeArrowColor(Container, NormalizedParameters.canvasId, 'right', newColorofCorrespondingArrowsofRightImage);


            });


        }
    });
}
// Function to toggle blinking effect
function toggleBlinking() {
    curve.set('visible', !curve.visible);
    canvas.requestRenderAll();

    arrowHeadStart.set('visible', !arrowHeadStart.visible);
    arrowHeadStart.requestRenderAll();

}
//Add Arrows
function AddArrows(Action, Container, canvas, startPoints, endPoints, curveDefinitions, Image1ArrowsColor, Image2ArrowsColor) {
    //debugger;
    // Define the start point of the arrow's path
    //var startPoint = new fabric.Point(50, 50);
    if (!(curveDefinitions != null && curveDefinitions.length > 0))
        return;
    var Image1arrows = [], Image2arrows = [];
    for (var i = 0; i < curveDefinitions.length; i++) {
        //////debugger;
        var curveDefinition = curveDefinitions[i];
        var startPoint = startPoints[i];
        var endPoint = endPoints[i];

        ////debugger;
        // Create a quadratic curve representing the arrow's path
        var curve = new fabric.Path(curveDefinition.position, {
            fill: '',
            stroke: curveDefinition.color,
            strokeWidth: 3, // Increase the strokeWidth to make the curve bolder
            selectable: false, // Disable selection and dragging
            ArrowCorrespondingImage: curveDefinition.ArrowCorrespondingImage
        });

        var angle = Math.atan2(endPoint.y - startPoint.y, endPoint.x - startPoint.x) * (180 / Math.PI);


        // Create arrowhead shapes at the start and end points
        var arrowHeadStart = new fabric.Triangle({
            width: 15,
            height: 15,
            fill: curveDefinition.color,
            //right: startPoint.x,
            left: startPoint.x + 10,
            top: startPoint.y - 10,
            angle: 80, // the angle of the head of arrow
            selectable: false, // Disable selection and dragging
            ArrowCorrespondingImage: curveDefinition.ArrowCorrespondingImage

        });


        if (Action == 'edit') {
            var deleteButton = new fabric.Rect({
                width: 20,
                height: 20,
                fill: 'gray',
                selectable: true,
                evented: true,
                opacity: 0.7,
            });

            var deleteText = new fabric.Text('X', {
                left: 10,
                top: 10,
                fontSize: 12,
                fill: curveDefinition.color,
                selectable: false,
                evented: false,
                originX: 'center',
                originY: 'center'
            });

            var deleteButtonGroup = new fabric.Group([deleteButton, deleteText,], {
                left: (startPoint.x + endPoint.x) / 2 - 10,
                top: (startPoint.y + endPoint.y) / 2 - 10,
                selectable: true,
                evented: true
            });
            var randomNumber = Math.floor(Math.random() * 100);
            deleteButtonGroup.set('id', 'btn_deletearrowId_' + randomNumber);
            deleteButtonGroup.set({
                id: 'btn_deletearrowId_' + randomNumber,
                class: 'btn_deletearrow',
                associatedCurve: curve,
                associatedArrowHeadStart: arrowHeadStart,
            });

            deleteButtonGroup.on('mousedown', function () {
                ////////debugger;
                var group = this;
                canvas.remove(group.get('associatedCurve'),
                    group.get('associatedArrowHeadStart'),
                    group);

                //update arrowlist
                if (Action == 'edit')
                    arrowPaths = getArrowPathsFromHTML(Container, canvas);
            });


            // Add objects to the canvas
            canvas.add(curve, arrowHeadStart, deleteButtonGroup);
            if (curveDefinition.color == Image1ArrowsColor)
                Image1arrows.push({ "curve": curve, "arrowHeadStart": arrowHeadStart, "deleteButton": deleteButtonGroup });
            else
                if (curveDefinition.color == Image2ArrowsColor)
                    Image2arrows.push({ "curve": curve, "arrowHeadStart": arrowHeadStart, "deleteButton": deleteButtonGroup });

        }
        else {
            canvas.add(curve, arrowHeadStart);
            if (curveDefinition.color == Image1ArrowsColor)
                Image1arrows.push({ "curve": curve, "arrowHeadStart": arrowHeadStart });
            else
                if (curveDefinition.color == Image2ArrowsColor)
                    Image2arrows.push({ "curve": curve, "arrowHeadStart": arrowHeadStart });

        }

        //update arrowlist
        if (Action == 'edit')
            arrowPaths = getArrowPathsFromHTML(Container, canvas);
    }

    var BlinkingIntervalIds = [];

    $('#' + Container).find('#img_TransportProcess_Button1').on('click', function () {
        //stop blinking
        for (var i = 0; i < BlinkingIntervalIds.length; i++) {
            clearInterval(BlinkingIntervalIds[i]);
        }
        //display Image2arrows
        for (var i = 0; i < Image2arrows.length; i++) {
            toggleBlinking(canvas, Image2arrows[i].curve, Image2arrows[i].arrowHeadStart, true);
        }

        for (var i = 0; i < Image1arrows.length; i++) {
            (function (index) {
                var intervalId = setInterval(function () {
                    toggleBlinking(canvas, Image1arrows[index].curve, Image1arrows[index].arrowHeadStart);
                }, 500);

                BlinkingIntervalIds.push(intervalId);

            })(i); // Pass the current value of i to the IIFE
        }
    });

    $('#' + Container).find('#img_TransportProcess_Button2').on('click', function () {
        //stop blinking
        for (var i = 0; i < BlinkingIntervalIds.length; i++) {
            clearInterval(BlinkingIntervalIds[i]);
        }
        //display Image1arrows
        for (var i = 0; i < Image1arrows.length; i++) {
            toggleBlinking(canvas, Image1arrows[i].curve, Image1arrows[i].arrowHeadStart, true);
        }

        for (var i = 0; i < Image2arrows.length; i++) {
            (function (index) {
                var intervalId = setInterval(function () {
                    toggleBlinking(canvas, Image2arrows[index].curve, Image2arrows[index].arrowHeadStart);
                }, 500);

                BlinkingIntervalIds.push(intervalId);

            })(i); // Pass the current value of i to the IIFE
        }
    });

}
function getArrowPaths(container) {
    ////////debugger;
    var paths = arrowPaths.filter(function (item) {
        return item.container === container;
    });

    paths.forEach(function (item) {
        delete item.container;
    });

    return paths;
}
var arrowPaths = [];
function getArrowPathsFromHTML(container, canvas) {
    var arrowPaths = [];
    var seenPaths = new Set();

    // Iterate over all objects on the canvas
    canvas.getObjects().forEach(function (obj) {
        // Check if the object is a fabric.Path (or your custom class for arrows)
        if (obj instanceof fabric.Path) {
            // Extract the path data
            var pathData = obj.get('path');

            var ArrowCorrespondingImage = obj.get('ArrowCorrespondingImage');

            // Extract the color (assuming the color is stored in the 'stroke' property)
            var color = obj.get('stroke') || obj.get('fill'); // Change as needed based on your implementation

            // Format path data
            var formattedPath = pathData.map(function (segment) {
                return segment.join(' ');
            }).join(' ');

            // Check if the path has already been added
            if (!seenPaths.has(formattedPath)) {
                //////debugger;
                // Push formatted path data and color to the array
                arrowPaths.push({
                    container: container,
                    position: formattedPath,
                    color: color,
                    ArrowCorrespondingImage: ArrowCorrespondingImage
                });

                // Add the path to the set of seen paths
                seenPaths.add(formattedPath);
            }
        }
    });

    return arrowPaths;
}


function ChangeArrowColor(Container, CanvasId, ArrowCorrespondingImage, NewColor) {
    var canvas;
    for (var i = 0; i < CanvasList.length; i++) {
        if (CanvasList[i].id == CanvasId) {
            canvas = CanvasList[i].canvas;
        }
    }
    //debugger;
    canvas.getObjects().forEach(function (obj) {
        if (obj.type === 'path' || obj.type === 'triangle') {
            if (obj.ArrowCorrespondingImage === ArrowCorrespondingImage) {
                //changing color of arrows of left image
                if (obj.stroke)
                    obj.set('stroke', NewColor);
                canvas.renderAll();
                if (obj.fill)
                    obj.set('fill', NewColor);
                canvas.renderAll();
            }
        }
    });
    canvas.renderAll();

    arrowPaths = getArrowPathsFromHTML(Container, canvas);
}

// Function to toggle blinking effect
function toggleBlinking(canvas, curve, arrowHeadStart, visibleStatus) {
    // Create a Fabric.js canvas
    curve.set('visible', visibleStatus != null ? visibleStatus : !curve.visible);
    arrowHeadStart.set('visible', visibleStatus != null ? visibleStatus : !arrowHeadStart.visible);

    canvas.requestRenderAll();

}


