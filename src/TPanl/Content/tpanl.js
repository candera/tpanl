var _regions = 
{
    "targettype1"       : [63, 12, 42, 40],
    "targettype2"       : [112, 12, 43, 39],
    "targettype3"       : [161, 13, 42, 38],
    "targettypeother"   : [212, 12, 44, 40],
    "wingman1"          : [36, 60, 42, 41],
    "wingman2"          : [85, 61, 44, 41],
    "wingman3"          : [133, 61, 44, 41],
    "wingman4"          : [181, 60, 44, 41],
    "wingmanall"        : [231, 59, 45, 41],
    "clear"             : [105, 108, 45, 41],
    "DLIngress"         : [162, 108, 45, 41],
    "SendMem"           : [215, 108, 45, 42]
    
    // TODO: Dynamically generate these. 
}

var _req = new XMLHttpRequest();
var _touchX;
var _touchY;
var _regionName;

function log(message) {
    var d = document.getElementById("diagnostics");
    d.innerHTML = message; 
}

function post() {
    if (_regionName != null) {
        _req.open("POST", "notify?region=" + _regionName, true);
        _req.send(null);
    }
}

function inbounds(bounds, coords)
{
    var left = bounds[0];
    var top = bounds[1];
    var width = bounds[2];
    var height = bounds[3]; 
    var right = left + width; 
    var bottom = top + height; 
    
    return (coords.x >= left) && (coords.x <= right) &&
        (coords.y >= top) && (coords.y <= bottom);     
}


function findregion(coords)
{
    for (var region in _regions)
    {
        var bounds = _regions[region];
        
        if (inbounds(bounds, coords))
        {
            return { Name: region, Bounds: bounds };
        }
    }
    
    return null; 
}

function getGlobalBounds(panelBounds) {
    var panel = document.getElementById("panel");
    var panelTop = panel.offsetTop;
    var panelLeft = panel.offsetLeft;

    var globalBounds = new Array(4);
    globalBounds[0] = panelBounds[0] + panelLeft;
    globalBounds[1] = panelBounds[1] + panelTop;
    globalBounds[2] = panelBounds[2] - 6; // Offset accounts for border thickness
    globalBounds[3] = panelBounds[3] - 6; // Offset accounts for border thickness
    return globalBounds;
}

function showHighlight(bounds) {
    var box = document.getElementById("highlight-box");
    box.style.left = bounds[0];
    box.style.top = bounds[1];
    box.style.width = bounds[2];
    box.style.height = bounds[3];
    box.style.display = "block";
}

function down(x, y) {
    log("down at " + x.toString() + ", " + y.toString()); 
    var region = findregion(getPanelCoords(x, y));

    if (region != null) {
        _regionName = region.Name;
        var globalBounds = getGlobalBounds(region.Bounds);
        showHighlight(globalBounds); 
    }
    else {
        _regionName = null;
    }
}

function removeHighlight() {
    var box = document.getElementById("highlight-box");
    box.style.display = "none";
}

function up() {
    log("up"); 
    if (_regionName != null) {
        removeHighlight();
        post();
    }
}

function ontouchstart(event) {
    down(event.targetTouches[0].pageX, event.targetTouches[0].pageY);        
}

function ontouchend(event) {
    up(); 
}

function coords(x, y) {
    this.x = x;
    this.y = y; 
}

function getPanelCoords(x, y) {
    var panel = document.getElementById("panel");
    var panelTop = panel.offsetTop;
    var panelLeft = panel.offsetLeft;

    return new coords(x - panelLeft, y - panelTop);
}

function move(x, y) {
    log("move to " + x.toString() + ", " + y.toString());

    // If we move out of the current region, cancel the
    // press that would have occurred on touch end
    if (_regionName != null) {
        var bounds = _regions[_regionName];
        var coords = getPanelCoords(x, y); 

        if (!inbounds(bounds, coords)) { 
            _regionName = null;
            removeHighlight(); 
        }
    }
}

function ontouchmove(event) {
//    event.preventDefault(); 
    move(event.targetTouches[0].pageX, event.targetTouches[0].pageY);
}

function onmousedown(event) {
    down(event.pageX, event.pageY);
}

function onmouseup(event) {
    up(event.pageX, event.pageY);
}

function onmousemove(event) {
    move(event.pageX, event.pageY); 
}

function initialize() {
    var panel = document.getElementById("panel");

    panel.addEventListener('touchstart', ontouchstart);
    panel.addEventListener('touchend', ontouchend);
    panel.addEventListener('touchmove', ontouchmove);


//    panel.onmousedown = onmousedown; 
//    panel.onmouseup = onmouseup; 
//    panel.onmousemove = onmousemove; 

}


window.onload = initialize; 
