using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DeadAlbatross.Commons;
using DeadAlbatross.Client;
using System.ServiceModel.Channels;
using System.IO;

namespace DeadAlbatross.GUI
{
    public partial class MainShareForm : Form
    {
        private HashSet<LocalShare> localShares;
        private List<Share> shares;
        private ServerClient client;

        public MainShareForm()
        {
            localShares = new HashSet<LocalShare>();
            shares = new List<Share>();
            
            InitializeComponent();
        }

        private void ReloadLocalShares()
        {
            localSharesListView.Items.Clear();

            foreach (LocalShare share in localShares)
            {
                ListViewItem item = new ListViewItem(share.Name);
                item.SubItems.Add(share.StringSize.ToString());
                item.SubItems.Add(share.FilePath);
                item.SubItems.Add(share.Hash);

                localSharesListView.Items.Add(item);
            }
        }

        private void shareFilesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (String item in dialog.FileNames)
                {
                    LocalShare ls = new LocalShare(item);
                    DeadAlbatross.Client.ClientImplementation.Register(ls.Hash, ls.FilePath);
                    localShares.Add(ls);
                }
                ReloadLocalShares();
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (connectButton.Checked)
            {
                Connect();
            }
            else
            {
                Disconnect();
            }

            ReloadShares();
        }

        private void Disconnect()
        {
            if (client != null)
            {
                client.Close();
            }
            connectButton.Text = "Połącz";
            refreshButton.Enabled = false;
            shares.Clear();

            serverAddressTextbox.Enabled = true;
        }

        private void Connect()
        {
            System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding();
            Uri baseAddress = new Uri("http://" + serverAddressTextbox.Text + ":1337/DeadAlbatross/Server/DeadAlbatrossServer");
            System.ServiceModel.EndpointAddress address = new System.ServiceModel.EndpointAddress(baseAddress);
            client = new ServerClient(binding, address);

            connectButton.Text = "Rozłącz";
            refreshButton.Enabled = true;
            shares = LoadShares();

            serverAddressTextbox.Enabled = false;
        }

        private void ReloadShares()
        {
            sharesListView.Items.Clear();

            foreach (Share share in shares)
            {
                ListViewItem item = new ListViewItem(share.Name);
                item.SubItems.Add(share.Size.ToString());
                item.SubItems.Add(share.Hash);

                sharesListView.Items.Add(item);
            }
        }

        private List<Share> LoadShares()
        {
            List<Share> shares = new List<Share>();
            foreach(LocalShare share in localShares)
                shares.Add(share);
            client.ReportShares(shares.ToArray());
            List<Share> result = new List<Share>();
            foreach (Share share in client.ListShares())
                result.Add(share);
            return result;
        }

        public HashSet<Share> GetShares()
        {
            HashSet<Share> result = new HashSet<Share>();
            foreach (LocalShare share in localShares)
            {
                result.Add(share);
            }
            return result;
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            Share share = shares[sharesListView.SelectedIndices[0]];
            string hash = share.Hash;
            string[] addresses = client.RequestDownload(hash);

            System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
            binding.MessageEncoding = System.ServiceModel.WSMessageEncoding.Mtom;
            binding.TransferMode = System.ServiceModel.TransferMode.StreamedResponse;
            binding.MaxReceivedMessageSize = long.MaxValue;
            
            Uri baseAddress = new Uri("http://"+addresses[0]+":1337/DeadAlbatross/Client/DeadAlbatrossClient");
            System.ServiceModel.EndpointAddress address = new System.ServiceModel.EndpointAddress(baseAddress);

            ClientImplementationClient cic = new ClientImplementationClient(binding, address);

            using (var input = cic.Download(hash))
            {
                using (var file = System.IO.File.Create(shares[sharesListView.SelectedIndices[0]].Name))
                {
                    byte[] bytes = ReadFully(input);
                    file.Write(bytes, 0, bytes.Length);
                }
            }
        }

        public static void ReadWholeArray(Stream stream, byte[] data)
        {
            int offset = 0;
            int remaining = data.Length;
            while (remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                    throw new EndOfStreamException
                        (String.Format("End of stream reached with {0} bytes left to read", remaining));
                remaining -= read;
                offset += read;
            }
        }

        public static byte[] ReadFully(Stream stream)
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

        private void sharesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sharesListView.SelectedItems.Count == 0)
            {
                downloadButton.Enabled = false;
            }
            else
            {
                downloadButton.Enabled = true;
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            downloadButton.Enabled = false;
            shares = LoadShares();
            ReloadShares();
        }
    }
}
