namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Core
{
    public interface ICategory:IEntity
    {
         string CategoryName { get; set; }
        Profile Profile { get; set; }
    }
}