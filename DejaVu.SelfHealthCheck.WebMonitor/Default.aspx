<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="DejaVu.SelfHealthCheck.WebMonitor.Default" %>

<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxSplitter" TagPrefix="dx" %>

<%@ Register Assembly="DevExpress.Web.v13.1, Version=13.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxMenu" TagPrefix="dx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <style>
        @font-face {
            font-family: "My Custom Font";
            src: url(Fonts/Transformers.TTF) format("truetype");
        }
        a.customfont { 
            font-family: "My Custom Font", Verdana, Tahoma;
        }
    </style>
    <form id="form1" runat="server">
    <div>
        <dx:ASPxMenu ID="ASPxMenu1" runat="server" RenderMode="Lightweight" Width="100%" AllowSelectItem="true" Height="25" OnItemClick="ASPxMenu1_ItemClick" Target="MainFrame">
            <Items>
                <dx:MenuItem Selected="true" Text="Monitor" NavigateUrl="Monitor.aspx">
                </dx:MenuItem>
                <dx:MenuItem Text="Add Autonomous Component" NavigateUrl="WebPages/AddComponent.aspx">
                </dx:MenuItem>
                <dx:MenuItem Text="View Components" NavigateUrl="WebPages/ViewComponents.aspx">
                </dx:MenuItem>
            </Items>
        </dx:ASPxMenu>
        <dx:ASPxSplitter ID="ASPxSplitter1" runat="server" Orientation="Vertical" Width="100%" Height="500">
            <Panes>
                <dx:SplitterPane AutoHeight="true">
                    <ContentCollection>
                        <dx:SplitterContentControl>
                            <a class="customfont" style="font-size:x-large; color: steelblue">DejaVu Self-Health Check Web Monitor </a>
                            <a class="customfont" style="font-size:x-large; color: crimson">}</a>
                        </dx:SplitterContentControl>
                    </ContentCollection>
                </dx:SplitterPane>
                <dx:SplitterPane ScrollBars="Auto" ContentUrlIFrameName="MainFrame" Name="MainFrame" AllowResize="True">                   
                </dx:SplitterPane>
            </Panes>
        </dx:ASPxSplitter>
    </div>
    </form>
</body>
</html>
