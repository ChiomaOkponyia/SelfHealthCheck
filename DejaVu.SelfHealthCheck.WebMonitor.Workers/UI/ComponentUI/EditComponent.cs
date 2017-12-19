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
    public class EditComponent : EntityUI<Component>
    {
        public EditComponent()
        {
            UseFullView();
            WithTitle("Edit Autonomous Component Information");
            AddSection()
                .ApplyMod<IconMod>(y => y.WithIcon(Ext.Net.Icon.ApplicationEdit))
                .IsFormGroup()
                .WithTitle("Edit Component")
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
                         //Map(a=> a.ParentComponentId).AsSectionField<DropDownList>().Of<Comp
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
                .IsPaged<Component>(20, (x, e) =>
                {
                    int totalCount = 0;
                    try
                    {
                        string[] allChildrenIds = { };
                        if (x.ChildrenAsString != null)
                        {
                            allChildrenIds = x.ChildrenAsString.Split(' ');
                        }//x.ChildrenAsString.Split(' ');
                        x.ChildrenComponents = new List<Component>();
                        x.HelperComponent = new Component();
                        using (IDocumentSession session = Workers.RavenDB.RavenStore.Store.OpenSession())
                        {
                            foreach (var childId in allChildrenIds)
                            {
                                Component childApp = new Component();
                                childApp = session.Query<Component>().Where(a => a.AppID == childId).FirstOrDefault();
                                if (childApp != null)
                                {
                                    //x.HelperComponent.HelperParentModel = x;
                                    childApp.HelperParentModel = x;
                                    x.ChildrenComponents.Add(childApp);
                                    x.JustRemoved = false;
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
                })
                .LabelTextIs("Children");

            AddButton()
                .WithText("Update")
                .SubmitTo(x =>
                {
                    try
                    {
                        //bool result = ComponentLogic.EditEntity(new Component()
                        //{
                        //    Id = x.Id,
                        //    AppID = x.AppID,
                        //    AppName = x.AppName,
                        //    IsRootComponent = x.IsRootComponent,
                        //    ChildrenComponents = x.ChildrenComponents
                        //});
                        //return result;
                        string childrenAsString = string.Empty;
                        if (x.ChildrenComponents == null) x.ChildrenComponents = new List<Component>();
                        foreach (var child in x.ChildrenComponents)
                        {
                            if(child.AppID != x.AppID)
                            childrenAsString += child.AppID + " ";
                        }
                        using (IDocumentSession session = Workers.RavenDB.RavenStore.Store.OpenSession())
                        {
                            var thisApp = session.Query<Component>().Where(a => a.AppID == x.AppID).First();
                            var thisAppInRaven = session.Load<Component>(thisApp.Id);
                            thisAppInRaven.AppID = x.AppID;
                            thisAppInRaven.AppName = x.AppName;
                            thisAppInRaven.IsRootComponent = x.IsRootComponent;
                            thisAppInRaven.ChildrenAsString = childrenAsString;
                            thisAppInRaven.DateChecked = Convert.ToDateTime(x.DateChecked).ToString("dd-MMM-yyyy hh:mm:ss tt");
                            session.SaveChanges();
                            return true;
                        }                       
                    }
                    catch (Exception e)
                    {
                        Ext.Net.X.Msg.Alert("Error", e.Message).Show();
                        return false;
                    }

                })
                .OnSuccessDisplay("New Component has been Saved")
                .OnFailureDisplay("Unable to save Component information! \n You Probably have an app with the same Id already \n Try Again");

            AddButton()
                .WithText("Cancel").NoValidate()
                .SubmitTo(x => true)
                .OnFailureRedirectTo("EditComponent.aspx");
        }
    }
}
