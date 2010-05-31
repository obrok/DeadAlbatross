using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Configuration;

namespace DeadAlbatross.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost selfHost = new ServiceHost(typeof(Server), Commons.Config.BaseAddress("Server"));

            try
            {
                selfHost.AddServiceEndpoint(
                    typeof(Server),
                    new BasicHttpBinding("BasicHttpBinding_Server"),
                    ConfigurationSettings.AppSettings["ServerSuffix"]);

                selfHost.Open();
                Console.WriteLine("The server is running.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

                selfHost.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }
        }
    }
}
