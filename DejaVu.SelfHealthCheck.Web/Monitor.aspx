<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Monitor.aspx.cs" Inherits="Monitor" %>

<!DOCTYPE html> 

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<telerik:RadStyleSheetManager id="RadStyleSheetManager1" runat="server" />

</head>
<body>
    <div>
        <section style="display: table;">
          <div style="display: table-row;">
            <div style="display: table-cell;">
                <form id="form2" runat="server">
	                <telerik:RadScriptManager ID="RadScriptManager2" runat="server">
		                <Scripts>
			                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
			                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQuery.js" />
			                <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.jQueryInclude.js" />
		                </Scripts>
	                </telerik:RadScriptManager>
	                <telerik:RadAjaxManager ID="RadAjaxManager2" runat="server">
	                </telerik:RadAjaxManager>
                                <telerik:RadTreeList ID="RadTreeList1" runat="server" Skin="MetroTouch" AllowLoadOnDemand="true" AllowPaging="true" PageSize="15" AutoGenerateColumns="false" 
                                OnNeedDataSource="RadTreeList1_NeedDataSource" OnChildItemsDataBind="RadTreeList1_ChildItemsDataBind"  OnItemCreated="RadTreeList1_ItemCreated"
                                 ParentDataKeyNames="AppID" DataKeyNames="AppID" OnItemDataBound="RadTreeList1_ItemDataBound"  >
                                <Columns>
                                    <telerik:TreeListBoundColumn DataField="AppName" HeaderText="Name" UniqueName="AppName" />
                                    <telerik:TreeListBoundColumn DataField="AppID" HeaderText="Application ID" UniqueName="AppID"/>
                                    <telerik:TreeListBoundColumn DataField="Status" HeaderText="Status" UniqueName="Status" />
                                    <telerik:TreeListBoundColumn DataField="DateChecked" HeaderText="Date Checked" UniqueName="DateChecked"/>             
                                </Columns>
                                </telerik:RadTreeList>                        
                    <div class="test"></div>
	            </form>
            </div>
            <div  style="display: table-cell; width:400px; height: 500px; padding-left:10px">
                <div style="position:relative; padding-left:0px">
                    <table>
                        <tr>
                            <td>
                                <input id="appIdToLoad" maxlength="100" type="text"  placeholder="application id" style="width:280px; height:30px; font-size:large; font-family: 'Segoe UI', Arial,Helvetica,sans-serif;"/> 
                            </td>
                            <td>
                                <button id="loadResults" style="width:120px; height: 36px; font-size:large; font-family: 'Segoe UI', Arial,Helvetica,sans-serif; background:#25a0da; border:none; color:white">load results</button>
                            </td>
                        </tr>
                    </table> 
                </div>
                <div id="resultPanel" style="position:relative; top:10px; width:100%; height:100%; font-family: 'Segoe UI', Arial,Helvetica,sans-serif;">
                    <div id="resultsLbl" style="width:100%; font-family: 'Segoe UI', Arial,Helvetica,sans-serif;">
                        awaiting request...
                    </div>                   
                </div>
             </div>
          </div>
        </section>
    <script src="Scripts/jquery-1.8.3.min.js"></script>
    <script src="Scripts/jquery.signalR-2.1.1.min.js"></script>
    <script src="Scripts/jquery-ui-1.10.4.js"></script>
    <%--<script src="Scripts/app.js"></script>--%>
    <script type="text/javascript">
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
    </script>
    </div>
    
</body>
</html>
