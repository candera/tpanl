var _req = new XMLHttpRequest();

function post() {
    var d = new Date();
    //alert("before open");
    _req.open("POST", "notify?hh=" + d.getHours() + "&mm=" + d.getMinutes() + "&ss=" + d.getSeconds() + "." + d.getMilliseconds(), true);
    //alert("Before send");
    _req.send(null);
    //alert("after send");
}