using System;
using System.IO;
using System.Threading.Tasks;

namespace CodeManager.Executors
{
    public class JavaExecutor : BaseExecutor
    {
        public override Task<ProcessExecutionResult> Execute(string input, string fileName, string workingDirectory, string arguments)
        {
            string fileNameWithputExtension = Path.GetFileNameWithoutExtension(fileName);
            arguments = $"/c cd {workingDirectory} & set PATH=%PATH%;{CompilationConstants.JavaCompilerPath}; & java -Xmx128m -jar -Djava.security.manager -Djava.security.policy=={fileNameWithputExtension}.policy {fileName}";
            return base.Execute(input, fileName, workingDirectory, arguments);
        }
    }
}
