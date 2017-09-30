﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _1150Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            string hostname = "api.wunderground.com";
            int port = 80;
            Byte[] bytesReceived = new Byte[512];

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            IPAddress[] adds = Dns.GetHostEntry(hostname).AddressList;
            IPAddress host = Dns.GetHostEntry(hostname).AddressList[0];
            IPEndPoint hostep = new IPEndPoint(host, port);
            sock.Connect(hostep);

            string request = "GET /api/d35da3b56e1c814c/conditions/q/CA/San_Francisco.xml HTTP/1.1 \r\n" + "Host: api.wunderground.com\r\n" + "Content-Length: 0\r\n" + "\r\n";
            int response = sock.Send(Encoding.UTF8.GetBytes(request));

            int bytes = 0;
            string data = "";
            do
            {
                bytes = sock.Receive(bytesReceived, bytesReceived.Length, 0);
                data = data + Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);

            Console.WriteLine(host);
            Console.WriteLine(data);
        }
    }
}
