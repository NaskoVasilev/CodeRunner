﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CodeManager.Enums;

namespace CodeManager.Executors
{
    public class BaseExecutor : IExecutor
    {
        private const int ProcessMaxRunningTime = 2000;
        private const int TimeLimit = 100;
        private const int MemoryLimit = 5000000;

        public static IExecutor CreateExecutor(CompilerType compilerType)
        {
            switch (compilerType)
            {
                case CompilerType.CSharp:
                    return new CSharpExecutor();
                case CompilerType.CPlusPlus:
                    return new CPlusPlusExecutor();
                case CompilerType.Java:
                    return new JavaExecutor();
                default:
                    throw new ArgumentException("Unsupported compiler.");
            }
        }

        public virtual async Task<ProcessExecutionResult> Execute(string input, string fileName, string workingDirectory, string arguments)
        {
            ProcessExecutionResult processExecutionResult = new ProcessExecutionResult();

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = arguments;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;

                process.Start();

                const int TimeIntervalBetweenTwoMemoryConsumptionRequests = 45;
                var memoryTaskCancellationToken = new CancellationTokenSource();
                var memoryTask = Task.Run(
                    () =>
                    {
                        while (true)
                        {
                            if (process.HasExited)
                            {
                                return;
                            }

                            processExecutionResult.MemoryUsed = Math.Max(processExecutionResult.MemoryUsed, process.PeakWorkingSet64);

                            if (memoryTaskCancellationToken.IsCancellationRequested)
                            {
                                return;
                            }

                            Thread.Sleep(TimeIntervalBetweenTwoMemoryConsumptionRequests);
                        }
                    },
                    memoryTaskCancellationToken.Token);

                if (!string.IsNullOrEmpty(input))
                {
                    await process.StandardInput.WriteLineAsync(input);
                    await process.StandardInput.FlushAsync();
                    process.StandardInput.Close();
                }

                bool exited = process.WaitForExit(ProcessMaxRunningTime);

                if (!exited)
                {
                    process.Kill();
                    processExecutionResult.Type = ProcessExecutionResultType.TimeLimit;
                }
                // Close the memory consumption check thread
                memoryTaskCancellationToken.Cancel();

                string output = await process.StandardOutput.ReadToEndAsync();
                string errors = await process.StandardError.ReadToEndAsync();

                processExecutionResult.ErrorOutput = errors;
                processExecutionResult.ReceivedOutput = output;
                processExecutionResult.ExitCode = process.ExitCode;
                processExecutionResult.TimeWorked = process.ExitTime - process.StartTime;
                processExecutionResult.PrivilegedProcessorTime = process.PrivilegedProcessorTime;
                processExecutionResult.UserProcessorTime = process.UserProcessorTime;

                if (processExecutionResult.TotalProcessorTime.TotalMilliseconds > TimeLimit)
                {
                    processExecutionResult.Type = ProcessExecutionResultType.TimeLimit;
                }

                if (!string.IsNullOrEmpty(processExecutionResult.ErrorOutput))
                {
                    processExecutionResult.Type = ProcessExecutionResultType.RunTimeError;
                }

                if (processExecutionResult.MemoryUsed > MemoryLimit)
                {
                    processExecutionResult.Type = ProcessExecutionResultType.MemoryLimit;
                }

                return processExecutionResult;
            }
        }
    }
}
