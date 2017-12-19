﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModifiedMonitor.aspx.cs" Inherits="DejaVu.SelfHealthCheck.WebMonitor.WebPages.ModifiedMonitor" %>

<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxMenu" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxTreeList.v13.1, Version=13.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxTreeList" TagPrefix="dx" %>

<%@ Register TagPrefix="dx" Namespace="DevExpress.Web.ASPxEditors" Assembly="DevExpress.Web.v13.1, Version=13.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <dx:ASPxTreeList ID="ASPxTreeList1" runat="server" Width="100%"
            OnVirtualModeCreateChildren="ASPxTreeList1_VirtualModeCreateChildren" 
            OnVirtualModeNodeCreating="ASPxTreeList1_VirtualModeNodeCreating"
            OnVirtualModeNodeCreated="ASPxTreeList1_VirtualModeNodeCreated"
            OnCustomDataCallback="ASPxTreeList1_CustomDataCallback" OnHtmlRowPrepared="ASPxTreeList1_HtmlRowPrepared"
             OnHtmlDataCellPrepared="ASPxTreeList1_HtmlDataCellPrepared">
            <Columns>
                <dx:TreeListDataColumn Caption="Name" FieldName="AppName" VisibleIndex="0" />
                <dx:TreeListDataColumn FieldName="AppID" VisibleIndex="1" />
                <dx:TreeListDataColumn Caption="Overall Status" FieldName="Status" VisibleIndex="2" Name="status" />
                <dx:TreeListDataColumn FieldName="DateChecked" VisibleIndex="3" Name="date" />
            </Columns>
            <SettingsBehavior ExpandCollapseAction="NodeDblClick" />
            <Templates>
                <Preview>
                    <a href="javascript:;" onclick="loadResults(escapeChars('<%# Eval("AppID") %>'), this)">Show Results... </a>
                </Preview>
            </Templates> 
            <Styles>
                <Preview Paddings-Padding="2" />
            </Styles>
            <ClientSideEvents CustomDataCallback="function(s, e) { showResults(e.result); }" />
        </dx:ASPxTreeList>

        <dx:ASPxPopupMenu ID="ASPxPopupMenu1" runat="server" RenderMode="Lightweight"
            ClientInstanceName="resultsPopup" HeaderText="Results" AllowDragging="True" 
            CloseAction="OuterMouseClick" Width="400px" PopupHorizontalAlign="LeftSides" 
            PopupVerticalAlign="Below">
        </dx:ASPxPopupMenu>
    </div>

        <%--///////////////////////////Scripts/////////////////////////////////--%>
        <script src="Scripts/jquery-1.8.3.min.js"></script>
        <script src="Scripts/jquery.signalR-2.1.1.min.js"></script>
        <script src="Scripts/jquery-ui-1.10.4.js"></script>
        <script type="text/javascript">
            // <![CDATA[
            function showResults(text) {
                resultsPopup.SetContentHTML(text);
            }
            function loadResults(id, refElement) {
                resultsPopup.Hide();
                resultsPopup.ShowAtElement(refElement);
                resultsPopup.SetContentHTML("Loading...");
                treeList.PerformCustomDataCallback(id);
            }
            function escapeChars(text) {
                var x = String(text);
                var newString = x;
                if (x.substring(0, 0) === "{") {//starts with
                    newString = "!-!" + x.substring(1, x.length); //!-! will be the special characters used to escape {}
                }
                if (x.substring(x.length - 1, x.length - 1) === "}") { //ends with
                    newString = newString.substring(0, newString.length - 1) + "!-!";
                }
                return String(newString);
            }
            // ]]> 

            function reloadPage() {
                window.location.reload();
            }

            $(function () {
                var globalConnection = $.connection("/monitorx");
                globalConnection.start();

                //CODE FOR TESTING
                //$("#btnSubmit").click(function () {
                //    globalConnection.send("rubbish");
                //});

                globalConnection.received(function (mem) {
                    //Json serialized tree member is meant to be received here;   
                    if (mem === "reloadPage") {
                        $("#server").append("<li>RELOAD PAGE</li>");
                        reloadPage(); //reload page if new data is ready. It will make the next line unecessary
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
                    //$("#error").append("<li>" + String(mem) + "</li>");
                    /*
                    if (json.RowClientId === null) {
                        $("#server").append("<li>UPDATE PAGE</li>");
                        reloadPage();//reload page if the data is fresh so as to retrieve its client id's for signalR update
                    }
                    */
                    if (json.DateCellClientId === json.StatusCellClientId) reloadPage();

                    //Blink a row when a new message is received
                    //$('#' + json.RowClientId).animate({ "background-color": "#3D7EFF" }, 100);
                    //$('#' + json.RowClientId).animate({ "background-color": "white" }, 500

                    //var date = new Date(json.DateChecked); //Convert the JSON date to a more pretty format
                    $('#' + json.DateCellClientId).text(new Date(json.DateChecked).toLocaleString());
                    switch (json.Status) {
                        case 0:
                            $('#' + json.StatusCellClientId).css("background-color", "gray").text("Unknown");
                            break;
                        case 1:
                            $('#' + json.StatusCellClientId).css("background-color", "lawngreen").text("Up");
                            break;
                        case 2:
                            $('#' + json.StatusCellClientId).css("background-color", "yellow").text("PerformanceDegraded");
                            break;
                        case 3:
                            $('#' + json.StatusCellClientId).css("background-color", "crimson").text("Down");
                            break;
                        default:
                            break;
                    };

                });
            });
        </script>
    </form>
</body>
</html>