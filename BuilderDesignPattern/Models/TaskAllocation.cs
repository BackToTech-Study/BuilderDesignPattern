namespace BuilderDesignPattern.Models;

public class TaskAllocation
{
    public long Id { get; set; }
    public Task Task { get; set; }
    public User User { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}