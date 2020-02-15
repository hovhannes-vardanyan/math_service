using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("1-TCP:   2-UDP");
            int status = int.Parse(Console.ReadLine());
            if (status == 1)
            {
                TCPClient();
            }
            if (status == 2)
            {
                UDPClient();
            }

        }
        static void UDPClient()
        {
            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            const int portNumber = 1042;
            IPEndPoint iPEndPoint = new IPEndPoint(localhost, portNumber);
            UdpClient udpClient = new UdpClient(iPEndPoint);

            try
            {
                udpClient.Connect(iPEndPoint);
                Console.WriteLine("Enter The Massage");
                string message = Console.ReadLine();
                byte[] data = Encoding.UTF8.GetBytes(message);

                //sending message
                Console.WriteLine("Sending Message");
                udpClient.Send(data, data.Length);

                // receiving message
                byte[] receivedData = udpClient.Receive(ref iPEndPoint);
                string receivedMsg = Encoding.UTF8.GetString(data);
                Console.WriteLine("Received Messeage:");
                Console.WriteLine($"Result is {receivedMsg}");
                udpClient.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                udpClient.Close();
            }

        }
        static void TCPClient()
        {
            const int portNumber = 1042;
            byte[] buffer = new byte[1024];

            while (true)
            {
                Console.WriteLine("Enter The Message: Enter  n to stop");
                string message = Console.ReadLine();

                using (TcpClient client = new TcpClient("127.0.0.1", portNumber))
                {
                    using (NetworkStream stream = client.GetStream())
                    {
                        Console.WriteLine($"Sending: {message}");
                        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
                        stream.Write(messageBytes, 0, messageBytes.Length);

                        int bytesRead = stream.Read(buffer, 0, 1024);

                        string answer = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                        Console.WriteLine($"Received: {answer}");
                        Console.WriteLine();
                        if (answer == "n")
                        {
                            break;
                        }


                    }
                }
            }
        }
    }
}
