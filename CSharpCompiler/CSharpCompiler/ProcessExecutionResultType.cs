using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpCompiler
{
	public enum ProcessExecutionResultType
	{
		Success = 0,
        TimeLimit = 1,
        MemoryLimit = 2,
        RunTimeError = 4,
	}
}
