using Dicom.Network;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace QueryRetrieveService
{
    public class GUILogic
    {
        public Process listenerProcess;

        public static void clearImageThumbs()
        {
            // clear image thumbs folder
            DirectoryInfo di = new DirectoryInfo("./images/");
            foreach (FileInfo file in di.GetFiles())
                file.Delete();

        }
        public GUILogic()
        {
            clearImageThumbs();
            // kill existing listeners
            Process[] pname = Process.GetProcessesByName("Listener");
            if (pname.Length != 0)
                foreach (Process proc in pname)
                    proc.Kill();

            newProcess();
        }
        public void newProcess()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = readFromFile("listener");
            // continua a riempirmi di listener!!!!
            //   startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;

            listenerProcess = Process.Start(startInfo);
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
