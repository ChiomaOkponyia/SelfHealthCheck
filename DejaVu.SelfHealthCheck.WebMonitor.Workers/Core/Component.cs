using DejaVu.SelfHealthCheck.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Core
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Component : IComponent
    {
        [JsonProperty]
        public virtual long Id { get; set; }
        [JsonProperty]
        public virtual string AppID { get; set; }
        public virtual bool HasSubComponents { get; set; }
        public virtual bool IsRootComponent { get; set; }
        public virtual string ParentComponentId { get; set; }
        public virtual string ParentsAsString { get; set; }
        [JsonProperty]
        public virtual string AppName { get; set; }
        public virtual AppStatus Status { get; set; }

        public virtual HeartBeatStatus HeartBeatStatus { get; set; }
      
        public virtual DateTime? LastUpdate { get; set; }
        [JsonProperty]
        public virtual string TheStatus
        {
            get
            {
                if (this.Status != null)
                {
                    return this.Status.ToString();
                }
                return "";
            }
        }
         [JsonProperty]
        public virtual string TheLastUpdateTime
        {
            get
            {
                if (this.LastUpdate != null)
                {
                    return String.Format("{0:d/M/yyyy HH:mm:ss}", this.LastUpdate);
                }
                return "";
            }
        }

        public virtual DateTime DateChecked { get; set; }
        [JsonProperty]
        public virtual IList<Component> ChildrenComponents { get; set; }
        public virtual IList<Component> Components { get; set; }
        public virtual Component ParentComponent { get; set; }
        public virtual Component HelperComponent { get; set; }
        public virtual Component HelperParentModel { get; set; }
        private bool _justRemoved = false;
        public virtual bool JustRemoved
        {
            get
            {
                return _justRemoved;
            }
            set
            {
                _justRemoved = value;
            }
        }
        public virtual string ChildrenAsString { get; set; } //this is a concatenated string consisting of all AppIds of the children

        public override bool Equals(object obj)
        {
            return (obj is Component) && ((obj as Component).Id == Id);
        }
    }
}
