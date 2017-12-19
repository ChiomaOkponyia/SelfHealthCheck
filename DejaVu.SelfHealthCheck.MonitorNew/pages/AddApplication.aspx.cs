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

namespace DejaVu.SelfHealthCheck.MonitorNew.pages
{
    public partial class AddApplication : System.Web.UI.Page
    {

        static List<Component> appList = new List<Component>();
        static bool isEdit;
        static string ID;
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
                   // AppIDTxt.Value = application.AppID;
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
                    ID = application.Id.ToString();
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
            string appID = string.Empty;
            //  string appID = AppIDTxt.Value;
            if (!isEdit)
            {
                appID = new ComponentLogic().GenerateAppID();
            }
            else { appID = Convert.ToString(Request.QueryString["appID"]); }
            var selectedItems = from item in AppsDrpDown.Items.Cast<ListItem>() where item.Selected select appListDic[item.Value];
            string state;
            bool isExists = ComponentLogic.isExist(appID, appName, ID, out state);
            if (isExists)
            {
                ShowNotification(false, string.Format("An Application with this {0} already exists", state), "");
                return;
            }
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
                ShowNotification(true, "Sucessfully Updated", "view-applications.html");
                //Response.Redirect("view-applications.html");
            }
            else
            {
                ComponentLogic.AddNewEntity(application);
                ShowNotification(true,string.Format( "Sucessfully Created, APPLICATION ID: {0}",application.AppID), "AddApplication.aspx");
                //Response.Redirect(Request.RawUrl);
            }


        }

        void ShowNotification(bool isSucessfull, string message, string redirectUrl)
        {
            string script = "window.onload = function() { showNotification('" + isSucessfull + "','" + message + "','" + redirectUrl + "'); };";
            ClientScript.RegisterStartupScript(this.GetType(), "showNotification", script, true);
        }


    }
}