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

namespace CSharpCompiler
{
	public class StartUp
	{
		public static void Main(string[] args)
		{
			string input = "Pesho\r\n15\r\nPlovdiv";
			string expectedOutput = $"I am Pesho - 15 years old. I am from Plovdiv.";

			string fileName = "Program.cs";
			string filePath = $"../../../../Test/{fileName}";
			string sourceCode = GetCSharpTestCode(filePath);
			string assemblyName = "Program";
			CSharpCompilation compilation = BuildCSharpCompilation(sourceCode, assemblyName);

			OutputResult outputResult = ProcessResult(compilation, input, assemblyName);

			if (outputResult.IsSuccesfull)
			{
				Console.WriteLine("The application was run seccessfully!");
				Console.WriteLine(outputResult.Output);
				Console.WriteLine($"Is test correct: {outputResult.Output.Trim() == expectedOutput}");
			}
			else
			{
				Console.WriteLine("Some errors occured!");
				Console.WriteLine(outputResult.Errors);
			}
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

		private static OutputResult ProcessResult(CSharpCompilation compilation, string input, string assemblyName)
		{
			string outputDllName = assemblyName + ".dll";
			string dllFilePath = new FileInfo(outputDllName).FullName;
			string workingDirectory = new FileInfo(outputDllName).DirectoryName;
			EmitResult emitResult = compilation.Emit(outputDllName);
			string runtimeConfigJsonFileContent =  GenerateRuntimeConfigJsonFile();
			string runtimeConfigJsonFilePath = workingDirectory + "\\" + assemblyName + ".runtimeconfig.json";
			File.WriteAllText(runtimeConfigJsonFilePath, runtimeConfigJsonFileContent);

			if(!emitResult.Success)
			{
				StringBuilder errors = new StringBuilder();

				IEnumerable<Diagnostic> failures = emitResult.Diagnostics
					.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

				foreach (Diagnostic diagnostic in failures)
				{
					errors.AppendLine($"{diagnostic.Location.GetLineSpan()} {diagnostic.GetMessage()}");
				}

				return new OutputResult { Errors = errors.ToString().TrimEnd() };
			}

			string commandPromptArgument = @"/C dotnet " + dllFilePath;

			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.FileName = "cmd.exe";
					process.StartInfo.Arguments = commandPromptArgument;
					process.StartInfo.RedirectStandardInput = true;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.CreateNoWindow = true;
					process.StartInfo.UseShellExecute = false;

					process.Start();
					process.StandardInput.WriteLine(input);
					process.StandardInput.Flush();
					process.StandardInput.Close();
					process.WaitForExit();

					string output = process.StandardOutput.ReadToEnd();
					return new OutputResult { Output = output };
				}
			}
			catch(Exception ex)
			{
				return new OutputResult { Errors = ex.Message };
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
