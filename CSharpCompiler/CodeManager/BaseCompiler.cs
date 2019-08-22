using CodeManager.Compilers;
using CodeManager.Enums;
using System;
using System.Diagnostics;
using System.IO;

namespace CodeManager
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

        public virtual CompileResult Compile(string cmdArguments)
        {
            Process process = new Process();
            ProcessStartInfo info = new ProcessStartInfo
            {
                Arguments = cmdArguments,
                FileName = "cmd.exe",
                UseShellExecute = false,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            process.StartInfo = info;

            process.Start();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return new CompileResult(errors);
        }
    }
}
