using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

class PipeServer
{

    private bool continueServer = true;
    private NamedPipeServerStream pipeServer;
    private IntPtr hwnd;

    public PipeServer(IntPtr hwnd)
    {
        this.hwnd = hwnd;
    }

    public class PipeServerEventArgs : EventArgs
    {
        public string File { get; set; }
    }

    public event EventHandler<PipeServerEventArgs> FileRecieved;

    public void RunServer()
    {
        RunConnection();
        
    }

    private void RunConnection()
    {
        if (continueServer)
        {
            pipeServer = new NamedPipeServerStream("CombatManager.FilePipe", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                
            pipeServer.BeginWaitForConnection(new AsyncCallback((a) =>  {

                Console.WriteLine("Client connected.");
                try
                {
                    pipeServer.EndWaitForConnection(a);
                   
                    // Read user input and send that to the client process. 
                    using (StreamReader sr = new StreamReader(pipeServer))
                    {
                        String name = sr.ReadLine();
                        
                        
                        if (name != null)
                        {
                            if (FileRecieved != null)
                            {
                                FileRecieved(this, new PipeServerEventArgs() {File = name});
                            }
                        }
                        StreamWriter sw = new StreamWriter(pipeServer);
                        
                        sw.WriteLine(hwnd.ToInt32().ToString());
                        sw.Flush();
                        
                    }
                }
                catch (IOException ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
                try
                {
                    pipeServer.Close();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }
                pipeServer = null;
                RunConnection();

            }), null);
                
        }
    }

    public void EndServer()
    {
        continueServer = false;
        if (pipeServer != null)
        {
            try
            {
                //pipeServer.Close();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }

    }
    

}