using UnityEngine;

namespace Nebula.Modules;

public enum LogLevel
{
    Debug,
    Info,
    Warn,
    Error
}
public static class NebulaLogger
{
    private static readonly List<string> _buffer = new();

    private static string _dataFolder = "";

    private static string _logsFolder = "";

    private static DateTime _sessionStart;

    public static bool _initialized;

    private static bool _sessionActive;

    private static int _entryCount;

    private static StreamWriter? _writer;

    public static void Init()
    {
        if (_initialized)
            return;

        _initialized = true;

        string root = Path.GetDirectoryName(Application.dataPath)!;

        _dataFolder = Path.Combine(root, "Nebula-Data");
        _logsFolder = Path.Combine(_dataFolder, "Logs");
        
        Directory.CreateDirectory(_dataFolder);
        Directory.CreateDirectory(_logsFolder);        
    }

    public static void StartLog()
    {
        if (!_initialized)
            return;

        if (_sessionActive)
            return;

        _sessionStart = DateTime.Now;

        string fileName = $"{_sessionStart.ToString("yyyy-MM-dd_HH-mm-ss")}.log";
        string logPath = Path.Combine(_logsFolder,fileName);

        _writer = new StreamWriter(logPath);

        _sessionActive = true;

        WriteHeader();
    }
    public static void WriteHeader(string lobbyCode = "Unknown")
    {
        _writer.WriteLine("========================================");
        _writer.WriteLine("Nebula Session Log");
        _writer.WriteLine("========================================");

        _writer.WriteLine($"Started : {_sessionStart}");
        _writer.WriteLine($"Version : {Main.PluginDisplayVersion} ({Main.PluginVersion})");
        _writer.WriteLine($"Lobby   : {lobbyCode}");        
        _writer.WriteLine("========================================");
        _writer.WriteLine("");

        _writer.WriteLine("Plugin", $"Nebula {Main.PluginVersion} initialized.");
        _writer.WriteLine("Environment", $"Unity {Application.unityVersion}");
        _writer.WriteLine("Environment", $"OS: {SystemInfo.operatingSystem}");
        _writer.WriteLine("Session", "Session started.");
    }
    public static void EndLog()
    {
        if (!_sessionActive)
            return;

        _writer.WriteLine("");
        _writer.WriteLine("========================================");
        _writer.WriteLine($"Session Close Time: {DateTime.Now}");
        _writer.WriteLine($"Total Entries: {_entryCount}");
        _writer.WriteLine("========================================");

        _writer?.Dispose();

        _writer = null;
        _sessionActive = false;
        _entryCount = 0;
    }
}