﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace _1150Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Invalid arguments!\nUsage: <city name> <state abbreviation>");
                return;
            }

            const string hostname = "api.wunderground.com";
            const int port = 80;

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            var host = Dns.GetHostEntry(hostname).AddressList[0];
            var hostep = new IPEndPoint(host, port);
            socket.Connect(hostep);

            var bytesReceived = new byte[512];

            var request = $"GET /api/d35da3b56e1c814c/conditions/q/{args[1]}/{args[0].Replace(" ", "_")}.xml HTTP/1.1 \r\n" + "Host: api.wunderground.com\r\n" + "Content-Length: 0\r\n" + "\r\n";
            var response = socket.Send(Encoding.UTF8.GetBytes(request));

            var headerBytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
            var headerData = Encoding.ASCII.GetString(bytesReceived, 0, headerBytes);

            var contentLength = int.Parse(Regex.Match(headerData, @"Content-Length: (\d*)").Groups[1].Value);
            var initialResponse = Regex.Match(headerData, @"<response>[\s\S]*").Value;
            var responseBuffer = new byte[contentLength - initialResponse.Length];

            var responseBytes = socket.Receive(responseBuffer, responseBuffer.Length, 0);
            var responseData = initialResponse + Encoding.ASCII.GetString(responseBuffer, 0, responseBytes);

            if (responseData.Contains("querynotfound"))
            {
                Console.WriteLine("City not found! Please try again.");
                return;
            }

            var xml = XDocument.Parse(responseData);

            var city = xml.Descendants("city").First()?.Value;
            var state = xml.Descendants("state").First()?.Value;

            var weather = xml.Descendants("weather").First()?.Value;
            var temperature = xml.Descendants("temperature_string").First()?.Value;
            var wind = xml.Descendants("wind_string").First()?.Value;

            Console.WriteLine($"Weather forecast for {city}, {state}\n\n{weather}\n{temperature}\n{wind}");
        }
    }
}
