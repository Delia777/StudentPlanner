using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using StudentPlanner.Models;

namespace StudentPlanner.Services;

public class JsonStudySessionService
{
    private readonly string _filePath = Path.Combine(AppContext.BaseDirectory, "study_sessions.json");

    public List<StudySession> LoadSessions()
    {
        if (!File.Exists(_filePath))
            return new List<StudySession>();

        string json = File.ReadAllText(_filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<StudySession>();

        return JsonSerializer.Deserialize<List<StudySession>>(json) ?? new List<StudySession>();
    }

    public void SaveSessions(List<StudySession> sessions)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        string json = JsonSerializer.Serialize(sessions, options);
        File.WriteAllText(_filePath, json);
    }
}