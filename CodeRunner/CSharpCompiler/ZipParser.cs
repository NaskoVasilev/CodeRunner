﻿using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace CSharpCompiler
{
    public class ZipParser
    {
        private const string CSharpFileExtension = ".cs";

        public List<string> ExtractZipFile(string path)
        {
            List<string> filesData = new List<string>();

            using (FileStream file = File.OpenRead(path))
            {
                using (ZipArchive zip = new ZipArchive(file, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        if (entry.Name.EndsWith(CSharpFileExtension))
                        {
                            using (StreamReader reader = new StreamReader(entry.Open()))
                            {
                                string data = reader.ReadToEnd();
                                filesData.Add(data);
                            }
                        }
                    }
                }
            }

            return filesData;
        }
    }
}
