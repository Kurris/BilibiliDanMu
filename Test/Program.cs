// See https://aka.ms/new-console-template for more information

using BDanmuLib.Models;
using BDanMuLib;

var danmuCore = new DanMuCore();
danmuCore.ReceiveMessage += DanmuCore_ReceiveMessage;



var connect = danmuCore.ConnectAsync(7685334).Result;

Console.ReadKey();

static void DanmuCore_ReceiveMessage(MessageType messageType, object obj)
{
    if (messageType == MessageType.SUPER_CHAT_MESSAGE )
    {
        Console.WriteLine($"{messageType}:{obj}");
    }
}
