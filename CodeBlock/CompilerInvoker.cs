using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CodeBlock
{
    class CompilerInvoker
    {
        public static string Compile(string src, bool tree_output=false)
        {
            string fileName = "__temp.pcat";
            string exeName = "RuleBuilder.exe";
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(src);
            }
            using (Process process = new Process())
            {
                ProcessStartInfo p = new ProcessStartInfo();
                p.FileName = exeName;
                p.Arguments = fileName + (tree_output ? " -t" : " -i");
                p.RedirectStandardOutput = true;
                p.UseShellExecute = false;
                process.StartInfo = p;
                StringBuilder output = new StringBuilder();
                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            output.AppendLine(e.Data);
                        }
                    };
                    process.Start();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                    outputWaitHandle.WaitOne();
                    if (process.ExitCode != 0)
                    {
                        MessageBox.Show(output.ToString());
                        throw new ArgumentException("Compile Error");
                    }
                    return output.ToString();
                }
            }
        }
    }
}
