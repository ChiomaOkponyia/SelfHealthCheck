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


public class Index_Auto_2fTreeListMembers_2fByAppID : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fTreeListMembers_2fByAppID()
	{
		this.ViewText = @"from doc in docs.TreeListMembers
select new { AppID = doc.AppID }";
		this.ForEntityNames.Add("TreeListMembers");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "TreeListMembers", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				AppID = doc.AppID,
				__document_id = doc.__document_id
			});
		this.AddField("AppID");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("AppID");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("AppID");
		this.AddQueryParameterForReduce("__document_id");
	}
}
