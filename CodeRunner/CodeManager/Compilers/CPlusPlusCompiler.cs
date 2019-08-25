namespace CodeManager.Compilers
{
    public class CPlusPlusCompiler : BaseCompiler
    {
        public override CompileResult Compile(CompilerArguments compilerArguments)
        {
            //$"/c cd C://CompiledSolutions & set PATH=%PATH%;C:\\MinGW\\bin; & g++ Task.cpp -o Task.exe"
            compilerArguments.OutputFileExtension = CompilationConstants.ExeFileExtension;
            string arguments = $"/c cd {compilerArguments.WorkingDirectory} & set PATH=%PATH%;{CompilationConstants.CppCompilerPath}; & g++ {compilerArguments.FileName}.cpp -o {compilerArguments.OutputFile}";
            compilerArguments.Arguments = arguments;

            return base.Compile(compilerArguments);
        }
    }
}
