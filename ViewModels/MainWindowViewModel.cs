using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using StudentPlanner.Models;
using StudentPlanner.Services;

namespace StudentPlanner.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly JsonTaskService _jsonService = new JsonTaskService();

    private string _titluNou = string.Empty;
    private string _materieNoua = string.Empty;
    private DateTimeOffset? _deadlineNou = DateTimeOffset.Now;
    private string _statusNou = "To Do";
    private string _descriereNoua = string.Empty;
    private TaskItem? _taskSelectat;
    private string _textCautare = string.Empty;
    private string _filtruStatus = "Toate";

    public ObservableCollection<TaskItem> Tasks { get; set; }
    public ObservableCollection<TaskItem> TasksAfisate { get; set; } = new();

    public ObservableCollection<string> Statusuri { get; set; } =
        new ObservableCollection<string> { "To Do", "In Progress", "Done" };

    public ObservableCollection<string> StatusuriFiltru { get; set; } =
        new ObservableCollection<string> { "Toate", "To Do", "In Progress", "Done" };

    public RelayCommand AdaugaTaskCommand { get; }
    public RelayCommand StergeTaskCommand { get; }
    public RelayCommand MarcheazaDoneCommand { get; }

    public string TitluNou
    {
        get => _titluNou;
        set => SetProperty(ref _titluNou, value);
    }

    public string MaterieNoua
    {
        get => _materieNoua;
        set => SetProperty(ref _materieNoua, value);
    }

    public DateTimeOffset? DeadlineNou
    {
        get => _deadlineNou;
        set => SetProperty(ref _deadlineNou, value);
    }

    public string StatusNou
    {
        get => _statusNou;
        set => SetProperty(ref _statusNou, value);
    }

    public string DescriereNoua
    {
        get => _descriereNoua;
        set => SetProperty(ref _descriereNoua, value);
    }

    public TaskItem? TaskSelectat
    {
        get => _taskSelectat;
        set => SetProperty(ref _taskSelectat, value);
    }

    public string TextCautare
    {
        get => _textCautare;
        set
        {
            SetProperty(ref _textCautare, value);
            ActualizeazaListaAfisata();
        }
    }

    public string FiltruStatus
    {
        get => _filtruStatus;
        set
        {
            SetProperty(ref _filtruStatus, value);
            ActualizeazaListaAfisata();
        }
    }

    public int TotalTaskuri => Tasks.Count;
    public int TaskuriFinalizate => Tasks.Count(t => t.Status == "Done");
    public int TaskuriRestante => Tasks.Count(t => t.Status != "Done");

    public MainWindowViewModel()
    {
        Tasks = new ObservableCollection<TaskItem>(_jsonService.LoadTasks());

        AdaugaTaskCommand = new RelayCommand(AdaugaTask);
        StergeTaskCommand = new RelayCommand(StergeTask);
        MarcheazaDoneCommand = new RelayCommand(MarcheazaDone);

        ActualizeazaListaAfisata();
    }

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
        ActualizeazaListaAfisata();
    }

    private void StergeTask()
    {
        if (TaskSelectat == null)
            return;

        Tasks.Remove(TaskSelectat);
        TaskSelectat = null;

        SalveazaTaskuri();
        ActualizeazaDashboard();
        ActualizeazaListaAfisata();
    }

    private void MarcheazaDone()
    {
        if (TaskSelectat == null)
            return;

        TaskSelectat.Status = "Done";

        SalveazaTaskuri();
        ActualizeazaDashboard();
        ActualizeazaListaAfisata();
    }

    private void ActualizeazaListaAfisata()
    {
        var rezultate = Tasks.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(TextCautare))
        {
            rezultate = rezultate.Where(t =>
                t.Titlu.Contains(TextCautare, StringComparison.OrdinalIgnoreCase));
        }

        if (FiltruStatus != "Toate")
        {
            rezultate = rezultate.Where(t => t.Status == FiltruStatus);
        }

        TasksAfisate.Clear();

        foreach (var task in rezultate)
        {
            TasksAfisate.Add(task);
        }
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