using System;
using System.IO;

namespace MySqlScriptConcat;

public class Script
{
    StreamWriter OutputFile { get; set; }

    public Script(string scriptFileName)
    {
        if (File.Exists(scriptFileName))
        {
            File.Delete(scriptFileName);
        }
        OutputFile = File.AppendText(scriptFileName);
    }

    public void Concat(string searchPattern)
    {
        var files = Directory.GetFiles(".", searchPattern);
        foreach (var f in files)
        {
            string? line = "";
            using StreamReader sr = new(f);
            {
                while ((line = sr.ReadLine()) != null)
                {
                    OutputFile.WriteLine(line);
                }
            }
        }
    }

    public void WriteLine(string line)
    {
        OutputFile.WriteLine(line);
    }

    public void Close()
    {
        OutputFile.Close();
    }
}


