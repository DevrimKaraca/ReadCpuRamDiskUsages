using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReadCpuRamDiskUsages
{
    class Program
    {
        static PerformanceCounter cpuUsage = new PerformanceCounter("Processor Information", "% Processor Utility", "_Total", true);
        static PerformanceCounter avaliableRam = new System.Diagnostics.PerformanceCounter("Memory", "Available MBytes");

        public static void Main(string[] args)
        {
            var cpu = cpuUsage.NextValue();
            var memory = avaliableRam.NextValue();
            while (true)
            {
                readPerformance(cpuUsage,"Cpu");
                readPerformance(avaliableRam,"Ram");
                Console.Clear();
            }
        }
        public static void readPerformance(PerformanceCounter usage, string Description)
        {
            try
            {
                string FileName = "";
                if (Description == "Ram") FileName = "avaliableRam.txt";
                else if (Description == "Cpu") FileName = "Cpu.txt";

                float x = 0;
                x = usage.NextValue();
                Console.WriteLine(x);
                File.AppendAllText("c:/"+ FileName, DateTime.Now + "\n" + x + "\n");

                Thread.Sleep(400);
            }

            catch (Exception ex)
            {
                Log.ErrorOccured(ex.ToString(), ex.StackTrace,MethodInfo.GetCurrentMethod().Name);
            }

        }
    }
    class Log
    {
        private static readonly object LockOfLog = new object();
        public static void ErrorOccured(string message, string stackTrace, string targetSite)
        {
            lock (LockOfLog)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    //Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                    string logDirectory = AppDomain.CurrentDomain.BaseDirectory;

                    logDirectory += "\\" + "Logs";
                    if (!Directory.Exists(logDirectory))
                        Directory.CreateDirectory(logDirectory);

                    logDirectory += "\\" + DateTime.Now.Year;
                    if (!Directory.Exists(logDirectory))
                        Directory.CreateDirectory(logDirectory);

                    logDirectory += "\\" + DateTime.Now.Month;
                    if (!Directory.Exists(logDirectory))
                        Directory.CreateDirectory(logDirectory);

                    string hyphenIcon = "-------------------------------------";
                    string path = logDirectory + "\\" + DateTime.Now.Date.ToShortDateString() + " - " + "Error" + ".txt";

                    if (File.Exists(path))
                    {
                        hyphenIcon = "\n-------------------------------------";
                    }

                    var sw = !File.Exists(path) ? new StreamWriter(path) : File.AppendText(path);

                    sb.AppendLine(hyphenIcon);
                    sb.AppendFormat("Log Zamanı : \n{0}\n\n", DateTime.Now.ToString());
                    sb.AppendFormat("Mesaj : \n{0}\n\n", message);
                    sb.AppendFormat("Kod Parçacığı : \n{0}\n\n", stackTrace);
                    sb.AppendFormat("Metot Adı: \n{0}\n\n", targetSite);
                    sb.AppendLine("Log Tipi : \nError \n");
                    sb.Append("-------------------------------------");

                    sw.Write(sb.ToString());
                    sw.Flush();
                    sw.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine( MethodInfo.GetCurrentMethod().Name + ex.StackTrace );
                }
            }
        }
    }
}
