using System.Threading.Tasks;

namespace CodeManager.Executors
{
    public class CPlusPlusExecutor : BaseExecutor
    {
        public override Task<ProcessExecutionResult> Execute(string input, string fileName, string workingDirectory, string arguments = null)
        {
            arguments = $"/c cd {workingDirectory} & set PATH=%PATH%;{CompilationConstants.CppCompilerPath}; & {fileName}";
            return base.Execute(input, fileName, workingDirectory, arguments);
        }
    }
}
