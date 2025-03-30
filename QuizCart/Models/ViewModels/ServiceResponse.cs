public class ServiceResponse
{
    public enum ServiceStatus
    {
        Success,
        Created,
        Updated,
        Deleted,
        NotFound,
        Error
    }

    public ServiceStatus Status { get; set; } = ServiceStatus.Success;
    public List<string> Messages { get; set; } = new();
    public int CreatedId { get; set; } = 0;
}
