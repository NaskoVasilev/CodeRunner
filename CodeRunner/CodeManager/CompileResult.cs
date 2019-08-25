namespace CodeManager
{
    public class CompileResult
    {
        public string Errors { get; set; }

        public string CompiledFilePath { get; set; }

        public bool IsSuccessfull => string.IsNullOrEmpty(Errors);
    }
}
