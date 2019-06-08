namespace CSharpCompiler
{
	public class OutputResult
	{
		public string Output { get; set; }

		public string Errors { get; set; }

		public bool IsSuccesfull => Errors == null;
	}
}
