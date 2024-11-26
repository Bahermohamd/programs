using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.IO;

namespace do_ping
{
    class Program
    {
        public static string ExecuteCommand(string command)
        {
            int ExitCode;
            string fin;
            ProcessStartInfo ProcessInfo;
            Process Process;

            ProcessInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            ProcessInfo.CreateNoWindow = true;
            ProcessInfo.UseShellExecute = false;
            ProcessInfo.RedirectStandardOutput = true;


            Process = Process.Start(ProcessInfo);
            Process.WaitForExit();

            fin = Process.StandardOutput.ReadToEnd();

            ExitCode = Process.ExitCode;
            Process.Close();

            return fin;
        }

        static void Main(string[] args)
        {
            FileInfo MyFile;
            string ip, qr, file;
            int[] branches = { 2 };//, 02, 03, 04, 05, 06, 07, 08, 10, 12, 13, 14, 15, 17, 19, 20, 21, 22, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 39, 40, 41, 42, 43, 44, 45, 47, 48, 49, 50, 51, 52, 53, 53, 54, 55, 57, 58, 59 };
            Ping request = new Ping();
            string path = @"\\10.20.0.22\DB_BR_Backups\";
            string resault;

            using (StreamWriter writer = new StreamWriter(@"C:\users\baher.mohamed\Desktop\index.txt"))
            {

                foreach (int br in branches)
                {
                    ip = "10.20." + br.ToString() + ".10";
                    file = br.ToString() + "SRV.bak";

                    qr = "sqlcmd -S " + ip + " -U sa -P sa -Q " + '"' + "BACKUP DATABASE RetailChannelDatabase TO DISK='" +
                        path + file + "'"
                         + " WITH INIT,COMPRESSION,STATS=10" + '"';
                    //Console.WriteLine(qr);
                    PingReply response = request.Send(ip);
                    if (response.Status != IPStatus.Success) { Console.WriteLine(br + " is Down"); continue; }
                    else
                    {
                        MyFile = new FileInfo(path + file);
                        FileStream fs = MyFile.Create();
                        Console.WriteLine(br + " on goning");
                        resault = ExecuteCommand(qr);
                        Console.WriteLine(br + " Done");
                        writer.Write(resault);


                    }
                }
            }


            
            Console.WriteLine("All Done");
            Console.ReadLine();
        }
    }
}
