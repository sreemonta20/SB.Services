using Serilog;

namespace SBERP.HumanResources.Service
{
    public interface IHRLogService
    {
        void LogDebug(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogCritical(string message);
    }

    public class HRLogService : IHRLogService
    {
        public void LogDebug(string message)    => Log.Debug(message);
        public void LogInfo(string message)     => Log.Information(message);
        public void LogWarning(string message)  => Log.Warning(message);
        public void LogError(string message)    => Log.Error(message);
        public void LogCritical(string message) => Log.Fatal(message);
    }
}
