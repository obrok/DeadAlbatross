using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Configuration;
using log4net;

namespace DeadAlbatross.Server
{
    class Program
    {
        private static ILog _log = LogManager.GetLogger(typeof(Program).Name);

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
                
                _log.Info("The server is running.");
                _log.Info("Waiting for <ENTER> to terminate service.");
                Console.ReadLine();

                selfHost.Close();
            }
            catch (CommunicationException ce)
            {
                _log.FatalFormat("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }
        }
    }
}
