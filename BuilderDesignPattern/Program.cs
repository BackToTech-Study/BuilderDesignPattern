// See https://aka.ms/new-console-template for more information

using BuilderDesignPattern.AllocationReports;
using BuilderDesignPattern.AllocationReports.Reports;
using BuilderDesignPattern.Models;
using OfficeOpenXml;
using Task = BuilderDesignPattern.Models.Task;
using TaskStatus = BuilderDesignPattern.Models.TaskStatus;

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var taskAllocations = GetTaskAllocations();
ReportDirector reportDirector = new();
var taskHistoryReportBuilder = new TaskHistoryReportBuilder()
                                    .AppendRecords(taskAllocations)
                                    .SetTitle("Task History Report")
                                    .EnableHeaders()
                                    .EnableTotalSummary()
                                    .EnableSectionSummary()
                                    .AddDistributionNote("This is an internal document. Do not distribute without consent!");

var taskReport = reportDirector.BuildReport(taskHistoryReportBuilder);
var reportFile = taskReport.Save();
Console.WriteLine($"Task Report saved to {reportFile}");

var userAllocationReportBuilder = new UserAllocationReportBuilder()
                                    .AppendRecords(taskAllocations)
                                    .SetTitle("User Allocation Report")
                                    .EnableHeaders()
                                    .EnableTotalSummary()
                                    .EnableSectionSummary()
                                    .AddDistributionNote("This is an internal document. Do not distribute without consent!");

var userReport = reportDirector.BuildReport(userAllocationReportBuilder);
reportFile = userReport.Save();
Console.WriteLine($"User Report saved to {reportFile}");


List<TaskAllocation> GetTaskAllocations()
{
    var taskStatusCollection = new List<TaskStatus>
    {
        new TaskStatus { Id = 1, Name = "Not Started" },
        new TaskStatus { Id = 2, Name = "In Progress" },
        new TaskStatus { Id = 3, Name = "Completed" }
    };

    var dan = new User { Id = 1, Name = "Dan" };
    var john = new User { Id = 2, Name = "John" };

    var tasks = new List<Task>
    {
        new Task { Id = 1, Title = "Task 1", Status = taskStatusCollection[0] },
        new Task { Id = 2, Title = "Task 2", Status = taskStatusCollection[1] },
    };
    
    var taskAllocations = new List<TaskAllocation>
    {
        new TaskAllocation
        {
            Task = tasks[0],
            User = dan,
            StartDate = DateTime.Now.AddDays(-5),
            EndDate = DateTime.Now.AddDays(-2)
        },
        new TaskAllocation
        {
            Task = tasks[0],
            User = john,
            StartDate = DateTime.Now.AddDays(-2),
            EndDate = DateTime.Now
        },
        new TaskAllocation
        {
            Task = tasks[1],
            User = dan,
            StartDate = DateTime.Now.AddDays(-5),
            EndDate = DateTime.Now.AddDays(-2)
        },
        new TaskAllocation
        {
            Task = tasks[1],
            User = john,
            StartDate = DateTime.Now.AddDays(-2),
            EndDate = DateTime.Now
        }
    };
    return taskAllocations;
}