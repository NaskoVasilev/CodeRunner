namespace CodeManager
{
    public interface ICompiler
    {
        CompileResult Compile(string cmdArguments);
    }
}
