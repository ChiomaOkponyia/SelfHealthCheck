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


public class Index_Auto_2fTimeoutData_2fByOwningTimeoutManagerAndTimeSortByTime : Raven.Database.Linq.AbstractViewGenerator
{
	public Index_Auto_2fTimeoutData_2fByOwningTimeoutManagerAndTimeSortByTime()
	{
		this.ViewText = @"from doc in docs.TimeoutData
select new { OwningTimeoutManager = doc.OwningTimeoutManager, Time = doc.Time }";
		this.ForEntityNames.Add("TimeoutData");
		this.AddMapDefinition(docs => 
			from doc in docs
			where string.Equals(doc["@metadata"]["Raven-Entity-Name"], "TimeoutData", System.StringComparison.InvariantCultureIgnoreCase)
			select new {
				OwningTimeoutManager = doc.OwningTimeoutManager,
				Time = doc.Time,
				__document_id = doc.__document_id
			});
		this.AddField("OwningTimeoutManager");
		this.AddField("Time");
		this.AddField("__document_id");
		this.AddQueryParameterForMap("OwningTimeoutManager");
		this.AddQueryParameterForMap("Time");
		this.AddQueryParameterForMap("__document_id");
		this.AddQueryParameterForReduce("OwningTimeoutManager");
		this.AddQueryParameterForReduce("Time");
		this.AddQueryParameterForReduce("__document_id");
	}
}
