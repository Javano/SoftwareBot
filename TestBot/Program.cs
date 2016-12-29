using System;

namespace SoftwareBot
{
    public class Program
    {
        public static SoftwareBot myBot = new SoftwareBot();
        static void Main(string[] args)
        {


            string commandStr;
            do
            {
                commandStr = Console.ReadLine().ToLower();
                myBot.DoCommand(commandStr);
            } while (commandStr != "close");
        }


    }
}
