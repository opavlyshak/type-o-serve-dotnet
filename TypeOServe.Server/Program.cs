using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace TypeOServe.Server
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                int port = args.Length > 0
                    ? Int32.Parse(args[0])
                    : 8080;
                RunServer(port);

                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid port {0}", args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void RunServer(int port)
        {
            var tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();

            while (true)
            {
                Console.WriteLine(">>> Listening on port {0}", port);

                var client = tcpListener.AcceptTcpClient();
                Console.WriteLine(">>> New connection from {0}", client.Client.RemoteEndPoint);
                ProcessConnection(client);
                client.Close();
            }
        }

        private static void ProcessConnection(TcpClient client)
        {
            var stream = client.GetStream();
            using (var streamReader = new StreamReader(stream))
            using (var streamWriter = new StreamWriter(stream))
            {
                foreach (var requestLine in ReadLinesFromRequest(streamReader))
                {
                    Console.WriteLine(requestLine);
                }

                Console.WriteLine(">>> Type in your response:");

                var responseText = ReadResponseFromConsole();
                streamWriter.Write(responseText);
                streamWriter.Flush();
            }
        }

        private static IEnumerable<string> ReadLinesFromRequest(StreamReader reader)
        {
            var requestLine = reader.ReadLine();
            yield return requestLine;
            while (!String.IsNullOrWhiteSpace(requestLine))
            {
                requestLine = reader.ReadLine();
                yield return requestLine;
            }
        }

        private static string ReadResponseFromConsole()
        {
            var result = String.Join(Environment.NewLine,
                ReadLinesFromConsole());
            return result;
        }

        private static IEnumerable<string> ReadLinesFromConsole()
        {
            string line;
            do
            {
                line = Console.ReadLine();
                yield return line;
            } while (!String.IsNullOrWhiteSpace(line));
        }
    }
}
