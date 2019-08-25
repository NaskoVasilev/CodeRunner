namespace CodeManager.Compilers
{
    public class JavaCompiler : BaseCompiler
    {
        public override CompileResult Compile(CompilerArguments compilerArguments)
        {
            //$"/c cd C://CompiledSolutions & set PATH=%PATH%;C:\\Program Files\\Java\\jdk1.8.0_181\\bin; & javac Task.java & jar cvfe Task.jar Task Task.class",
            compilerArguments.OutputFileExtension = CompilationConstants.JavaOutputFileExtension;
            string fileName = compilerArguments.FileName;
            string arguments = $"/c cd {compilerArguments.WorkingDirectory} & set PATH=%PATH%;{CompilationConstants.JavaCompilerPath}; & javac {fileName}.java & jar cvfe {fileName}{CompilationConstants.JavaOutputFileExtension} {fileName} {fileName}.class";
            compilerArguments.Arguments = arguments;

            return base.Compile(compilerArguments);
        }
    }
}
