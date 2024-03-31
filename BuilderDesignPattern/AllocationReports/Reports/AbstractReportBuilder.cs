namespace BuilderDesignPattern.AllocationReports.Reports;

public abstract class AbstractReportBuilder<TIn, TOut>
{
    public abstract AbstractReportBuilder<TIn, TOut> AppendRecords(IEnumerable<TIn> recordCollection);
    public abstract AbstractReportBuilder<TIn, TOut> RemoveRecords(Func<TIn, bool> predicate);
    
    protected string Title;
    public virtual AbstractReportBuilder<TIn, TOut> SetTitle(string title)
    {
        Title = title;
        return this;
    }
    public abstract TOut ApplyTitle(TOut data);
    
    protected bool HasHeaders = false;
    public virtual AbstractReportBuilder<TIn, TOut> EnableHeaders()
    {
        HasHeaders = true;
        return this;
    }
    public abstract TOut ApplyHeaders(TOut data);

    protected bool HasSectionSummary = false;
    public virtual AbstractReportBuilder<TIn, TOut> EnableSectionSummary()
    {
        HasSectionSummary = true;
        return this;
    }
    
    protected bool HasTotalSummary = false;
    public virtual AbstractReportBuilder<TIn, TOut> EnableTotalSummary()
    {
        HasTotalSummary = true;
        return this;
    }
    public abstract TOut ApplyTotalSummary(TOut data);

    protected string DistributionNote;
    public virtual AbstractReportBuilder<TIn, TOut> AddDistributionNote(string note)
    {
        DistributionNote = note;
        return this;
    }
    public abstract TOut ApplyDistributionNote(TOut data);
    
    public abstract Report<TOut> Build(TOut data);

    public abstract TOut GetReportData();
}