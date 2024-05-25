namespace ProjectManagement.Application.Contracts.Task;

public class DoTaskDto
{
    public Guid ProjectId { get; set; }
    public Guid TaskId { get; set; }
    public bool IsDone { get; set; }
}
