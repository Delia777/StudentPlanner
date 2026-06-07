using System;
using System.Collections.ObjectModel;
using System.Linq;
using StudentPlanner.Models;
using StudentPlanner.Services;

namespace StudentPlanner.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly JsonTaskService _jsonService = new JsonTaskService();

    public ObservableCollection<TaskItem> Tasks { get; set; }

    public string TitluNou { get; set; } = string.Empty;
    public string MaterieNoua { get; set; } = string.Empty;
    public DateTimeOffset? DeadlineNou { get; set; } = DateTimeOffset.Now;
    public string StatusNou { get; set; } = "To Do";
    public string DescriereNoua { get; set; } = string.Empty;

    public ObservableCollection<string> Statusuri { get; set; } =
        new ObservableCollection<string> { "To Do", "In Progress", "Done" };

    public TaskItem? TaskSelectat { get; set; }

    public int TotalTaskuri => Tasks.Count;
    public int TaskuriFinalizate => Tasks.Count(t => t.Status == "Done");
    public int TaskuriRestante => Tasks.Count(t => t.Status != "Done");

    public MainWindowViewModel()
    {
        Tasks = new ObservableCollection<TaskItem>(_jsonService.LoadTasks());
    }

    public void AdaugaTask()
    {
        if (string.IsNullOrWhiteSpace(TitluNou))
            return;

        Tasks.Add(new TaskItem
        {
            Titlu = TitluNou,
            Materie = MaterieNoua,
            Deadline = DeadlineNou?.DateTime ?? DateTime.Now,
            Status = StatusNou,
            Descriere = DescriereNoua
        });

        SalveazaTaskuri();

        TitluNou = string.Empty;
        MaterieNoua = string.Empty;
        DescriereNoua = string.Empty;
        StatusNou = "To Do";
        DeadlineNou = DateTimeOffset.Now;
    }

    public void StergeTask()
    {
        if (TaskSelectat == null)
            return;

        Tasks.Remove(TaskSelectat);
        SalveazaTaskuri();
    }

    public void MarcheazaDone()
    {
        if (TaskSelectat == null)
            return;

        TaskSelectat.Status = "Done";
        SalveazaTaskuri();
    }

    private void SalveazaTaskuri()
    {
        _jsonService.SaveTasks(Tasks.ToList());
    }
}