namespace CodeManager.Compilers
{
    public interface ICompiler
    {
        CompileResult Compile(CompilerArguments compilerArguments);
    }
}
