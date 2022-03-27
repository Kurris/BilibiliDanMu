using System;
using System.Threading.Tasks;
using BDanmuLib.Models;
using BDanMuLib;

namespace TestInConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            DanMuCore core = new ();
            core.ReceiveMessage += Core_ReceiveMessage;

            await core.ConnectAsync(6750632);

            Console.ReadKey();
        }

        private static void Core_ReceiveMessage(MessageType messageType, object obj)
        {
            Console.WriteLine(obj);
        }
    }
}
