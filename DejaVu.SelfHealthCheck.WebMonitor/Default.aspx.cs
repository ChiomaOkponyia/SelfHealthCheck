using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DejaVu.SelfHealthCheck.WebMonitor
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ASPxSplitter1.GetPaneByName("MainFrame").ContentUrl = ASPxMenu1.SelectedItem.NavigateUrl;
        }

        protected void ASPxMenu1_ItemClick(object source, DevExpress.Web.ASPxMenu.MenuItemEventArgs e)
        {
            ASPxSplitter1.GetPaneByName("MainFrame").ContentUrl = ASPxMenu1.SelectedItem.NavigateUrl;
        }
    }
}