using System.IO;
using System.Text.Json;

namespace OkGoogle.Infrastructure;

public enum LogLevel
{
    DEBUG,
    INFO,
    WARNING,
    ERROR
}

public static class AppLogger
{
    private static readonly object LockObj = new();
    private static readonly string LogFilePath;

    static AppLogger()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dir = Path.Combine(appData, "OkGoogle", "logs");
        Directory.CreateDirectory(dir);
        LogFilePath = Path.Combine(dir, $"app-{DateTime.Now:yyyy-MM-dd}.log");
    }

    public static void Debug(string message, object? meta = null) => Write(LogLevel.DEBUG, message, meta);
    public static void Info(string message, object? meta = null) => Write(LogLevel.INFO, message, meta);
    public static void Warning(string message, object? meta = null) => Write(LogLevel.WARNING, message, meta);
    public static void Error(string message, Exception? ex = null, object? meta = null)
    {
        Write(LogLevel.ERROR, message, new
        {
            ExceptionType = ex?.GetType().Name,
            ExceptionMessage = ex?.Message,
            Meta = meta
        });
    }

    private static void Write(LogLevel level, string message, object? meta)
    {
        try
        {
            var entry = new
            {
                timestamp = DateTimeOffset.Now.ToString("O"),
                level = level.ToString(),
                message,
                meta
            };
            var line = JsonSerializer.Serialize(entry) + Environment.NewLine;
            lock (LockObj)
            {
                File.AppendAllText(LogFilePath, line);
            }
        }
        catch
        {
            // Никогда не роняем приложение из-за ошибки записи лога
        }
    }
}
