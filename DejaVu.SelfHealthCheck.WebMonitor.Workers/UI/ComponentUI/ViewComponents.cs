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
    public class ViewComponents : EntityUI<Component>
    {
        public ViewComponents()
        {
            UseFullView();
            WithTitle("Autonomous Components");
            AddSection()
                .ApplyMod<IconMod>(y => y.WithIcon(Ext.Net.Icon.ApplicationCascade))
                .IsFormGroup()
                .WithTitle("Find Component")
                .StretchFields(80)
                .WithColumns(new List<Column>()
                 {
                     new Column(new List<IField>()
                     {
                         Map(a => a.AppName).AsSectionField<TextBox>().WithLength(50),
                         Map(a=>a.AppID).AsSectionField<TextBox>().WithLength(50),
                     }),
                     new Column(new List<IField>()
                     {
                         Map(a => a.IsRootComponent).AsSectionField<CheckBox>().Required(),
                         //Map(a=> a.ParentComponentId).AsSectionField<TextBox>().WithLength(50).LabelTextIs("Parent App ID"),
                         AddSectionButton()
                        .WithText("Search")
                        .UpdateWith(x=>
                            {
                                return x;
                            })
                     }),
                 });

            HasMany(x => x.Components)
                .As<Grid>()
                .ApplyMod<ViewDetailsMod>(x => x.Popup<ComponentDetail>("Component Details"))
                .Of<Component>()
                .WithRowNumbers()
                .WithColumn(x => x.AppName)
                .WithColumn(x => x.AppID)
                .WithColumn(x => x.IsRootComponent)
                .IsPaged<Component>(10, (x, e) =>
                {
                    int totalCount = -1;
                    try
                    {
                        Func<Component, bool> query;

                        query = a => a.AppID.Contains(x.AppID) &&
                                a.AppName.Contains(x.AppName) &&
                                a.IsRootComponent == x.IsRootComponent;

                        //x.Components = ComponentLogic.PagedGetEntities(query, e.Start, e.Limit, out totalCount);
                        using (IDocumentSession session = Workers.RavenDB.RavenStore.Store.OpenSession())
                        {
                            List<Component> allComponents = session.Query<Component>().ToList();
                            x.Components = allComponents.Where(query).ToList();
                            totalCount = x.Components.Count;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    e.TotalCount = totalCount;
                    return x;
                })
                .LabelTextIs("Autonomous Components");

        }
    }
}
