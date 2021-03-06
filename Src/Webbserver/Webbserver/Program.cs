﻿using System;
using System.IO;
using System.Net;
using System.Text;

namespace Webbserver
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] input = { "http://localhost:5000/" };
            while (true)
            {
                SimpleListenerExample(input);
            }
        }

        // This example requires the System and System.Net namespaces.
        public static void SimpleListenerExample(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            HttpListener listener = new HttpListener();
            // Add the prefixes.
            foreach (string s in prefixes)
            {
                listener.Prefixes.Add(s);
            }
            listener.Start();
            Console.WriteLine("Listening...");

            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;

            string parentOfStartupPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../../../../../"));
            string indexPath = Path.Combine(parentOfStartupPath, "Content/index.html");

            string documentContents = File.ReadAllText(indexPath);

            Console.WriteLine($"Recived request for {request.Url}");
            Console.WriteLine(documentContents);

            // Obtain a response object.
            HttpListenerResponse response = context.Response;

            //Send a cookie to the client and add visitingdate and time
            Cookie timeStampCookie = new Cookie("VisitDate", DateTime.Now.ToString());                   
            //Add cookie to the response
            response.SetCookie(timeStampCookie);

            //Add header
            response.AddHeader("Expires", "54");


            // Construct a response.
            string responseString = documentContents;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            
            // You must close the output stream.
            output.Close();
            listener.Stop();

        }
    }
}
