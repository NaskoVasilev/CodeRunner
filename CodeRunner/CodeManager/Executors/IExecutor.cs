using System.Threading.Tasks;

namespace CodeManager.Executors
{
    public interface IExecutor
    {
        Task<ProcessExecutionResult> Execute(string input, string fileName, string workingDirectory, string arguments = null);
    }
}
