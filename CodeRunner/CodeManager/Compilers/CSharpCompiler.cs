using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeManager.Compilers
{
    public class CSharpCompiler : BaseCompiler
    {
        public override CompileResult Compile(CompilerArguments compilerArguments)
        {
            compilerArguments.OutputFileExtension = CompilationConstants.CSharpOutputFileExtension;

            CSharpCompilation compilation = BuildCompilation(compilerArguments);

            EmitResult emitResult = compilation.Emit(compilerArguments.OutputFilePath);

            string runtimeConfigJsonFileContent = GenerateRuntimeConfigJsonFile();
            string runtimeConfigJsonFilePath = compilerArguments.WorkingDirectory + "\\" + compilerArguments.FileName + ".runtimeconfig.json";
            File.WriteAllText(runtimeConfigJsonFilePath, runtimeConfigJsonFileContent);

            var compileResult = new CompileResult();

            if (!emitResult.Success)
            {
                StringBuilder errors = new StringBuilder();

                IEnumerable<Diagnostic> failures = emitResult.Diagnostics
                    .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (Diagnostic diagnostic in failures)
                {
                    errors.AppendLine($"{diagnostic.Location.GetLineSpan()} {diagnostic.GetMessage()}");
                }

                compileResult.Errors = errors.ToString();
                return compileResult;
            }

            compileResult.CompiledFilePath = compilerArguments.OutputFilePath;
            return compileResult;
        }

        private static CSharpCompilation BuildCompilation(CompilerArguments compilerArguments)
        {
            CSharpCompilation compilation = CSharpCompilation.Create(compilerArguments.FileName)
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

            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            foreach (var sourceCode in compilerArguments.Sources)
            {
                syntaxTrees.Add(SyntaxFactory.ParseSyntaxTree(sourceCode));
            }

            compilation = compilation.AddSyntaxTrees(syntaxTrees);
            return compilation;
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
    }
}
