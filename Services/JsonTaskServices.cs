using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using StudentPlanner.Models;

namespace StudentPlanner.Services;

public class JsonTaskService
{
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "tasks.json");

    public List<TaskItem> LoadTasks()
    {
        if (!File.Exists(_filePath))
            return new List<TaskItem>();

        string json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<TaskItem>();

        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
    }

    public void SaveTasks(List<TaskItem> tasks)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(tasks, options);
        File.WriteAllText(_filePath, json);
    }
}