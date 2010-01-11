var _req = new XMLHttpRequest();
var _touchX;
var _touchY;

function post() {
    _req.open("POST", "notify?x=" + _touchX + "&y=" + _touchY, true);
    _req.send(null);
}

function ontouchstart(event) {
    _touchX = event.targetTouches[0].pageX;
    _touchY = event.targetTouches[0].pageY;     
}

function ontouchend(event) {
    post();
}

function ontouchmove(event) {
    event.preventDefault(); // Don't let Safari scroll
}

function initialize() {
    document.addEventListener('touchstart', ontouchstart);
    document.addEventListener('touchend', ontouchend);
    document.addEventListener('touchmove', ontouchmove);
}


window.onload = initialize; 
