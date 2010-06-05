using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadAlbatross.Client;
using DeadAlbatross.Commons;
using System.IO;

namespace DeadAlbatross.GUI
{
    class MainController
    {
        private MainModel model;
        private ServerClient client;

        public MainController()
        {
            model = new MainModel();
        }

        internal IEnumerable<LocalShare> GetLocalShares()
        {
            return model.GetLocalShares();
        }

        internal void AddLocalShare(string item)
        {
            LocalShare ls = model.AddLocalShare(item);
            DeadAlbatross.Client.ClientImplementation.Register(ls.Hash, ls.FilePath);
        }

        internal void Disconnect()
        {
            if (client != null)
            {
                client.Close();
            }
            model.DeleteRemoteShares();
        }

        internal void LoadRemoteShares()
        {
            model.DeleteRemoteShares();

            List<Share> shares = new List<Share>();
            foreach (LocalShare share in model.GetLocalShares())
            {
                shares.Add(share);
            }
            client.ReportShares(shares.ToArray());
            
            foreach (Share share in client.ListShares())
            {
                model.AddRemoteShare(share);
            }
        }

        internal void Connect(string addressString)
        {
            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding("BasicHttpBinding_Server");
            System.ServiceModel.EndpointAddress address = new System.ServiceModel.EndpointAddress(Config.ServerBaseAddress(addressString));
            client = new ServerClient(binding, address);

            LoadRemoteShares();
        }

        internal IEnumerable<Share> GetRemoteShares()
        {
            return model.GetRemoteShares();
        }

        internal double DownloadShare(int index)
        {
            Share share = model.GetRemoteShares(index);

            string hash = share.Hash;
            string[] addresses = client.RequestDownload(hash);

            System.ServiceModel.BasicHttpBinding binding =
                new System.ServiceModel.BasicHttpBinding("BasicHttpBinding_Client");

            ClientImplementationClient cic =
                new ClientImplementationClient(binding,
                    new System.ServiceModel.EndpointAddress(Config.ClientBaseAddress(addresses[0])));


            DateTime time = DateTime.Now;

            using (var input = cic.Download(hash))
            {
                using (var file = System.IO.File.Create(share.Name))
                {
                    byte[] bytes = ReadFully(input);
                    file.Write(bytes, 0, bytes.Length);
                }
            }

            int diff = (DateTime.Now - time).Seconds;
            if (diff == 0)
            {
                diff = 1;
            }

            return share.Size / diff / 1024.0;
        }

        private static byte[] ReadFully(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }

        }
    }
}
