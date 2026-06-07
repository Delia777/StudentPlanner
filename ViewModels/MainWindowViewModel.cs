using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentPlanner.Models;
using StudentPlanner.Services;

namespace StudentPlanner.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly JsonTaskService _jsonService = new JsonTaskService();

    public ObservableCollection<TaskItem> Tasks { get; set; }

    public ObservableCollection<string> Statusuri { get; set; } =
        new ObservableCollection<string> { "To Do", "In Progress", "Done" };

    [ObservableProperty]
    private string titluNou = string.Empty;

    [ObservableProperty]
    private string materieNoua = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? deadlineNou = DateTimeOffset.Now;

    [ObservableProperty]
    private string statusNou = "To Do";

    [ObservableProperty]
    private string descriereNoua = string.Empty;

    [ObservableProperty]
    private TaskItem? taskSelectat;

    public int TotalTaskuri => Tasks.Count;
    public int TaskuriFinalizate => Tasks.Count(t => t.Status == "Done");
    public int TaskuriRestante => Tasks.Count(t => t.Status != "Done");

    public MainWindowViewModel()
    {
        Tasks = new ObservableCollection<TaskItem>(_jsonService.LoadTasks());
    }

    [RelayCommand]
    private void AdaugaTask()
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
        ReseteazaCampuri();
        ActualizeazaDashboard();
    }

    [RelayCommand]
    private void StergeTask()
    {
        if (TaskSelectat == null)
            return;

        Tasks.Remove(TaskSelectat);
        TaskSelectat = null;

        SalveazaTaskuri();
        ActualizeazaDashboard();
    }

    [RelayCommand]
    private void MarcheazaDone()
    {
        if (TaskSelectat == null)
            return;

        TaskSelectat.Status = "Done";

        SalveazaTaskuri();
        ActualizeazaDashboard();
    }

    private void ReseteazaCampuri()
    {
        TitluNou = string.Empty;
        MaterieNoua = string.Empty;
        DescriereNoua = string.Empty;
        StatusNou = "To Do";
        DeadlineNou = DateTimeOffset.Now;
    }

    private void ActualizeazaDashboard()
    {
        OnPropertyChanged(nameof(TotalTaskuri));
        OnPropertyChanged(nameof(TaskuriFinalizate));
        OnPropertyChanged(nameof(TaskuriRestante));
    }

    private void SalveazaTaskuri()
    {
        _jsonService.SaveTasks(Tasks.ToList());
    }
}