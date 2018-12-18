using Dicom.Network;
using Listener;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace QueryRetrieveService
{
    public class GUILogic
    {
        public IDicomServer imageListener;
        public IDicomServer listener;
        public Process listenerProcess;
        public GUILogic()
        {
            // kill existing listeners
            Process[] pname = Process.GetProcessesByName("Listener");
            if (pname.Length != 0)
                foreach (Process proc in pname)
                    proc.Kill();
            


            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = readFromFile("listener");
            // continua a riempirmi di listener!!!!
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;


            listenerProcess = Process.Start(startInfo);

            imageListener = DicomServer.Create<CStoreSCP>(Int32.Parse(readFromFile("imageListenerPort")));
        }

        public static string readFromFile(string what)
        {
            String ret = "";

            StreamReader reader = new StreamReader("./Constants.txt");
            int lineNumber = 0;
            string line = reader.ReadLine();
            while (line != null)
            {
                if (line.Contains("@"+what+"@"))
                    ret = line.Split(' ')[line.Split(' ').Length - 1];
                lineNumber++;
                line = reader.ReadLine();
            }
            reader.Close();

            return ret;
        }


    }
}
