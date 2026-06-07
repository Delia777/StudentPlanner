using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using StudentPlanner.Models;
using StudentPlanner.Services;

namespace StudentPlanner.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly JsonTaskService _jsonService = new JsonTaskService();
    private readonly JsonStudySessionService _studyService = new JsonStudySessionService();

    private readonly DispatcherTimer _pomodoroTimer = new DispatcherTimer();
    private int _pomodoroSecunde = 25 * 60;
    private string _pomodoroStatus = "Gata de start";

    private string _titluNou = string.Empty;
    private string _materieNoua = string.Empty;
    private DateTimeOffset? _deadlineNou = DateTimeOffset.Now;
    private string _statusNou = "To Do";
    private string _descriereNoua = string.Empty;
    private TaskItem? _taskSelectat;
    private string _textCautare = string.Empty;
    private string _filtruStatus = "Toate";

    private string _materieSesiune = string.Empty;
    private int _durataMinute = 25;
    private DateTimeOffset? _dataSesiune = DateTimeOffset.Now;
    private string _notiteSesiune = string.Empty;
    private StudySession? _sesiuneSelectata;

    public ObservableCollection<TaskItem> Tasks { get; set; }
    public ObservableCollection<TaskItem> TasksAfisate { get; set; } = new();
    public ObservableCollection<StudySession> StudySessions { get; set; }

    public ObservableCollection<string> Statusuri { get; set; } =
        new ObservableCollection<string> { "To Do", "In Progress", "Done" };

    public ObservableCollection<string> StatusuriFiltru { get; set; } =
        new ObservableCollection<string> { "Toate", "To Do", "In Progress", "Done" };

    public RelayCommand AdaugaTaskCommand { get; }
    public RelayCommand StergeTaskCommand { get; }
    public RelayCommand MarcheazaDoneCommand { get; }

    public RelayCommand AdaugaSesiuneCommand { get; }
    public RelayCommand StergeSesiuneCommand { get; }

    public RelayCommand StartPomodoroCommand { get; }
    public RelayCommand PauzaPomodoroCommand { get; }
    public RelayCommand ResetPomodoroCommand { get; }

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

    public string MaterieSesiune
    {
        get => _materieSesiune;
        set => SetProperty(ref _materieSesiune, value);
    }

    public int DurataMinute
    {
        get => _durataMinute;
        set => SetProperty(ref _durataMinute, value);
    }

    public DateTimeOffset? DataSesiune
    {
        get => _dataSesiune;
        set => SetProperty(ref _dataSesiune, value);
    }

    public string NotiteSesiune
    {
        get => _notiteSesiune;
        set => SetProperty(ref _notiteSesiune, value);
    }

    public StudySession? SesiuneSelectata
    {
        get => _sesiuneSelectata;
        set => SetProperty(ref _sesiuneSelectata, value);
    }

    public string PomodoroDisplay => $"{_pomodoroSecunde / 60:00}:{_pomodoroSecunde % 60:00}";

    public string PomodoroStatus
    {
        get => _pomodoroStatus;
        set => SetProperty(ref _pomodoroStatus, value);
    }

    public int TotalTaskuri => Tasks.Count;
    public int TaskuriFinalizate => Tasks.Count(t => t.Status == "Done");
    public int TaskuriRestante => Tasks.Count(t => t.Status != "Done");

    public int TotalSesiuni => StudySessions.Count;
    public int TotalMinuteInvatate => StudySessions.Sum(s => s.DurataMinute);

    public MainWindowViewModel()
    {
        Tasks = new ObservableCollection<TaskItem>(_jsonService.LoadTasks());
        StudySessions = new ObservableCollection<StudySession>(_studyService.LoadSessions());

        AdaugaTaskCommand = new RelayCommand(AdaugaTask);
        StergeTaskCommand = new RelayCommand(StergeTask);
        MarcheazaDoneCommand = new RelayCommand(MarcheazaDone);

        AdaugaSesiuneCommand = new RelayCommand(AdaugaSesiune);
        StergeSesiuneCommand = new RelayCommand(StergeSesiune);

        StartPomodoroCommand = new RelayCommand(StartPomodoro);
        PauzaPomodoroCommand = new RelayCommand(PauzaPomodoro);
        ResetPomodoroCommand = new RelayCommand(ResetPomodoro);

        _pomodoroTimer.Interval = TimeSpan.FromSeconds(1);
        _pomodoroTimer.Tick += PomodoroTick;

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
        ReseteazaCampuriTask();
        ActualizeazaDashboardTaskuri();
        ActualizeazaListaAfisata();
    }

    private void StergeTask()
    {
        if (TaskSelectat == null)
            return;

        Tasks.Remove(TaskSelectat);
        TaskSelectat = null;

        SalveazaTaskuri();
        ActualizeazaDashboardTaskuri();
        ActualizeazaListaAfisata();
    }

    private void MarcheazaDone()
    {
        if (TaskSelectat == null)
            return;

        TaskSelectat.Status = "Done";

        SalveazaTaskuri();
        ActualizeazaDashboardTaskuri();
        ActualizeazaListaAfisata();
    }

    private void AdaugaSesiune()
    {
        if (string.IsNullOrWhiteSpace(MaterieSesiune) || DurataMinute <= 0)
            return;

        StudySessions.Add(new StudySession
        {
            Materie = MaterieSesiune,
            DurataMinute = DurataMinute,
            Data = DataSesiune?.DateTime ?? DateTime.Now,
            Notite = NotiteSesiune
        });

        SalveazaSesiuni();
        ReseteazaCampuriSesiune();
        ActualizeazaDashboardSesiuni();
    }

    private void StergeSesiune()
    {
        if (SesiuneSelectata == null)
            return;

        StudySessions.Remove(SesiuneSelectata);
        SesiuneSelectata = null;

        SalveazaSesiuni();
        ActualizeazaDashboardSesiuni();
    }

    private void StartPomodoro()
    {
        if (_pomodoroSecunde <= 0)
            _pomodoroSecunde = 25 * 60;

        PomodoroStatus = "Timer pornit";
        _pomodoroTimer.Start();
        OnPropertyChanged(nameof(PomodoroDisplay));
    }

    private void PauzaPomodoro()
    {
        _pomodoroTimer.Stop();
        PomodoroStatus = "Timer pe pauza";
    }

    private void ResetPomodoro()
    {
        _pomodoroTimer.Stop();
        _pomodoroSecunde = 25 * 60;
        PomodoroStatus = "Gata de start";
        OnPropertyChanged(nameof(PomodoroDisplay));
    }

    private void PomodoroTick(object? sender, EventArgs e)
    {
        if (_pomodoroSecunde > 0)
        {
            _pomodoroSecunde--;
            OnPropertyChanged(nameof(PomodoroDisplay));
        }

        if (_pomodoroSecunde == 0)
        {
            _pomodoroTimer.Stop();
            PomodoroStatus = "Sesiune finalizata";
        }
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

    private void ReseteazaCampuriTask()
    {
        TitluNou = string.Empty;
        MaterieNoua = string.Empty;
        DescriereNoua = string.Empty;
        StatusNou = "To Do";
        DeadlineNou = DateTimeOffset.Now;
    }

    private void ReseteazaCampuriSesiune()
    {
        MaterieSesiune = string.Empty;
        DurataMinute = 25;
        DataSesiune = DateTimeOffset.Now;
        NotiteSesiune = string.Empty;
    }

    private void ActualizeazaDashboardTaskuri()
    {
        OnPropertyChanged(nameof(TotalTaskuri));
        OnPropertyChanged(nameof(TaskuriFinalizate));
        OnPropertyChanged(nameof(TaskuriRestante));
    }

    private void ActualizeazaDashboardSesiuni()
    {
        OnPropertyChanged(nameof(TotalSesiuni));
        OnPropertyChanged(nameof(TotalMinuteInvatate));
    }

    private void SalveazaTaskuri()
    {
        _jsonService.SaveTasks(Tasks.ToList());
    }

    private void SalveazaSesiuni()
    {
        _studyService.SaveSessions(StudySessions.ToList());
    }
}