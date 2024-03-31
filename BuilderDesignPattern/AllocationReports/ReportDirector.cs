using BuilderDesignPattern.AllocationReports.Reports;
using BuilderDesignPattern.Models;
using BuilderDesignPattern.Models.Excel;

namespace BuilderDesignPattern.AllocationReports;

public class ReportDirector
{
    public Report<DataCollection> BuildReport(AbstractReportBuilder<TaskAllocation, DataCollection> builder)
    {
        var data = new DataCollection();
        data = builder.ApplyTitle(data);
        data = builder.ApplyHeaders(data);
        data.Rows.AddRange(builder.GetReportData().Rows);
        data = builder.ApplyTotalSummary(data);
        data = builder.ApplyDistributionNote(data);
        return builder.Build(data);
    }
}