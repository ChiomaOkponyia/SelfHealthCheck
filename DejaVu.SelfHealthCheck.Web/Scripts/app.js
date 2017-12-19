function reloadPage() {
    window.location.reload();
}
function escapeChars(text) {
    var x = String(text);
    var newString = x;
    if (x.substring(0, 0) === "{") {//starts with
        newString = x.substring(1, x.length - 1); //!-! will be the special characters used to escape {}
    }
    if (x.substring(x.length - 1, x.length - 1) === "}") { //ends with
        newString = newString.substring(0, newString.length - 2);
    }
    return String(newString);
}

$(function () {
    var globalConnection = $.connection("/monitorx");
    var resultConnection = $.connection("/resultx");
    globalConnection.logging = true;
    resultConnection.logging = true;
    globalConnection.start();
    resultConnection.start();
    $("#loadResults").click(function () {
        resultConnection.send($("#appIdToLoad").val()); // put value here
    });
    resultConnection.received(function (result) {
        $("#resultsLbl").html(result);
    });

    //CODE FOR TESTING
    //$("#btnSubmit").click(function () {
    //    globalConnection.send("rubbish");
    //});

    globalConnection.received(function (mem) {
        //Json serialized tree member is meant to be received here;   
        if (mem === "reloadPage") {
            $("#server").append("<li>RELOAD PAGE</li>");
            //reloadPage(); //reload page if new data is ready. It will make the next line unecessary
        }
        //if (mem === "messageReceived") {
        //    $("#server").append("<li>Health Message has been triggered</li>");
        //};
        if (String(mem).substring(0, 5) === "error:") {
            $("#error").append("<li>" + String(mem) + "</li>");
        };
        ////if (mem === "afterProcessing") {
        ////    $("#server").append("<li>Message has been processed in IF</li>");
        ////};
        var json = $.parseJSON(mem);
        var appIdString = String(json.AppID);
        //$('.test').text(appIdString);
        //$("#error").append("<li>" + String(mem) + "</li>");
        /*
        if (json.RowClientId === null) {
            $("#server").append("<li>UPDATE PAGE</li>");
            reloadPage();//reload page if the data is fresh so as to retrieve its client id's for signalR update
        }
        */
        //if (json.DateCellClientId === json.StatusCellClientId) reloadPage();

        //Blink a row when a new message is received
        $('.' + appIdString + ' rtlRBtm').animate({ "background-color": "#3D7EFF" }, 100);
        $('.' + appIdString + ' rtlRBtm').animate({ "background-color": "white" }, 500);
        $('.' + appIdString).animate({ "background-color": "#3D7EFF" }, 100);
        $('.' + appIdString).animate({ "background-color": "white" }, 500);


        //var date = new Date(json.DateChecked); //Convert the JSON date to a more pretty format
        $('.' + appIdString + 'Date').text(new Date(json.DateChecked).toLocaleString());
        $('.' + appIdString + 'Date rtlCL').text(new Date(json.DateChecked).toLocaleString());
        switch (json.Status) {
            case "Unknown":
                $('.' + appIdString + 'Status').css("background-color", "gray").text("Unknown");
                $('.' + appIdString + 'Status rtlCL').css("background-color", "gray").text("Unknown");
                break;
            case "Up":
                $('.' + appIdString + 'Status').css("background-color", "lawngreen").text("Up");
                $('.' + appIdString + 'Status rtlCL').css("background-color", "lawngreen").text("Up");
                break;
            case "PerformanceDegraded":
                $('.' + appIdString + 'Status').css("background-color", "yellow").text("PerformanceDegraded");
                $('.' + appIdString + 'Status rtlCL').css("background-color", "yellow").text("PerformanceDegraded");
                break;
            case "Down":
                $('.' + appIdString + 'Status').css("background-color", "crimson").text("Down");
                $('.' + appIdString + 'Status rtlCL').css("background-color", "crimson").text("Down");
                break;
            default:
                break;
        };

    });
});