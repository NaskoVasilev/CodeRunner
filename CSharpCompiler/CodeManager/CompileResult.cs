namespace CodeManager
{
    public class CompileResult
    {
        public CompileResult(string errors)
        {
            Errors = errors;
        }

        public string Errors { get; private set; }

        public bool IsSuccessfull => string.IsNullOrEmpty(Errors);
    }
}
