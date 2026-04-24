namespace Logging
{
    public interface ILogsWriter
    {
        void Log(string message);
        
        void LogWarning(string message);
        
        void LogError(string message);
    }
}