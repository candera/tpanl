function post() {
    //alert("Got here");
    req = new XMLHttpRequest();
    req.open('POST', "notify", true);
    req.send(); 
}