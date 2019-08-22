using System;
using System.Collections.Generic;
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
            string input = "test";
            string source = @"
                            import java.util.Scanner; 
                            public class Task
                            {
	                            public static void main(String[] args) {
		                            Scanner in = new Scanner(System.in); 
         
                                    // Reading data using readLine 
                                     String s = in.nextLine();  
  
                                    // Printing the read line 
                                    System.out.println(s);         
                                }
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
            string fileArguments = $"/c cd {compiledFilePath} & set PATH=%PATH%;C:\\Program Files\\Java\\jdk1.8.0_181\\bin; & java -Xmx128m -jar -Djava.security.manager -Djava.security.policy==Task.policy Task.jar ";
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
            if(!string.IsNullOrEmpty(errors))
            {
                throw new ArgumentException(errors);
            }

            return output;
        }

        private static string Compile(string source)
        {
            Directory.CreateDirectory(compiledFilePath);
            string taskPolicy = compiledFilePath +  compiledFileName + ".policy";
            File.WriteAllText(taskPolicy, "grant {};");

            string javaFile = compiledFilePath + compiledFileName + ".java";
            File.WriteAllText(javaFile, source);
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = $"/c cd {compiledFilePath} & set PATH=%PATH%;C:\\Program Files\\Java\\jdk1.8.0_181\\bin; & javac Task.java & jar cvfe Task.jar Task Task.class",
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
