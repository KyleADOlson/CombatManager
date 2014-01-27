using System;
using System.IO;
using System.IO.Pipes;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace CombatManager.Pipes
{
    static class PipeClient
    {
        public static void SendFile(string filename)
        {
            using (NamedPipeClientStream pipeClient =
                new NamedPipeClientStream(".", "CombatManager.FilePipe", PipeDirection.InOut))
            {

                // Connect to the pipe or wait until the pipe is available.
                pipeClient.Connect();
                StreamWriter sw= new StreamWriter(pipeClient);
                
                // Display the read text to the console 
                sw.WriteLine(filename);
                sw.Flush();
                using (StreamReader sr = new StreamReader(pipeClient))
                {
                    IntPtr ptr = new IntPtr(int.Parse(sr.ReadLine()));
                    SetForegroundWindow(ptr);

                }
                
            }

        }

        [DllImportAttribute("User32.dll")]
        private static extern int SetForegroundWindow(IntPtr hWnd);
    }
}