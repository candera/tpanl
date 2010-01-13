var _regions = 
{
    "targettype1" : [63, 12, 42, 40],
    "targettype2" : [112, 12, 43, 39],
    "targettype3" : [161, 13, 42, 38],
    "targettypeother" : [212, 12, 44, 40]
    
    // TODO: More
    // TODO: Dynamically generate these. 
}

var _req = new XMLHttpRequest();
var _touchX;
var _touchY;
var _regionName;

function post() {
    if (_regionName != null) {
        _req.open("POST", "notify?region=" + _regionName, true);
        _req.send(null);
    }
}

function inbounds(bounds, x, y)
{
    var left = bounds[0];
    var top = bounds[1];
    var width = bounds[2];
    var height = bounds[3]; 
    var right = left + width; 
    var bottom = top + height; 
    
    return (x >= left) && (x <= right) &&
        (y >= top) && (y <= bottom);     
}

function findregion(x, y)
{
    for (var region in _regions)
    {
        var bounds = _regions[region]; 
        
        if (inbounds(bounds, x, y))
        {
            return { Name: region, Bounds: bounds };
        }
    }
    
    return null; 
}

function down(x, y) {
    var panel = document.getElementById("panel");
    var panelTop = panel.offsetTop;
    var panelLeft = panel.offsetLeft;

    var region = findregion(x - panelLeft, y - panelTop);

    if (region != null) {
        _regionName = region.Name;
        var box = document.getElementById("highlight-box");
        box.style.left = region.Bounds[0] + panelLeft;
        box.style.top = region.Bounds[1] + panelTop;
        box.style.width = region.Bounds[2];
        box.style.height = region.Bounds[3];
        box.style.display = "block";
    }
    else {
        _regionName = null;
    }
}

function up() {
    var box = document.getElementById("highlight-box");
    box.style.display = "none";
    post();
}

function ontouchstart(event) {
    down(event.targetTouches[0].pageX, event.targetTouches[0].pageY);        
}

function ontouchend(event) {
    up(); 
}

function ontouchmove(event) {
    event.preventDefault(); // Don't let Safari scroll
}

function onmousedown(event) {
    down(event.pageX, event.pageY);
}

function onmouseup(event) {
    up(event.pageX, event.pageY); 
}

function initialize() {
    document.addEventListener('touchstart', ontouchstart);
    document.addEventListener('touchend', ontouchend);
    document.addEventListener('touchmove', ontouchmove);

    var panel = document.getElementById("panel");

    panel.onmousedown = onmousedown; 
    panel.onmouseup = onmouseup; 
    panel.onmousemove = onmousemove; 

}


window.onload = initialize; 
