namespace BuilderDesignPattern.Models;

public class Task
{
    public long Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public List<string> Notes { get; set; }
    public TaskStatus Status { get; set; }
}