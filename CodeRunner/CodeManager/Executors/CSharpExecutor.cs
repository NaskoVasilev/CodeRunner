using System.Threading.Tasks;

namespace CodeManager.Executors
{
    public class CSharpExecutor : BaseExecutor
    {
        public override Task<ProcessExecutionResult> Execute(string input, string fileName, string workingDirectory, string arguments = null)
        {
            arguments = @"/C dotnet " + workingDirectory + "\\" + fileName;
            return base.Execute(input, fileName, workingDirectory, arguments);
        }
    }
}
