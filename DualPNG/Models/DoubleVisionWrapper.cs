using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DualPNG.Models
{
    public class DoubleVisionWrapper
    {
        public static void GenerateImage(string workingDirectory, string withoutGamma, string withGamma, string outFile)
        {
            Process p = new Process();
            ProcessStartInfo pStart = new ProcessStartInfo();
            pStart.UseShellExecute = false;
            pStart.FileName = "cmd.exe";
            pStart.WorkingDirectory = workingDirectory;
            pStart.Arguments = String.Format("/C doubleVision {0} {1} {2}", withoutGamma, withGamma, outFile);
            p.StartInfo = pStart;
            p.Start();
            p.WaitForExit();
        }
    }
}