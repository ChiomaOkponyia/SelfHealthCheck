using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Data;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic;
using DejaVu.SelfHealthCheck.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DejaVu.SelfHealthCheck.Monitor.pages
{
    public partial class AddApplication : System.Web.UI.Page
    {

        static List<Component> appList = new List<Component>();
        static bool isEdit;
        protected void Page_Load(object sender, EventArgs e)
        {
            string appID = Convert.ToString(Request.QueryString["appID"]);
            if (!string.IsNullOrEmpty(appID))
            {
                isEdit = true;
            }
            else
            {
                isEdit = false;
            }
            if (!IsPostBack)
            {
                appList = ComponentDb.GetAll();
                BindData();
                if (isEdit)
                {
                    Component application = ComponentLogic.GetByAppId(appID);
                    AppNameTxt.Value = application.AppName;
                    AppIDTxt.Value = application.AppID;
                    if (application.ChildrenComponents != null && application.ChildrenComponents.Count > 0)
                    {
                        foreach (var item in application.ChildrenComponents)
                        {
                            ListItem listItem = new ListItem(item.AppName, item.Id.ToString());
                            listItem.Selected = true;
                            AppsDrpDown.Items.Add(listItem);
                        }
                    }
                    SaveButton.Text = "Update";
                }
            }
        }

        private void BindData()
        {

            AppsDrpDown.DataSource = appList;
            AppsDrpDown.DataTextField = "AppName";
            AppsDrpDown.DataValueField = "Id";
            AppsDrpDown.DataBind();

            AppsDrpDown.Multiple = true;
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Component application;
            Dictionary<string, Component> appListDic = new Dictionary<string, Component>();
            appListDic = appList.ToDictionary<Component, string>(x => x.Id.ToString());
            string appName = AppNameTxt.Value;
            string appID = AppIDTxt.Value;
            var selectedItems = from item in AppsDrpDown.Items.Cast<ListItem>() where item.Selected select appListDic[item.Value];
            application = ComponentLogic.GetByAppId(appID);
            if (!isEdit)
            {
                application = new Component();
                application.Status = AppStatus.Down;
                application.HeartBeatStatus = HeartBeatStatus.Down;
            }
            application.AppID = appID;
            application.AppName = appName;
            application.ChildrenComponents = selectedItems.ToList();
            if (isEdit)
            {
                ComponentLogic.EditEntity(application);
                Response.Redirect("view-applications.html");
            }
            else
            {
                ComponentLogic.AddNewEntity(application);
                Response.Redirect(Request.RawUrl);
            }

        }
    }
}