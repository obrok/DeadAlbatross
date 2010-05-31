using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using DeadAlbatross.Client;
using System.Threading;
using DeadAlbatross.Commons;
using System.Configuration;

namespace DeadAlbatross.GUI
{
    static class Program
    {
        static void Init()
        {
            ServiceHost selfHost = new ServiceHost(
                typeof(DeadAlbatross.Client.ClientImplementation),
                Config.BaseAddress("Client"));

            try
            {
                selfHost.AddServiceEndpoint(
                    typeof(DeadAlbatross.Client.ClientImplementation),
                    new BasicHttpBinding("BasicHttpBinding_Client"),
                    ConfigurationSettings.AppSettings["ClientSuffix"]);

                new Thread(new ThreadStart(selfHost.Open)).Start();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Init();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainShareForm());
        }
    }
}
