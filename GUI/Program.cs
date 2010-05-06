using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using DeadAlbatross.Client;
using System.Threading;

namespace DeadAlbatross.GUI
{
    static class Program
    {
        static void Init()
        {
            Uri baseAddress = new Uri("http://localhost:1337/DeadAlbatross/Client");
            ServiceHost selfHost = new ServiceHost(typeof(DeadAlbatross.Client.ClientImplementation), baseAddress);

            try
            {
                selfHost.AddServiceEndpoint(typeof(DeadAlbatross.Client.ClientImplementation), new WSHttpBinding(), "DeadAlbatrossClient");

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);

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
