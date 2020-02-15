using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MathService
{
    class Proram
    {
        static public Math math = new Math();
        static void Main(string[] args)
        {
            Console.WriteLine("Choose Service Type: 1-TCP   2-UDP");
            int status = int.Parse(Console.ReadLine());

            switch (status)
            {
                case 1:
                    TCPServer();
                    break;
                case 2:
                    UDPServer();
                    break;
                default:
                    Console.WriteLine("Wrong Input");
                    break;
            }


            Console.ReadKey();

        }
        static string MsgReader(string msg)
        {

            double result = 0;
            string message = String.Empty;
            string[] msgArr = msg.Split(':');
            try
            {
                switch (msgArr[0])
                {
                    case "+":
                        result = math.Add(double.Parse(msgArr[1]), double.Parse(msgArr[2]));
                        break;
                    case "-":
                        result = math.Sub(double.Parse(msgArr[1]), double.Parse(msgArr[2]));
                        break;
                    case "*":
                        result = math.Mult(double.Parse(msgArr[1]), double.Parse(msgArr[2]));
                        break;
                    case "/":
                        result = math.Div(double.Parse(msgArr[1]), double.Parse(msgArr[2]));
                        break;
                    default:
                        throw new InvalidCastException();

                }
                message = result.ToString();
            }

            catch (InvalidCastException)
            {
                message = "Wrong Input";
            }

            return message;
        }
        static void TCPServer()
        {
            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            const int portNumber = 1042;
            byte[] buffer = new byte[1024];
            TcpListener listener = new TcpListener(localhost, portNumber);
            listener.Start();
            while (true)
            {
                Console.WriteLine("Waiting for Client Input");
                using (TcpClient client = listener.AcceptTcpClient())
                {
                    Console.WriteLine("Connected");
                    using (NetworkStream stream = client.GetStream())
                    {

                        int bytesRead = stream.Read(buffer, 0, 1024);
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        if (message == "n")
                        {
                            Console.WriteLine("Sending Result");
                            byte[] closeBytes = Encoding.ASCII.GetBytes("Closed");
                            stream.Write(closeBytes, 0, closeBytes.Length);
                            Console.WriteLine("Closing");
                            break;
                        }
                        Console.WriteLine($"Message Received {message}");
                        string result = MsgReader(message);
                        Console.WriteLine("Sending Result");
                        byte[] answerBytes = Encoding.ASCII.GetBytes(result);
                        stream.Write(answerBytes, 0, answerBytes.Length);
                        Console.WriteLine("Closing");


                    }
                }
            }
        }
        static void UDPServer()
        {
            IPAddress localhost = IPAddress.Parse("127.0.0.1");
            const int portNumber = 1042;
            IPEndPoint IPEndPoint = new IPEndPoint(localhost, portNumber);
            UdpClient udpListener = new UdpClient(IPEndPoint);
            try
            {
                // udpListener.Connect(IPEndPoint);
                Console.WriteLine("Waiting for Message");

                byte[] bytes = udpListener.Receive(ref IPEndPoint);
                string message = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                Console.WriteLine("Received message");


                Console.WriteLine("Sending Messeage:");
                byte[] sendMessage = Encoding.ASCII.GetBytes(MsgReader(message));
                udpListener.Send(sendMessage, sendMessage.Length);


            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                udpListener.Close();
            }


        }
    }
}
