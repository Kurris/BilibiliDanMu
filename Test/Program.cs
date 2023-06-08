// See https://aka.ms/new-console-template for more information

using BDanMuLib;

var danmuCore = new DanMuCore();
danmuCore.ReceiveMessage += DanmuCore_ReceiveMessage;



var connect = danmuCore.ConnectAsync(6750632).Result;

Console.ReadKey();



void DanmuCore_ReceiveMessage(BDanmuLib.Models.MessageType messageType, object obj)
{
    Console.WriteLine($"{messageType}:{obj}");
}
