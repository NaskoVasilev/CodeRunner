using System;
using System.Collections.Generic;

namespace CodeManager
{
    public class CompilerArguments
    {
        private const string InvalidOutputFileExtensionsErrorMessage = "OutputFileExtension cannot be null or empty and should starts with point.";

        public CompilerArguments(string fileName, string workingDirectory)
        {
            FileName = fileName;
            WorkingDirectory = workingDirectory;
        }

        public CompilerArguments(string fileName, string workingDirectory, IEnumerable<string> sources = null)
        {
            Sources = sources;
            FileName = fileName;
            WorkingDirectory = workingDirectory;
        }

        public string FileName { get; private set; }

        public string WorkingDirectory { get; private set; }

        public string OutputFileExtension { get; set; }

        public IEnumerable<string> Sources { get; private set; }

        public string Arguments { get; set; }

        public string OutputFile => $"{FileName}{OutputFileExtension}";

        public string OutputFilePath
        {
            get
            {
                if(string.IsNullOrEmpty(OutputFileExtension) || !OutputFileExtension.StartsWith("."))
                {
                    throw new ArgumentException(InvalidOutputFileExtensionsErrorMessage);
                }

                return $"{WorkingDirectory}\\{FileName}{OutputFileExtension}";
            }
        }
    }
}
