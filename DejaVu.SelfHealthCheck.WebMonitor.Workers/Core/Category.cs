namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Core
{
    public class Category:Entity, ICategory
    {
        public virtual string CategoryName { get; set; }
        public virtual Profile Profile { get; set; }
    }
}