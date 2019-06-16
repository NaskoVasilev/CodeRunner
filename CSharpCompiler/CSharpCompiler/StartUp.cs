using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharpCompiler
{
	public class StartUp
	{
		private const int ProcessMaxRunningTime = 10000;
		private const int TimeLimit = 100;
		private const int MemoryLimit = 5000000;

		public static void Main(string[] args)
		{
			string input = "Pesho\r\n15\r\nPlovdiv";
			string expectedOutput = $"I am Pesho - 15 years old. I am from Plovdiv.";

			string fileName = "Program.cs";
			string filePath = $"../../../../Test/{fileName}";
			string sourceCode = GetCSharpTestCode(filePath);
			string assemblyName = "Program";
			CSharpCompilation compilation = BuildCSharpCompilation(sourceCode, assemblyName);

			string outputDllName = assemblyName + ".dll";
			string dllFilePath = new FileInfo(outputDllName).FullName;
			string workingDirectory = new FileInfo(outputDllName).DirectoryName;
			EmitResult emitResult = compilation.Emit(outputDllName);
			string runtimeConfigJsonFileContent = GenerateRuntimeConfigJsonFile();
			string runtimeConfigJsonFilePath = workingDirectory + "\\" + assemblyName + ".runtimeconfig.json";
			File.WriteAllText(runtimeConfigJsonFilePath, runtimeConfigJsonFileContent);

			if (!emitResult.Success)
			{
				StringBuilder errors = new StringBuilder();

				IEnumerable<Diagnostic> failures = emitResult.Diagnostics
					.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

				foreach (Diagnostic diagnostic in failures)
				{
					errors.AppendLine($"{diagnostic.Location.GetLineSpan()} {diagnostic.GetMessage()}");
				}

				Console.WriteLine("Compile time error");
				Console.WriteLine(errors.ToString());
				return;
			}

			ProcessExecutionResult processExecutionResult = ProcessResult(input, dllFilePath).GetAwaiter().GetResult();

			if (processExecutionResult.IsSuccesfull && processExecutionResult.Type == ProcessExecutionResultType.Success)
			{
				Console.WriteLine("The application was run seccessfully!");
				Console.WriteLine(processExecutionResult.ReceivedOutput);
				Console.WriteLine($"Is test correct: {processExecutionResult.ReceivedOutput.Trim() == expectedOutput}");
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
		}

		private static CSharpCompilation BuildCSharpCompilation(string sourceCode, string assemblyName)
		{
			CSharpCompilation compilation = CSharpCompilation.Create(assemblyName)
						.WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication))
						.AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

			var netStandardAssembly = Assembly.Load(new AssemblyName("netstandard"));
			compilation = compilation.AddReferences(MetadataReference.CreateFromFile(netStandardAssembly.Location));
			AssemblyName[] netStandardAssemblies = netStandardAssembly.GetReferencedAssemblies();

			foreach (var assembly in netStandardAssemblies)
			{
				string assemblyLocation = Assembly.Load(assembly).Location;
				compilation = compilation.AddReferences(MetadataReference.CreateFromFile(assemblyLocation));
			}

			SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(sourceCode);
			compilation = compilation.AddSyntaxTrees(syntaxTree);
			return compilation;
		}

		private static async Task<ProcessExecutionResult> ProcessResult(string input, string dllFilePath)
		{
			ProcessExecutionResult processExecutionResult = new ProcessExecutionResult();
			string commandPromptArgument = @"/C dotnet " + dllFilePath;

			using (Process process = new Process())
			{
				process.StartInfo.FileName = "cmd.exe";
				process.StartInfo.Arguments = commandPromptArgument;
				process.StartInfo.RedirectStandardInput = true;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.UseShellExecute = false;

				process.Start();
				process.PriorityClass = ProcessPriorityClass.High;
				await process.StandardInput.WriteLineAsync(input);
				await process.StandardInput.FlushAsync();
				process.StandardInput.Close();
				processExecutionResult.MemoryUsed = process.PrivateMemorySize64;
				bool exited = process.WaitForExit(ProcessMaxRunningTime);

				if (!exited)
				{
					process.Kill();
					processExecutionResult.Type = ProcessExecutionResultType.TimeLimit;
				}

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

		private static string GenerateRuntimeConfigJsonFile()
		{
			var runtimeConfigJson = new
			{
				RuntimeOptions = new
				{
					Tfm = "netcoreapp2.2",
					Framework = new
					{
						Name = "Microsoft.NETCore.App",
						Version = "2.2.0"
					}
				}
			};

			string runtimeConfigJsonFileContent = JsonConvert.SerializeObject(runtimeConfigJson, new JsonSerializerSettings()
			{
				ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new CamelCaseNamingStrategy()
				},
				Formatting = Formatting.Indented
			});

			return runtimeConfigJsonFileContent;
		}

		private static string GetCSharpTestCode(string filePath)
		{
			string source = File.ReadAllText(filePath);
			return source;
		}
	}
}
