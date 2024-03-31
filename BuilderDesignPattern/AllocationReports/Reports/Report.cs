namespace BuilderDesignPattern.AllocationReports.Reports;

public class Report<T>
{
    public string Name { get; set; }  
    public string Destination { get; set; }
    public string FileName => Path.Join(Destination, $"{Name}.xlsx");
    private T Content;
    private Action<T, string> SaveAction { get; set; }
    private Action<T> PrintAction { get; set; }
    
    public Report(T content, Action<T, string> saveAction, Action<T> printAction)
    {
        Content = content;
        Name = Guid.NewGuid().ToString();
        Destination = "";
        SaveAction = saveAction;
        PrintAction = printAction;
    }

    public string Save()
    {
        SaveAction(Content, FileName);
        return FileName;
    }
    
    public void Print()
    {
        PrintAction(Content);
    }
}