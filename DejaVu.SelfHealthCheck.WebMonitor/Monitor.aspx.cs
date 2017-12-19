using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Transactions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using DejaVu.SelfHealthCheck.Contracts;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.RavenDB;
using DevExpress.Web.ASPxTreeList;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Document;

namespace DejaVu.SelfHealthCheck.WebMonitor.WebPages
{
    public partial class Monitor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //RAVEN Db
            using (IDocumentSession session = Global.Store.OpenSession())
            {
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
                    myTree.DataSource = processedData;
                    myTree.DataBind();
                }
            }
        }

        protected void ASPxTreeList1_HtmlDataCellPrepared(object sender, DevExpress.Web.ASPxTreeList.TreeListHtmlDataCellEventArgs e)
        {
            e.Cell.BackColor = GetColor(e.Cell.Text);
            //using (IDocumentSession session = Global.Store.OpenSession())
            //{
            //    using (var transaction = new TransactionScope())
            //    {
            //        if (e.Column.Name == "status" && session.Query<TreeMember>().Where(x => x.AppID == e.NodeKey).Any())
            //        {
            //            TreeMember member = session.Load<TreeMember>(session.Query<TreeMember>().Where(x => x.AppID == e.NodeKey).First().Id);
            //            member.StatusCellClientId = e.Cell.ClientID;
            //            e.Cell.BackColor = GetColor(member.Status);
            //            Thread.Sleep(3000);
            //            session.SaveChanges();
            //        }
            //        if (e.Column.Name == "date" && session.Query<TreeMember>().Where(x => x.AppID == e.NodeKey).Any())
            //        {
            //            TreeMember member = session.Load<TreeMember>(session.Query<TreeMember>().Where(x => x.AppID == e.NodeKey).First().Id);
            //            member.DateCellClientId = e.Cell.ClientID;
            //            Thread.Sleep(3000);
            //            session.SaveChanges();
            //        }
            //        transaction.Complete();
            //    }
            //}
        }

        protected void ASPxTreeList1_HtmlRowPrepared(object sender, DevExpress.Web.ASPxTreeList.TreeListHtmlRowEventArgs e)
        {
            using (IDocumentSession session = Global.Store.OpenSession())
            {
                using (var transaction = new TransactionScope())
                {
                    if (e.RowKind != TreeListRowKind.Preview && session.Query<TreeListMember>().Where(x => x.AppID == e.NodeKey).Any())
                    {
                        TreeMember member = session.Load<TreeMember>(session.Query<TreeMember>().Where(x => x.AppID == e.NodeKey).First().Id);
                        member.RowClientId = e.Row.ClientID;
                        session.SaveChanges();
                    }
                    transaction.Complete();
                }
            }                                                                                                                                                                 
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

        protected Color GetColor(CheckResultStatus status)
        {
            switch (status)
            {
                case CheckResultStatus.Down:
                    return Color.Crimson;
                case CheckResultStatus.PerfomanceDegraded:
                    return Color.Yellow;
                case CheckResultStatus.Unknown:
                    return Color.Gray;
                case CheckResultStatus.Up:
                    return Color.LawnGreen;
                default:
                    return Color.White;
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
}