using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneUI.Framework;
using AppZoneUI.Framework.Mods;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic;
using Raven.Client;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.UI.ComponentUI
{
    public class ComponentDetail : EntityUI<Component>
    {
        public ComponentDetail()
        {
            UseFullView();
            WithTitle("Component Details");

            AddSection()
                //.ApplyMod<IconMod>(a => a.WithIcon(Ext.Net.Icon.Link))
             .IsFormGroup()
             .WithColumns(new List<Column>()
             {
                new Column(new List<IField>()
                   { 
                       Map(x=>x.AppName).AsSectionField<TextLabel>(),
                       Map(x=>x.AppID).AsSectionField<TextLabel>(),
                       Map(x=>x.IsRootComponent).AsSectionField<TextLabel>(),
                       Map(x=>x.DateChecked).AsSectionField<TextLabel>().LabelTextIs("Date Last Checked")
                   })
             });
            HasMany(x => x.ChildrenComponents)
                .As<Grid>()
                .ApplyMod<IconMod>(x => x.WithIcon(Ext.Net.Icon.Link))
                .Of<Component>()
                .WithColumn(x => x.AppName)
                .WithColumn(x => x.AppID)
                .WithRowNumbers()
                .IsPaged(10)
                .IsPaged<Component>(20, (x, e) =>
                {
                    int totalCount = 0;
                    try
                    {

                        string[] allChildrenIds = {};
                        if (x.ChildrenAsString != null)
                        {
                            allChildrenIds = x.ChildrenAsString.Split(' ');
                        }//x.ChildrenAsString.Split(' ');
                        x.ChildrenComponents = new List<Component>();
                        using (IDocumentSession session = Workers.RavenDB.RavenStore.Store.OpenSession())
                        {
                            foreach (var childId in allChildrenIds)
                            {
                                Component childApp = new Component();
                                childApp = session.Query<Component>().Where(a => a.AppID == childId).FirstOrDefault();
                                if (childApp != null)
                                {
                                    
                                    x.ChildrenComponents.Add(childApp);

                                }
                            }
                            totalCount = x.ChildrenComponents.Count;
                        }
                    }
                    catch (Exception ex)
                    {
                        Ext.Net.X.Msg.Alert("Error", ex.Message).Show();
                    }

                    e.TotalCount = totalCount;
                    return x;
                }).WithAutoLoad()
                .LabelTextIs("Children Components");
                
            AddButton()
                .WithText("Edit")
                .ApplyMod<ButtonPopupMod>(x => x.Popup<EditComponent>("Edit Component"));

            AddButton()
                 .WithText("Ok").NoValidate()
                 .SubmitTo(x => true)
                 .OnFailureRedirectTo("ViewComponents.aspx");
        }
    }
}
