using System;

namespace StudentPlanner.Models;

public class StudySession
{
    public string Materie { get; set; } = string.Empty;

    public int DurataMinute { get; set; }

    public DateTime Data { get; set; }

    public string Notite { get; set; } = string.Empty;
}