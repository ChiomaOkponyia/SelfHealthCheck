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


public class Index_Auto_2fTreeMembers_2fByAppID : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fTreeMembers_2fByAppID()
	{
		this.ViewText = @"from doc in docs.TreeMembers
select new { AppID = doc.AppID }";
		this.ForEntityNames.Add("TreeMembers");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "TreeMembers", System.StringComparison.InvariantCultureIgnoreCase)
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
