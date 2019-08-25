using CodeManager.Compilers;
using CodeManager.Enums;
using CodeManager.Executors;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeManager
{
    class StartUp
    {
        static void Main(string[] args)
        {
            CompilerType compilerType = ReadCompilerType();
            ICompiler compiler = BaseCompiler.CreateCompiler(compilerType);

            Console.WriteLine("Enter file path.");
            string sourceFilePath = Console.ReadLine();
            string source = File.ReadAllText(sourceFilePath);

            string fileName = "HelloWorld";
            string workingDirectory = Environment.CurrentDirectory + "\\" + Guid.NewGuid().ToString();
            Directory.CreateDirectory(workingDirectory);

            WriteSourceToFile(source, compilerType, workingDirectory, fileName);
            CompileResult compileResult = compiler.Compile(new CompilerArguments(fileName, workingDirectory, new List<string> { source }));

            if (!compileResult.IsSuccessfull)
            {
                Console.WriteLine($"Compile time error: {compileResult.Errors}");
                return;
            }

            IExecutor executor = BaseExecutor.CreateExecutor(compilerType);
            string compiledFileName = Path.GetFileName(compileResult.CompiledFilePath);
            string compiledFileDirectory = Path.GetDirectoryName(compileResult.CompiledFilePath);

            Console.WriteLine("Enter inputs to test the program. If you want to stop enter 'stop'!");
            string input = string.Empty;
            while ((input = Console.ReadLine()) != "stop")
            {
                ProcessExecutionResult processExecutionResult = executor.Execute(input, compiledFileName, compiledFileDirectory).GetAwaiter().GetResult();

                if (processExecutionResult.IsSuccesfull && processExecutionResult.Type == ProcessExecutionResultType.Success)
                {
                    Console.WriteLine("The application was run seccessfully!");
                    Console.WriteLine(processExecutionResult.ReceivedOutput);
                }
                else
                {
                    Console.WriteLine("Some errors occured!");
                    Console.WriteLine(processExecutionResult.ErrorOutput);
                }

                Console.WriteLine("Process statistics");
                Console.WriteLine($"Exit code: {processExecutionResult.ExitCode}");
                Console.WriteLine($"Memory Used: {processExecutionResult.MemoryUsed}");
                Console.WriteLine($"Processor time used: {processExecutionResult.TotalProcessorTime}");
                Console.WriteLine($"Working time: {processExecutionResult.TimeWorked}");
                Console.WriteLine($"Execution Type: {processExecutionResult.Type}");
                Console.WriteLine(new string('-', 50));
            }
        }

        private static void WriteSourceToFile(string source, CompilerType compilerType, string workingDirectory, string fileName)
        {
            workingDirectory += "\\";
            if (compilerType == CompilerType.CPlusPlus)
            {
                string cppFile = workingDirectory + fileName + CompilationConstants.CppFileExtnsion;
                File.WriteAllText(cppFile, source);
            }
            else if (compilerType == CompilerType.Java)
            {
                string taskPolicy = workingDirectory + fileName + ".policy";
                File.WriteAllText(taskPolicy, "grant {};");

                string javaFile = workingDirectory + fileName + CompilationConstants.JavaFileExtension;
                File.WriteAllText(javaFile, source);
            }
        }

        private static CompilerType ReadCompilerType()
        {

            int counter = 1;
            foreach (string item in Enum.GetNames(typeof(CompilerType)))
            {
                Console.WriteLine($"Enter {counter++} if you want to execute {item}");
            }

            int language = int.Parse(Console.ReadLine());
            if (language < 1 || language >= counter)
            {
                throw new ArgumentException("Invalid compiler type");
            }
            CompilerType compilerType = (CompilerType)language;
            return compilerType;
        }
    }
}
