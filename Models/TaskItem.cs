using System;

namespace StudentPlanner.Models;

public class TaskItem
{
    public string Titlu { get; set; } = string.Empty;

    public string Materie { get; set; } = string.Empty;

    public DateTime Deadline { get; set; }

    public string Status { get; set; } = "To Do";

    public string Descriere { get; set; } = string.Empty;
}