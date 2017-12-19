using Raven.Abstractions;
using Raven.Database.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System;
using Raven.Database.Linq.PrivateExtensions;
using Lucene.Net.Documents;
using System.Globalization;
using System.Text.RegularExpressions;
using Raven.Database.Indexing;


public class Index_Auto_2fTreeCheckResults_2fByAppIDAndTitle : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fTreeCheckResults_2fByAppIDAndTitle()
	{
		this.ViewText = @"from doc in docs.TreeCheckResults
select new { AppID = doc.AppID, Title = doc.Title }";
		this.ForEntityNames.Add("TreeCheckResults");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "TreeCheckResults", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				AppID = doc.AppID,
				Title = doc.Title,
				__document_id = doc.__document_id
			});
		this.AddField("AppID");
		this.AddField("Title");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("AppID");
		this.AddQueryParameterForMap("Title");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("AppID");
		this.AddQueryParameterForReduce("Title");
		this.AddQueryParameterForReduce("__document_id");
	}
}
