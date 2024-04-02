using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


internal class Program
{
    static IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 7000);
    static int couter = 1;
    static Random rnd = new Random();
    static Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();
    private static readonly object locker = new object();
    private static List<string> clientRequests = new List<string>();

    private static async Task Main(string[] args)
    {

        try
        {
            TcpListener listener = new(ipEndPoint);
            listener.Start();

            Console.WriteLine("Server started");

            while (couter < 100)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                clients.Add(couter, client);
                _ = Task.Run(() => HandleClientAsync(clients[couter], couter));
                Console.WriteLine($"This is main thread {Thread.CurrentThread.ManagedThreadId}");
                couter++;
            }
            listener.Stop();
            Console.WriteLine("Server stopped");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.ReadKey();
    }
    static async Task HandleClientAsync(TcpClient client, int clientNumber)
    {
        try
        {
            int randomNumber = rnd.Next(1000, 7000);
            Console.WriteLine($"Client {clientNumber} execute in thread {Thread.CurrentThread.ManagedThreadId}");
            Console.WriteLine($"Client {clientNumber} connected and will spleep {randomNumber}");
            await Task.Delay(randomNumber);

            using (NetworkStream stream = client.GetStream())
            {
                //request
                byte[] inputbytes = new byte[256];
                int bytesRead = stream.Read(inputbytes, 0, inputbytes.Length);
                string request = Encoding.ASCII.GetString(inputbytes, 0, bytesRead);
                Console.WriteLine($"This is request from client {clientNumber}: {request}");

                //response
                string text = "This is server response: You are client number " + clientNumber;
                byte[] outputbytes = Encoding.ASCII.GetBytes(text);

                stream.Write(outputbytes, 0, outputbytes.Length);
                stream.Flush();

                lock (locker)
                {
                    clientRequests.Add(request);
                }

            }
            Console.WriteLine($"Client {clientNumber} disconnected in thread {Thread.CurrentThread.ManagedThreadId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Problem {ex.Message} with clint {clientNumber}");
        }

        finally
        {
            client.Close();
        }
    }
}