var theAppID = "";
var theAppChildrenComponents;
var allApplications;
$(function () {

    $('#components').masonry({
        // options
        itemSelector: '.item',
        columnWidth: 20,
        isAnimated: true
    });

    var timer;
    var tabs = $('.nav-tabs:first');
    $('#control-sidebar-theme-demo-options-tab').remove();
    var ss = tabs.children().first().remove();
    tabs.children().first().addClass("active").click();
    $('#control-sidebar-home-tab').addClass("active");
    $('#control-sidebar-settings-tab').removeClass("active");

// Declare a proxy to reference the hub.        

//Function to load App Divs
var displayApp = function (appName, appID, aggStatus, lastUpdateTime) {
    var componentClass;
    if (aggStatus == 'Up') {
        componentClass = 'item up'
    }
    else if (aggStatus == 'Down') {
        componentClass = 'item down'
    }
    else {
        componentClass = 'item warning'
    }

    var mainHolder = document.createElement("a");
    $(mainHolder).attr("id", appID + "click");
    $(mainHolder).attr("href", "#");
    $(mainHolder).attr("onclick", "clearBreadCrumb();loadAppInfo('" + appID + "'" + ",'" + appName + "');return false;");

    //var mainHolderDiv = document.createElement("div");
    //$(mainHolderDiv).addClass("col-lg-3").appendTo(mainHolder);

    var panel = document.createElement("div");
    $(panel).addClass(componentClass).appendTo(mainHolder);
    $(panel).attr("id", appID);
    $(panel).attr("data-healthBeat", "reset");
    $(panel).attr("data-appName", appName);
    $(panel).attr("data-status", aggStatus);
    $(panel).attr("data-lastUpdate", lastUpdateTime);
    //var panelheading = document.createElement("div");
    //$(panelheading).addClass("inner").append("<p>" + appName + "</p>").appendTo(panel);
    $("#components").append(mainHolder);
}

var updateApplicationDiv = function (appID, aggStatus, time, fromHealthChecks) {
    var appIDStr = appID + "";
    var appIDStr = appIDStr.replace(new RegExp("\\.", "g"), "\\.");
    var componentClass, iconClass;
    if (aggStatus == 'Up') {
        componentClass = 'item up'
    }
    else if (aggStatus == 'Down') {
        componentClass = 'item down'
    }
    else {
        componentClass = 'item warning'
    }

    var appName = $("#" + appIDStr).attr("data-appName")
    $("#" + appIDStr).removeClass().addClass(componentClass);


    if (fromHealthChecks == true) {
        $("#" + appIDStr + "checktime").text(time)
        $("#" + appIDStr).attr("data-lastUpdate", time);
    }

    //Changes the position of the app as it's status changes
    var previousStatus = $("#" + appIDStr).attr("data-status");
    if (previousStatus == "Up" && aggStatus != "Up") {
        var copy = $("#" + appIDStr + "click");
        $("#" + appIDStr + "click").remove();
        $("#components").prepend(copy);
    }
    else if (previousStatus != "Up" && aggStatus == "Up") {
        var copy = $("#" + appIDStr + "click");
        $("#" + appIDStr + "click").remove();
        $("#components").append(copy);
    }
    $("#" + appIDStr).attr("data-status", aggStatus);

    componentService.healthMessage.server.sendChildStatusMessageToParentApps(appID, aggStatus);

}

function checkHealthBeats() {
    $.each(jQuery.parseJSON(allApplications), function (idx, app) {
        var status;
        var appIDStr = app.AppID + "";
        var appIDStr = appIDStr.replace(new RegExp("\\.", "g"), "\\.");
        var status = $("#" + appIDStr).attr("data-healthBeat");
        if (status == "reset") {
            updateApplicationDiv(app.AppID, 'Down', null, false, true);
            status = "Down";
            //if the present app being loaded receives no heart beat,then it should clear it's list of check
            if (theAppID == app.AppID) {
                $("#checkList").empty();
            }
        }
        else {
            status = "Up";
        }
        //send heart beat
        healthMessage.server.updateHeartBeat(app.AppID, status);
        var status = $("#" + appIDStr).attr("data-healthBeat", "reset");
    });
}

function addCheck(appID, status, date, title) {
    if (status == 'Up') {
        var iconClass = "menu-icon fa fa-check bg-green";
    }
    else if (status == 'PerformanceDegraded') {
        var iconClass = "menu-icon fa fa-warning bg-yellow";
    }

    else {
        var iconClass = "menu-icon fa fa-warning bg-red";
    }

    var checkID = appID.replace(new RegExp("\\.", "g"), "") + title.replace(new RegExp(" ", "g"), "-");
    var checkItemHtml = "<li id ='" + checkID + "'><a href='javascript::;'><i  class='" + iconClass + "'></i><div class='menu-info'><h4 class='control-sidebar-subheading'>" + title + "</h4><p><b>Last Check: </b>" + date + "</p></div> </a> </li>";
    if ($('#checkList').find("#" + checkID).length) {
        // found!
        $("#" + checkID).replaceWith(checkItemHtml)
    }
    else {
        $("#checkList").append(checkItemHtml);
    }
}
//////////////////////////
componentService.healthMessage.client.addHealthMessage = function (appID, title, date, time, status, timeElapsed, addInfo, aggStatus, failedMsgs) {
    console.log('received health message AppID:' + appID + 'Title:' + title + 'Date:' + date + 'Time:' + time + 'Status:' + status + 'TimeElapsed' + timeElapsed + 'Aggreated Status:' + aggStatus + 'FailedMessages:' + failedMsgs);
    updateApplicationDiv(appID, aggStatus, date, true);
    if (theAppID != "") {
        //A app has been selected
        if (appID == theAppID) {
            addCheck(appID, status, date, title);
            //Check the number of succesfull Checks
            var up = 0, down = 0, degraded = 0;
            $('#checkList').children().each(function (index, list) {
                var item = $(list).children().first().children().first();
                if (item.hasClass('bg-green')) {
                    up++;
                }
                else if (item.hasClass('bg-red')) {
                    down++;
                }
                else {
                    degraded++;
                }

                if (down > 0) {
                    checkIconClass = 'fa fa-check-square-o rediconcolor'
                }
                else if (degraded > 0) {
                    checkIconClass = 'fa fa-check-square-o yellowiconcolor'
                }
                else {
                    checkIconClass = 'fa fa-check-square-o greeniconcolor'
                }
                $("#checksIcon").removeClass().addClass(checkIconClass);

            });
        }
        else {
            theAppChildrenComponents.forEach(function (childApp) {
                if (childApp.AppID == appID) {

                    if (aggStatus == 'Up') {
                        var iconClass = "menu-icon fa fa-check bg-green";
                    }
                    else if (aggStatus == 'Down') {
                        var iconClass = "menu-icon fa fa-warning bg-red";
                    }
                    else {
                        var iconClass = "menu-icon fa fa-warning bg-yellow";
                    }
                    var appIDStr = appID.replace(new RegExp("\\.", "g"), "\\.");
                    $("#" + appIDStr + "lastupdate").html("<b>Last Update: </b>" + date);
                    $("#" + appIDStr + "status").removeClass().addClass(iconClass);
                }
            });
        }
    }
};

componentService.healthMessage.client.reportAppStatus = function (appID, aggStatus) {
    console.log('App Status' + 'AppId:' + appID + 'STATUS' + aggStatus);
    updateApplicationDiv(appID, aggStatus, null, false);
};

componentService.healthMessage.client.loadAllApllications = function (data) {
    console.log('All Seat' + data);
    allApplications = data;
    //$.each(jQuery.parseJSON(data), function (idx, app) {
    //    displayApp(app.AppName, app.AppID, app.TheStatus, app.TheLastUpdateTime);
    //});
    timer = setInterval(checkHealthBeats, 7000);//initiates a timer that starts checking every 7s for received health Beats.
};

componentService.healthMessage.client.sendHealthBeat = function (appID, date) {
    var appIDStr = appID + "";
    var appIDStr = appIDStr.replace(new RegExp("\\.", "g"), "\\.");
    $("#" + appIDStr).attr("data-healthBeat", "updated")
};

$.connection.hub.start().done(function () {
 healthMessage.server.loadAllApplications(); //makes a call to the server to get all applications
});
    });

    function addDependentApp(appID, parentAppName) {

        var appIDStr = appID.replace(new RegExp("\\.", "g"), "\\.");
        var appName = $("#" + appIDStr).attr("data-appName")
        var appStatus = $("#" + appIDStr).attr("data-status")
        var appLastUpdate = $("#" + appIDStr).attr("data-lastUpdate")
        if (appStatus == 'Up') {
            var iconClass = "menu-icon fa fa-check bg-green";
        }
        else if (appStatus == 'Down') {
            var iconClass = "menu-icon fa fa-warning bg-red";
        }
        else {
            var iconClass = "menu-icon fa fa-warning bg-yellow";
        }
        var clickFunc = "";
        var checkItemHtml = "<li><a id='" + appID + "appClick' href='javascript::;'><i id='" + appID + "status' class='" + iconClass + "'></i><div class='menu-info'><h4 class='control-sidebar-subheading'>" + appName + "</h4><p id='" + appID + "lastupdate'><b>Last Update: </b>" + appLastUpdate + "</p></div> </a> </li>";
        $("#dependentAppsList").append(checkItemHtml);
        $("#" + appIDStr + "appClick").attr("onclick", "loadAppInfo('" + appID + "'" + ",'" + appName + "');return false;");
    }

    function loadAppInfo(appID, appName) {
        theAppID = appID;
        var tabs = $('.nav-tabs:first');
        tabs.children().first().addClass("active").click();
        //tabs.children().first().removeClass("active");
        tabs.children().last().removeClass("active");
        $('#control-sidebar-settings-tab').removeClass("active");
        $('#control-sidebar-activity-tab').removeClass("active");
        $('#control-sidebar-home-tab').addClass("active");
        $("#tabAppName").text(appName);
        $('#checkList').empty();
        $('#dependentAppsList').empty();

        //Sets the color of the checks icon back to black
        $("#checksIcon").removeClass().addClass('fa fa-check-square-o');
        $.each(jQuery.parseJSON(allApplications), function (idx, app) {
            if (appID == app.AppID) {
                theAppChildrenComponents = app.ChildrenComponents;
                app.ChildrenComponents.forEach(function (childApp) {
                    addDependentApp(childApp.AppID, appName);
                });
            }
        });

        var appIDStr = appID.replace(new RegExp("\\.", "g"), "\\.");
        if ($('#breadcrumb').find("#" + appIDStr + "crumb").length) {
            // found!
            $("#" + appIDStr + "appClick").attr("onclick", "loadAppInfo('" + appID + "'" + ",'" + appName + "');return false;");

        }
        else {
            $("#breadcrumb").append("<li id='" + appID + "crumb' data-name='" + appName + "' data-id='" + appID + "' style='font-size:11px' class='active'>" + appName + "</li>");
        }

        var active = $("#" + appIDStr + "crumb");
        active.attr("style", "font-size:11px;display:inline");
        active.addClass("active");

        var parent = active.prev();
        parent.removeClass();
        parent.attr("style", "font-size:11px;display:inline");
        var parentName = parent.attr("data-name");
        var parentID = parent.attr("data-id");
        if (parentID != undefined) {
            var parentIDSStr = parentID.replace(new RegExp("\\.", "g"), "\\.");
            parent.html("<a href='#' id='" + parentID + "crumbclick');return false;'>" + parentName + "</a>")
            $("#" + parentIDSStr + "crumbclick").attr("onclick", "loadAppInfo('" + parentID + "'" + ",'" + parentName + "');return false;");
        }

        active.nextAll().removeClass();
        active.nextAll().attr("style", "font-size:11px;display:none");
        parent.prevAll().removeClass;
        parent.prevAll().attr("style", "font-size:11px;display:none");
    }

    function clearBreadCrumb()
    { $('#breadcrumb').empty() }