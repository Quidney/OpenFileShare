using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OpenFileShare
{
    internal class Program
    {
        static TcpListener listener;
        static TcpClient client;
        static NetworkStream stream;

        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                if (args[0] == "-h")
                {
                    Console.WriteLine("OpenFileShare.exe\nMade by: Quidney\n(Note if the connection isn't working, you might have to PortForward or make a firewall exception.)\n");
                    WrongUsage();
                }
            }
            else if (args.Length == 4)
            {
                if (args[0].ToLower() == "pull")
                {
                    Pull(args[1], args[2], args[3]);
                }
                else WrongUsage();
            }
            else if (args.Length == 3)
            {
                if (args[0].ToLower() == "push")
                {
                    Push(args[1], args[2]);
                }
                else WrongUsage();

            }
            else WrongUsage();
        }

        static void WrongUsage()
        {
            Console.WriteLine("Usage:\nOpenFileShare.exe pull IP PORT PASSWORD//To get the file from remote server\nOpenFileShare.exe push PORT PASSWORD//To send a file to a remote client");
        }

        //Get file from server
        static void Pull(string ipStr, string portStr, string passwordStr)
        {
            IPAddress.TryParse(ipStr.Trim(), out IPAddress ip);
            int.TryParse(portStr.Trim(), out int port);

            if (ip != null && port > 0 && port <= 65535)
            {
                try
                {
                    client = new TcpClient();
                    client.Connect(ip, port);
                    stream = client.GetStream();

                    byte[] buffer = Encoding.UTF8.GetBytes(passwordStr);
                    stream.Write(buffer, 0, buffer.Length);

                    byte[] answer = new byte[1];
                    stream.Read(answer, 0, answer.Length);
                    bool answerReceived = BitConverter.ToBoolean(answer, 0);

                    if (answerReceived)
                    {
                        Console.WriteLine("Passwords Match!");
                        PullStart();
                    }
                    else
                    {
                        Console.WriteLine("Wrong Password!");
                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

            }
            else WrongUsage();
        }

        //Send file to client
        static void Push(string portStr, string passwordStr)
        {
            int.TryParse(portStr.Trim(), out int port);

            if (port > 0 && port <= 65535)
            {
                try
                {
                    //Create listener
                    listener = new TcpListener(IPAddress.Any, port);
                    listener.Start();

                    client = listener.AcceptTcpClient();

                    stream = client.GetStream();

                    byte[] buffer = new byte[4096];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    string receivedPassword = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    //Password check is done on Server side only.
                    if (passwordStr == receivedPassword)
                    {
                        byte[] answer = BitConverter.GetBytes(true);
                        stream.Write(answer, 0, answer.Length);
                        Console.WriteLine("Passwords Match!");

                        PushStart();
                    }
                    else
                    {
                        byte[] answer = BitConverter.GetBytes(false);
                        stream.Write(answer, 0, answer.Length);
                        Console.WriteLine("Wrong Password!");
                        Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            else WrongUsage();
        }
        static void PushStart()
        {
            try
            {
                Console.Write("Path to send: ");
                string filePath = Console.ReadLine();

                if (File.Exists(filePath))
                {
                    Console.WriteLine("File found, sending...");

                    long fileSize = (new FileInfo(filePath)).Length;

                    using (FileStream fileStream = File.OpenRead(filePath))
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead;

                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            stream.Write(buffer, 0, bytesRead);
                        }
                        fileStream.Close();
                        Console.WriteLine("File sent successfully!");
                    }
                }
                else
                {
                    Console.WriteLine("File cannot be found.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Console.WriteLine("Closing Connection...");
                client.Close();
                stream.Close();
            }
        }

        static void PullStart()
        {
            try
            {
                Console.Write("(Full Path optional) File name + extension to save the file: ");
                string filePath = Console.ReadLine();
                Console.WriteLine("Waiting for server to send the file...");

                using (FileStream fileStream = File.Create(filePath))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                    fileStream.Close();
                    Console.WriteLine("File received successfully!");
                    Console.WriteLine("Path to created file: " + Path.GetFullPath(filePath));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Console.WriteLine("Closing Connection...");
                client.Close();
                stream.Close();
            }
        }

    }
}
