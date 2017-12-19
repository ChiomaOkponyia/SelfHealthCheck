using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DejaVu.SelfHealthCheck.Monitor.pages
{
    public partial class ViewApplicationsServerSide : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int echo = Int32.Parse(Request.Params["sEcho"]);
            int displayLength = Int32.Parse(Request.Params["iDisplayLength"]);
            int displayStart = Int32.Parse(Request.Params["iDisplayStart"]);
            int start = displayStart;
            string search = Request.Params["sSearch"];

            ///////////
            //SEARCH (filter)
            //- build the where clause
            ////////
            StringBuilder sb = new StringBuilder();

            /////////////
            /// JSON output
            /// - build JSON output from DB results
            /// ///////////

            sb.Clear();
            string outputJson = string.Empty;
            int totalDisplayRecords = 0;
            int totalRecords = 0;
            List<Component> components = ComponentDb.Find("", "", (displayStart / displayLength), displayLength, "DESC", "Default", out totalDisplayRecords);
            totalRecords = components.Count();
            foreach (var component in components)
            {
                start++;
                sb.Append("[");
                sb.Append("\"" + (start) + "\",");
                sb.Append("\"" + component.AppName + "\",");
                sb.Append("\"" + component.AppID + "\",");
                string buttonHtml = string.Format(@"<button type='button' onclick=editComponent('{0}'); class='btn btn-primary btn-xs' >Edit</button>", component.AppID);
                sb.Append("\"" + buttonHtml + "\"");
                sb.Append("],");
            }
            outputJson = sb.ToString();
            outputJson = outputJson.Remove(outputJson.Length - 1);
            sb.Clear();

            sb.Append("{");
            sb.Append("\"sEcho\": ");
            sb.Append(echo);
            sb.Append(",");
            sb.Append("\"iTotalRecords\": ");
            sb.Append(totalRecords);
            sb.Append(",");
            sb.Append("\"iTotalDisplayRecords\": ");
            sb.Append(totalDisplayRecords);
            sb.Append(",");
            sb.Append("\"aaData\": [");
            sb.Append(outputJson);
            sb.Append("]}");
            outputJson = sb.ToString();

            /////////////
            /// Write to Response
            /// - clear other HTML elements
            /// - flush out JSON output
            /// ///////////
            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Write(outputJson);
            Response.Flush();
            Response.End();
        }
    }
}