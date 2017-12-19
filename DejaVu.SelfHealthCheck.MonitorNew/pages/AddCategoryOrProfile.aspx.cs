using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DejaVu.SelfHealthCheck.MonitorNew.pages
{
    public partial class AddCategoryOrProfile : System.Web.UI.Page
    {
       // public static string catNames;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        protected void SaveButton_Click1(object sender, EventArgs e)
        {
            Profile newProfile = new Profile();
            newProfile.ProfileName = txtProfileName.Text;
            if (ProfileLogic.IsExists(txtProfileName.Text))
            {
                ShowNotification(false, string.Format("A Profile with this {0} already exists", txtProfileName.Text), "");
                return;
            }
            else
            {
                ProfileLogic.AddNewEntity(newProfile);
               // ShowNotification(true, "Sucessfully Created", "AddCategoryOrProfile.aspx");
            }
            if (hdnCategories.Value.Contains(","))
            {
                string[] categoryNames = hdnCategories.Value.Replace(" ", "_").Split(',');
                foreach(string c in categoryNames)
                {
                    Category newCategory = new Category();
                    newCategory.CategoryName = c;
                    newCategory.Profile = newProfile;
                    if (CategoryLogic.IsExists(c))
                    {
                        ShowNotification(false, string.Format("A Category with this {0} already exists", c), "");
                       // return;
                    }
                    else
                    {
                        CategoryLogic.AddNewEntity(newCategory);
                        ShowNotification(true, "Sucessfully Created", "AddCategoryOrProfile.aspx");
                    }
                }
            }
            else
            {
                Category newCategory = new Category();
                newCategory.CategoryName = hdnCategories.Value.ToString();
                CategoryLogic.AddNewEntity(newCategory);
            }
            
           
        }

        void ShowNotification(bool isSucessfull, string message, string redirectUrl)
        {
            string script = "window.onload = function() { showNotification('" + isSucessfull + "','" + message + "','" + redirectUrl + "'); };";
            ClientScript.RegisterStartupScript(this.GetType(), "showNotification", script, true);
        }
        
    }
}