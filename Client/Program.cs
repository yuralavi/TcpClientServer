using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

try
{
    using (TcpClient client = new TcpClient("127.0.0.1", 7000))
    {
        Console.WriteLine("Client connected");

        using (NetworkStream stream = client.GetStream())
        {
            //request
            string text = "I send you request";
            byte[] inputbytes = Encoding.ASCII.GetBytes(text);

            stream.Write(inputbytes, 0, inputbytes.Length);
            stream.Flush();

            //response
            byte[] outputbytes = new byte[256];
            stream.Read(outputbytes, 0, outputbytes.Length);
            string response = Encoding.ASCII.GetString(outputbytes, 0, outputbytes.Length);
            Console.WriteLine(response);
        }
    }
    Console.WriteLine("Client disconnected");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
Console.ReadKey();