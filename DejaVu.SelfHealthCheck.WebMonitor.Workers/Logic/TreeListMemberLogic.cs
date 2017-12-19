using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.Contracts;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic
{
    public class TreeListMemberLogic //: EntityLogic<TreeListMember>
    {
        private const string cellStart = "<td style = " + "\"padding:2px 2px 2px 2px; ";
        private const string cellEnd = "</td>";
        private const string rowStart = "<tr>"; //put some style in here
        private const string rowEnd = "</tr>";
        public static string HtmlizeResults(List<TreeCheckResult> allResults)
        {
            string resultAsHtmlTable = @"<table >
	                            <tr style = " + "background-color:gray; width:100%" + @">
		                        <th style = " + "\"padding:4px 4px 4px 4px; color:white;\"" + @">Title</th>
		                        <th style = " + "\"padding:4px 4px 4px 4px; color:white;\"" + @">Status</th>		
		                        <th style = " + "\"padding:4px 4px 4px 4px; color:white;\"" + @">Time Elapsed</th>
                                <th style = " + "\"padding:4px 4px 4px 4px; color:white;\"" + @">Additional Informaition</th></tr>";

            foreach (var result in allResults)
            {
                resultAsHtmlTable += rowStart;
                resultAsHtmlTable += cellStart + "\">" + result.Title + cellEnd;
                resultAsHtmlTable += cellStart + styleStatusCell(result.Status) + "\">" + result.Status.ToString() + cellEnd;
                resultAsHtmlTable += cellStart + "\">" + result.TimeElasped.ToString() + cellEnd;
                resultAsHtmlTable += cellStart + "\">" + result.AdditionalInformation + cellEnd;
                resultAsHtmlTable += rowEnd;
            }
            return resultAsHtmlTable + "</table>";
        }

        protected static string styleStatusCell(CheckResultStatus status)
        {
            switch (status)
            {
                case CheckResultStatus.Down:
                    return "background-color:crimson;";
                case CheckResultStatus.PerfomanceDegraded:
                    return "background-color:yellow;";
                case CheckResultStatus.Unknown:
                    return "background-color:gray;";
                case CheckResultStatus.Up:
                    return "background-color:lawngreen;";
                default:
                    return "";
            }
        }
    }
}
