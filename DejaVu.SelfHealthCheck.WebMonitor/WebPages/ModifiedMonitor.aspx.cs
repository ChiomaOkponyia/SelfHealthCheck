using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DejaVu.SelfHealthCheck.WebMonitor.WebPages
{
    public partial class ModifiedMonitor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ASPxTreeList1_VirtualModeCreateChildren(object sender, DevExpress.Web.ASPxTreeList.TreeListVirtualModeCreateChildrenEventArgs e)
        {
            if (e.NodeObject == null)
            {
                using (IDocumentSession session = Global.Store.OpenSession())
                {
                    //TODO: Query the list of treelist members to get unique results based on appIds
                    if (session.Query<TreeMember>().Any())
                    {
                        var allData = session.Query<TreeMember>().ToList();
                        var processedData = session.Query<TreeMember>().ToList();
                        foreach (var data in allData)
                        {
                            if (processedData.Where(x => x.AppID == data.AppID).Count() > 1)
                            {
                                TreeMember uniqueMember = data;
                                processedData.RemoveAll(x => x.AppID == data.AppID);
                                processedData.Add(data);
                            }
                        }
                        e.Children = processedData;
                    }
                }
            }
            else
            {
                TreeMember theMember = e.NodeObject as TreeMember;
                using (IDocumentSession session = Global.Store.OpenSession())
                {
                    var theChildren = session.Query<TreeMember>().Where(x => x.ParentComponentId == theMember.AppID).ToList();
                    e.Children = theChildren;
                }              
            }
        }

        protected void ASPxTreeList1_VirtualModeNodeCreating(object sender, DevExpress.Web.ASPxTreeList.TreeListVirtualModeNodeCreatingEventArgs e)
        {
            TreeMember theMember = e.NodeObject as TreeMember;
            e.IsLeaf = theMember.HasSubComponents;
            e.SetNodeValue("Name", theMember.AppName);
            e.SetNodeValue("AppID", theMember.AppID);
            e.SetNodeValue("Status", theMember.Status);
            e.SetNodeValue("DateChecked", theMember.DateChecked);
            e.NodeKeyValue = theMember.AppID;         
        }

        protected void ASPxTreeList1_VirtualModeNodeCreated(object sender, DevExpress.Web.ASPxTreeList.TreeListVirtualNodeEventArgs e)
        {
            
        }

        protected void ASPxTreeList1_CustomDataCallback(object sender, DevExpress.Web.ASPxTreeList.TreeListCustomDataCallbackEventArgs e)
        {
            using (IDocumentSession session = Global.Store.OpenSession())
            {
                string processedId = e.Argument;
                if (e.Argument.StartsWith("!-!")) processedId = "{" + e.Argument.Substring(3, e.Argument.Length - 3);
                if (e.Argument.EndsWith("!-!")) processedId = processedId.Substring(0, processedId.Length - 3) + "}";
                using (var transaction = new TransactionScope())
                {
                    if (session.Query<TreeCheckResult>().Where(x => x.AppID == processedId).Any())
                    {
                        TreeMember member = session.Load<TreeMember>(session.Query<TreeMember>().Where(x => x.AppID == processedId).First().Id);
                        List<TreeCheckResult> allResults = session.Query<TreeCheckResult>().Where(r => r.AppID == processedId).ToList();
                        List<TreeCheckResult> processedResults = session.Query<TreeCheckResult>().Where(r => r.AppID == processedId).ToList();
                        foreach (var result in allResults)
                        {
                            if (processedResults.Where(x => x.Title == result.Title).Count() > 1)
                            {
                                TreeCheckResult uniqueResult = result;
                                processedResults.RemoveAll(x => x.Title == result.Title);
                                processedResults.Add(result);
                            }
                        }
                        e.Result = TreeListMemberLogic.HtmlizeResults(processedResults);
                    }
                    else e.Result = "Results not found";
                    transaction.Complete();
                }
            }
        }

        protected void ASPxTreeList1_HtmlRowPrepared(object sender, DevExpress.Web.ASPxTreeList.TreeListHtmlRowEventArgs e)
        {
            e.Row.CssClass = e.NodeKey;
        }

        protected void ASPxTreeList1_HtmlDataCellPrepared(object sender, DevExpress.Web.ASPxTreeList.TreeListHtmlDataCellEventArgs e)
        {
            e.Cell.BackColor = GetColor(e.Cell.Text);
            e.Cell.CssClass = e.NodeKey + e.Column.Name;
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
}