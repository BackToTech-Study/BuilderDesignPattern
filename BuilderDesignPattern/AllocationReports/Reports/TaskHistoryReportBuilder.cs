using BuilderDesignPattern.Models;
using BuilderDesignPattern.Models.Excel;
using OfficeOpenXml;

namespace BuilderDesignPattern.AllocationReports.Reports;

public class TaskHistoryReportBuilder : AbstractReportBuilder<TaskAllocation, DataCollection>
{
    private Dictionary<long, List<TaskAllocation>> _taskAllocations = new();
    
    private Action<DataCollection, string> _saveAction = (dataCollection, fileName) =>
    {
        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Task History");
            for (var i = 0; i < dataCollection.Rows.Count; i++)
            {
                var row = dataCollection.Rows[i];
                for (var j = 0; j < row.Cells.Count; j++)
                {
                    worksheet.Cells[i + 1, j + 1].Value = row.Cells[j];
                }
            }
            package.SaveAs(new FileInfo(fileName));
        }
    };
    
    private Action<DataCollection> _printAction = dataCollection =>
    {
        foreach (var row in dataCollection.Rows)
        {
            var cellValues = row.Cells.Select(cell => cell.ToString());
            var rowText = string.Join(" ", cellValues);
            Console.WriteLine(rowText);
        }
        Console.WriteLine();
    };
    
    public override TaskHistoryReportBuilder AppendRecords(IEnumerable<TaskAllocation> recordCollection)
    {
        foreach (var taskAllocation in recordCollection)
        {
            if (!_taskAllocations.ContainsKey(taskAllocation.Task.Id))
            {
                _taskAllocations.Add(taskAllocation.Task.Id, new List<TaskAllocation>());
            }
            _taskAllocations[taskAllocation.Task.Id].Add(taskAllocation);
        }
        return this;
    }

    public override TaskHistoryReportBuilder RemoveRecords(Func<TaskAllocation, bool> predicate)
    {
        var allocationsToRemove = _taskAllocations.SelectMany(allocations => allocations.Value)
                                                                    .Where(predicate)
                                                                    .ToList();
        foreach (var allocation in allocationsToRemove)
        {
            _taskAllocations[allocation.Task.Id].Remove(allocation);
        }
        return this;
    }

    public override Report<DataCollection> Build(DataCollection data)
    {
        return new Report<DataCollection>(data, _saveAction, _printAction);
    }
    
    public override DataCollection ApplyTitle(DataCollection data)
    {
        if (string.IsNullOrEmpty(Title))
        {
            return data;
        }
        
        data.Rows.Add(new DataRow
        {
            Cells = new List<object> { "Task History" }
        });

        data.Rows.Add(new DataRow());
        
        return data;
    }
    
    public override DataCollection ApplyHeaders(DataCollection data)
    {
        if (!HasHeaders)
        {
            return data;
        }
        
        data.Rows.Add( new DataRow
        {
            Cells = new List<object> { "Task", "Status", "User", "Start Date", "End Date", "Duration" }
        });
        return data;
    }
    
    public override DataCollection ApplyTotalSummary(DataCollection data)
    {
        if (!HasTotalSummary)
        {
            return data;
        }
        
        var durationCollection = _taskAllocations.SelectMany(taskAllocations => taskAllocations.Value)
                                                                      .Select(taskAllocations => taskAllocations.EndDate - taskAllocations.StartDate)
                                                                      .Where(duration => duration != null)
                                                                      .Select(duration => duration!.Value);
        var totalDuration = durationCollection.Aggregate((total, next) => total + next);
        
        data.Rows.Add(new DataRow());
        data.Rows.Add(new DataRow
        {
            Cells = new List<object> { "Total Duration", "", "", "", "", string.Format(DurationFormat, totalDuration.TotalHours) }
        });
        
        return data;
    }
    
    public override DataCollection ApplyDistributionNote(DataCollection data)
    {
        if (string.IsNullOrEmpty(DistributionNote))
        {
            return data;
        }
        
        data.Rows.Add(new DataRow());
        data.Rows.Add(new DataRow
        {
            Cells = new List<object> { DistributionNote }
        });
        return data;
    }
    
    private const string DurationFormat = "{0:F}h";
    public override DataCollection GetReportData()
    {
        var data = new DataCollection();
        foreach (var allocationEntry in _taskAllocations)
        {
            var rowCollection = allocationEntry.Value.Select(taskAllocation =>
            {
                var duration = taskAllocation.EndDate - taskAllocation.StartDate;
                return new DataRow
                {
                    Cells = new List<object>
                    {
                        taskAllocation.Task.Title,
                        taskAllocation.Task.Status.Name,
                        taskAllocation.User.Name,
                        taskAllocation.StartDate.ToString(),
                        taskAllocation.EndDate.ToString(),
                        string.Format(DurationFormat, duration?.TotalHours)
                    }
                };
            });

            rowCollection = ApplySectionSummary(allocationEntry, rowCollection);
            
            data.Rows.AddRange(rowCollection);
        }
        return data;
    }

    private IEnumerable<DataRow> ApplySectionSummary(KeyValuePair<long, List<TaskAllocation>> allocationEntry, IEnumerable<DataRow> rowCollection)
    {
        if (HasSectionSummary && allocationEntry.Value.Count != 0)
        {
            var durationCollection = allocationEntry.Value.Select(taskAllocations => taskAllocations.EndDate - taskAllocations.StartDate)
                                                                                .Where(duration => duration != null)
                                                                                .Select(duration => duration!.Value);
                                                                                
            var totalDuration = durationCollection.Aggregate((total, next) => total + next);
            rowCollection = rowCollection.Append(new DataRow
            {
                Cells = new List<object> { allocationEntry.Value.First().Task.Title, "Duration","","","", string.Format(DurationFormat, totalDuration.TotalHours) }
            });
        }
        
        rowCollection = rowCollection.Append(new DataRow());

        return rowCollection;
    }
}