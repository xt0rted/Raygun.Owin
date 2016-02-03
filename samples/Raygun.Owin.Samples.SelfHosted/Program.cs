using System;

namespace Raygun.Owin.Samples.SelfHosted
{
    using Microsoft.Owin.Hosting;

    public class Program
    {
        static void Main(string[] args)
        {
            var options = new StartOptions
            {
                Port = 64858
            };

            using (WebApp.Start<Startup>(options))
            {
                Console.ReadLine();
            }
        }
    }
}