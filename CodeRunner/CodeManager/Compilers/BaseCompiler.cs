using CodeManager.Enums;
using System;
using System.Diagnostics;

namespace CodeManager.Compilers
{
    public class BaseCompiler : ICompiler
    {
        public static ICompiler CreateCompiler(CompilerType compilerType)
        {
            switch (compilerType)
            {
                case CompilerType.CSharp:
                    return new CSharpCompiler();
                case CompilerType.CPlusPlus:
                    return new CPlusPlusCompiler();
                case CompilerType.Java:
                    return new JavaCompiler();
                default:
                    throw new ArgumentException("Unsupported compiler.");
            }
        }

        public virtual CompileResult Compile(CompilerArguments compilerArguments)
        {
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = compilerArguments.Arguments,
                FileName = "cmd.exe",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            process.StartInfo = info;

            process.Start();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            var compileResult = new CompileResult
            {
                Errors = errors,
                CompiledFilePath = compilerArguments.OutputFilePath
            };
            return compileResult;
        }
    }
}
