using System;
using System.IO;

namespace SoftwareBot
{
    public class Program
    {
        public static SoftwareBot myBot;
        static void Main(string[] args)
        {
            DateTime BUILD_DATE = DateTime.MinValue;

            try
            {
                BUILD_DATE = Properties.Settings.Default.BUILD_DATE;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("ERROR: Could not load Build Date. " + e.Message);
            }
            Console.Error.WriteLine("Hello, I am SoftwareBot. -- I was unleashed upon the world by Adam Carruthers.\n");
            Console.Error.WriteLine("My build date is: " + BUILD_DATE + "\n");

            FileStream ostrm_out;
            StreamWriter writer_out;
            TextWriter oldErr = Console.Error;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm_out = new FileStream("./log.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer_out = new StreamWriter(ostrm_out);
                writer_out.AutoFlush = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open log.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }
            Console.WriteLine("*******************************************************************************");
            Console.WriteLine("Console out being redirected to log.txt. Please see log file for further output");
            Console.WriteLine("*******************************************************************************");
            Console.SetOut(writer_out);
            Console.SetError(writer_out);
            Console.WriteLine("Logging begins now:");

            try
            {
                myBot = new SoftwareBot();
                string commandStr;
                do
                {
                    commandStr = Console.ReadLine().ToLower();
                    myBot.DoCommand(commandStr);
                } while (commandStr != "close");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("ERROR: Could not load Build Date. " + e.Message);
            }

            writer_out.Close();
            ostrm_out.Close();
            Console.SetOut(oldOut);
            Console.SetError(oldErr);
        }


    }
}
