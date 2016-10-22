namespace BelkaCloudDownloader.Utils
{
    using System;

    public interface IIoLogger
    {
        string LogFile { get; }

        bool IsTraceEnabled { get; }

        void Trace(string message);

        void Trace(string message, params object[] arguments);

        void Flush();

        void LogDebugMessage(string message, params object[] arguments);

        void LogMessage(string message);

        void LogMessage(string message, params object[] arguments);

        void LogError(string message, Exception e, params object[] arguments);

        void LogError(string message, params object[] arguments);

        void LogError(string message, Exception e);
    }
}
