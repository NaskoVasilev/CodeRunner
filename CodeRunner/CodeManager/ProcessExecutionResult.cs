using System;

namespace CodeManager
{
    public class ProcessExecutionResult
    {
        public ProcessExecutionResult()
        {
            ReceivedOutput = string.Empty;
            ErrorOutput = string.Empty;
            ExitCode = 0;
            Type = ProcessExecutionResultType.Success;
            TimeWorked = default;
            MemoryUsed = 0;
        }

        public string ReceivedOutput { get; set; }

        public string ErrorOutput { get; set; }

        public int ExitCode { get; set; }

        public ProcessExecutionResultType Type { get; set; }

        public TimeSpan TimeWorked { get; set; }

        public TimeSpan PrivilegedProcessorTime { get; set; }

        public TimeSpan UserProcessorTime { get; set; }

        public long MemoryUsed { get; set; }

        public TimeSpan TotalProcessorTime => PrivilegedProcessorTime + UserProcessorTime;

        public bool IsSuccesfull => string.IsNullOrEmpty(ErrorOutput);
    }
}
