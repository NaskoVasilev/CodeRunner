using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace JavaCompiler
{
    public class Program
    {
        private const int ProcessMaxRunningTime = 2000;
        private static readonly string compiledFileName = "Task";
        private static readonly string compiledFilePath = $"{Directory.GetCurrentDirectory()}\\{Guid.NewGuid()}\\";

        static void Main(string[] args)
        {
            string input = "45";
            string source = @"#include <iostream>
                            using namespace std;
                            int main()
                            {  
                                 int number;
                                 cin >> number;
                                 cout << number;
                            }";

            string errors = Compile(source);
            if (!string.IsNullOrEmpty(errors.Trim()))
            {
                Console.WriteLine(errors);
                return;
            }

            string output = RunProcess(input).GetAwaiter().GetResult();
            Console.WriteLine(output);
        }

        private static async Task<string> RunProcess(string input)
        {
            string fileArguments = $"/c cd {compiledFilePath} & set PATH=%PATH%;C:\\MinGW\\bin; & Task.exe";
            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = fileArguments,
                RedirectStandardInput = true,
                FileName = "cmd.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            Process process = new Process
            {
                StartInfo = info
            };

            process.Start();
            await process.StandardInput.WriteLineAsync(input);
            await process.StandardInput.FlushAsync();
            process.StandardInput.Close();
            bool exited = process.WaitForExit(ProcessMaxRunningTime);

            if (!exited)
            {
                process.Kill();
            }

            string output = await process.StandardOutput.ReadToEndAsync();
            string errors = await process.StandardError.ReadToEndAsync();
            if (!string.IsNullOrEmpty(errors))
            {
                throw new ArgumentException(errors);
            }

            return output;
        }

        private static string Compile(string source)
        {
            Directory.CreateDirectory(compiledFilePath);

            string cppFile = compiledFilePath + compiledFileName + ".cpp";
            File.WriteAllText(cppFile, source);
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = $"/c cd {compiledFilePath} & set PATH=%PATH%;C:\\MinGW\\bin; & g++ Task.cpp -o Task.exe",
                FileName = "cmd.exe",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            process.StartInfo = info;

            process.Start();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return errors;
        }
    }
}
