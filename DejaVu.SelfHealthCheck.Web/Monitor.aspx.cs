using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using Raven.Client;
using DejaVu.SelfHealthCheck.Web;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using System.Drawing;

public partial class Monitor : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void RadTreeList1_ItemCreated(object sender, TreeListItemCreatedEventArgs e)
    {
        
    }

    protected void RadTreeList1_NeedDataSource(object sender, TreeListNeedDataSourceEventArgs e)
    {
        List<Component> rootData = new List<Component>();
        using (IDocumentSession session = DejaVu.SelfHealthCheck.WebMonitor.Workers.RavenDB.RavenStore.Store.OpenSession())
        {
            rootData = session.Query<Component>().Where(x => x.IsRootComponent == true).ToList();
        }
        RadTreeList1.DataSource = rootData;
    }

    protected void RadTreeList1_ChildItemsDataBind(object sender, TreeListChildItemsDataBindEventArgs e)
    {
        string parentAppId = e.ParentDataKeyValues["AppID"].ToString();
        List<Component> childData = new List<Component>();
        using (IDocumentSession session = DejaVu.SelfHealthCheck.WebMonitor.Workers.RavenDB.RavenStore.Store.OpenSession())
        {
            //List<Component> childDataUnfiltered = new List<Component>();
            Component parent = session.Query<Component>().Where(x => x.AppID == parentAppId).FirstOrDefault();
            if(parent != null && parent.ChildrenAsString != null)
            {
                string[] allChildrenIds = parent.ChildrenAsString.Split(' ');
                foreach (var childId in allChildrenIds)
                {
                    Component child = new Component();
                    child = session.Query<Component>().Where(x => x.AppID == childId).FirstOrDefault();
                    if(child != null)
                    {
                        Session[childId + "ParentsAsString"] = Session[childId + "ParentsAsString"] == null ? string.Empty : Session[childId + "ParentsAsString"].ToString() + parentAppId + " ";
                        string[] allParentIds = Session[childId + "ParentsAsString"].ToString().Split(' ');
                        if (!(allParentIds.Contains(child.AppID) || childData.Any(x => x.AppID == childId))) childData.Add(child);
                    }
                }
            }
        }
        
        e.ChildItemsDataSource = childData;
    }

    protected void RadTreeList1_ItemDataBound(object sender, TreeListItemDataBoundEventArgs e)
    {
        if (e.Item is TreeListDataItem)
        {
            TreeListDataItem item = e.Item as TreeListDataItem;
            string appName = (string)DataBinder.Eval(item.DataItem, "AppName");
            string appId = (string)DataBinder.Eval(item.DataItem, "AppID");
            if (appId.StartsWith("{")) appId = "" + appId.Substring(1, appId.Length - 1);
            if (appId.EndsWith("}")) appId = appId.Substring(0, appId.Length - 1) + "";
            string status = (string)DataBinder.Eval(item.DataItem, "Status");
            item.CssClass = appId;
            item["Status"].CssClass = appId + "Status";        
            Color statusBackColor = GetColor(status);
            item["Status"].BackColor = statusBackColor;
            if(statusBackColor == Color.Crimson | statusBackColor == Color.Gray)
            {
                item["Status"].ForeColor = System.Drawing.Color.White;
            }
            item["DateChecked"].CssClass = appId + "Date";                
        }
    }

    protected Color GetColor(string status)
    {
        switch (status)
        {
            case "Down":
                return Color.Crimson;
            case "PerfomanceDegraded":
                return Color.Yellow;
            case "Unknown":
                return Color.Gray;
            case "Up":
                return Color.LawnGreen;
            default:
                return Color.White;
        }
    }
}
public class testClass
{
    public string AppName { get; set; }
    public string AppID { get; set; }
    public string Status { get; set; }
    public DateTime DateChecked { get; set; }

}
