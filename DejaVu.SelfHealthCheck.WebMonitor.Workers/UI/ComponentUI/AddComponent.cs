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
using System.Transactions;
using System.Threading;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.UI.ComponentUI
{
    public class AddComponent : EntityUI<Component>
    {
        public AddComponent()
        {
            UseFullView();
            WithTitle("Add New Autonomous Component");
            AddSection()
                .ApplyMod<IconMod>(y => y.WithIcon(Ext.Net.Icon.ApplicationAdd))
                .IsFormGroup()
                .WithTitle("New Component")
                .StretchFields(80)
                .WithColumns(new List<Column>()
                 {
                     new Column(new List<IField>()
                     {
                         Map(a => a.AppName).AsSectionField<TextBox>().Required().WithLength(50),
                         Map(a=>a.AppID).AsSectionField<TextBox>().Required().WithLength(50),
                     }),
                     new Column(new List<IField>()
                     {
                         Map(a => a.IsRootComponent).AsSectionField<CheckBox>().Required(),
                         //Map(a=> a.ParentComponentId).AsSectionField<TextBox>().WithLength(50).LabelTextIs("Parent App ID"),
                     }),               
                 });

            AddSection()
                .WithTitle("Children Components")
                .IsFramed()
                .WithFields(new List<IField>()
                {
                    Map(x => x.HelperComponent).AsSectionField<DropDownList>()
                    .Of(RavenDB.RavenStore.Store.OpenSession().Query<Component>().ToList())
                    .ListOf(x => x.AppName, x => x.AppID).NoValidation()
                    .LabelTextIs("Child"),

                    AddSectionButton()
                    .WithText("Add")
                    .ApplyMod<IconMod>(x => x.WithIcon(Ext.Net.Icon.ApplicationAdd))
                    .UpdateWith(x => {
                        if(x.HelperComponent != null && x.JustRemoved == false)
                        {
                            x.HelperComponent.HelperParentModel = x;
                            if (x.ChildrenComponents == null) x.ChildrenComponents = new List<Component>();
                            if (!x.ChildrenComponents.Any(a => a.AppID == x.HelperComponent.AppID))
                            {
                                x.JustRemoved = false;
                                x.ChildrenComponents.Add(x.HelperComponent);
                                x.ChildrenAsString += x.HelperComponent.AppID + " ";
                            }
                        }
                        return x;
                    }, true).NoValidate()
                });

            HasMany(x => x.ChildrenComponents)
                .As<Grid>()
                .ApplyMod<IconMod>(x => x.WithIcon(Ext.Net.Icon.Link))
                .ApplyMod<GridButtonMod>(a => a.WithPostBack().WithIcon(Ext.Net.Icon.Cancel)
                    .With<Component>(b =>
                    {
                        b.WithText("Remove Child").UpdateWith(c =>
                        {
                            c.HelperParentModel.ChildrenComponents.Remove(c);
                            c.HelperParentModel.ChildrenAsString = c.HelperParentModel.ChildrenAsString.Replace(c.AppID + " ", "");
                            c.HelperParentModel.JustRemoved = true;
                            return c;
                        }, true);
                        return b;
                    }))
                 .Of<Component>()
                  .WithColumn(x => x.AppName)
                  .WithColumn(x => x.AppID)
                  .WithRowNumbers()
                  .LabelTextIs("Children");


            AddButton()
                .WithText("Save")
                .SubmitTo(x =>
                {
                    try
                    {
                        //bool result = ComponentLogic.AddNewEntity(new Component()
                        //{
                        //    AppID = x.AppID,
                        //    AppName = x.AppName,
                        //    Status = "Unknown",
                        //    DateChecked = DateTime.Now.ToString("u"),
                        //    ChildrenComponents = x.ChildrenComponents
                        //});
                        //return result;
                        using (IDocumentSession session = Workers.RavenDB.RavenStore.Store.OpenSession())
                        {
                            string childrenAsString = string.Empty;
                            x.ChildrenComponents = x.ChildrenComponents == null ? new List<Component>() : x.ChildrenComponents;
                            foreach(var child in x.ChildrenComponents)
                            {
                                childrenAsString += child.AppID + " ";
                            }
                            Component newComponent = new Component()
                            {
                                AppID = x.AppID,
                                AppName = x.AppName,
                                Status = "Unknown",
                                DateChecked = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt"),
                                ChildrenAsString = childrenAsString
                            };
                            int count = session.Query<Component>().Where(b => b.AppID == newComponent.AppID).ToList().Count;
                            if (count > 0) return false;
                            session.Store(newComponent);
                            session.SaveChanges();
                            Thread.Sleep(1000);
                            count = session.Query<Component>().Where(b => b.AppID == newComponent.AppID).ToList().Count;
                            if(count > 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Ext.Net.X.Msg.Alert("Error", e.Message).Show();
                        return false;
                    }

                })
                .OnSuccessDisplay("New Component has been Saved")
                .OnFailureDisplay("Unable to save Component information! \n You Probably have an app with the same Id already\nTry Again");

        }
    }
}
