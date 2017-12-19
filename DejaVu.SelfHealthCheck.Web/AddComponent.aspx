<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddComponent.aspx.cs" Inherits="DejaVu.SelfHealthCheck.Web.AddComponent" %>
<%@ Register Assembly="Ext.Net" Namespace="Ext.Net" TagPrefix="ext" %>
<%@ Register assembly="AppZoneUI.Framework" namespace="AppZoneUI.Framework" tagprefix="cc1" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <ext:ResourceManager ID="ResourceManager1" runat="server"></ext:ResourceManager>
    <cc1:EntityUIControl ID="AddChannelUI" runat="server" UIType = "DejaVu.SelfHealthCheck.WebMonitor.Workers.UI.ComponentUI.AddComponent, DejaVu.SelfHealthCheck.WebMonitor.Workers" />
    
    </div>
    </form>
</body>
</html>
