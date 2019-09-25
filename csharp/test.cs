// Requires at least Mono 5.0 due to TLS issues

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;


public class ASRTest
{
   public static void ProcessData(ClientWebSocket ws, byte[] data, int count) {
        ws.SendAsync(new ArraySegment<byte>(data, 0, count), WebSocketMessageType.Binary, true, CancellationToken.None).Wait();
        byte[] result = new byte[4096];
        Task<WebSocketReceiveResult> receiveTask = ws.ReceiveAsync(new ArraySegment<byte>(result), CancellationToken.None);
        receiveTask.Wait();
        var receivedString = Encoding.UTF8.GetString(result, 0, receiveTask.Result.Count);
        Console.WriteLine("Result {0}", receivedString);
   }

   public static void ProcessFinalData(ClientWebSocket ws) {
        byte[] eof = Encoding.UTF8.GetBytes("{\"eof\" : 1}");
        ws.SendAsync(new ArraySegment<byte>(eof), WebSocketMessageType.Text, true, CancellationToken.None).Wait();
        byte[] result = new byte[4096];
        Task<WebSocketReceiveResult> receiveTask = ws.ReceiveAsync(new ArraySegment<byte>(result), CancellationToken.None);
        receiveTask.Wait();
        var receivedString = Encoding.UTF8.GetString(result, 0, receiveTask.Result.Count);
        Console.WriteLine("Result {0}", receivedString);
   }

   public static void Main()
   {
        ClientWebSocket ws = new ClientWebSocket();
        ws.ConnectAsync(new Uri("wss://api.alphacephei.com/asr/en/"), CancellationToken.None).Wait();

        FileStream fsSource = new FileStream("test.wav",
                   FileMode.Open, FileAccess.Read);

        byte[] data = new byte[16000];
        while (true) {
            int count = fsSource.Read(data, 0, 16000);
            if (count == 0)
                break;
            ProcessData(ws, data, count);
        }
        ProcessFinalData(ws);

        ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None).Wait();
   }
}
