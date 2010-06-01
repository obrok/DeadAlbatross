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
using log4net;

namespace DeadAlbatross.GUI
{
    public partial class MainShareForm : Form
    {
        private static ILog _log = LogManager.GetLogger(typeof(Program).Name);

        private HashSet<LocalShare> localShares;
        private List<Share> shares;
        private ServerClient client;

        public MainShareForm()
        {
            try
            {
                localShares = new HashSet<LocalShare>();
                shares = new List<Share>();

                InitializeComponent();
            }
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ReloadLocalShares()
        {
            try
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
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void shareFilesButton_Click(object sender, EventArgs ea)
        {
            try
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
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void connectButton_Click(object sender, EventArgs ea)
        {
            try
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
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Disconnect()
        {
            try
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
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Connect()
        {
            try
            {
                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding("BasicHttpBinding_Server");
                System.ServiceModel.EndpointAddress address = new System.ServiceModel.EndpointAddress(Config.ServerBaseAddress(serverAddressTextbox.Text));
                client = new ServerClient(binding, address);

                connectButton.Text = "Rozłącz";
                refreshButton.Enabled = true;
                shares = LoadShares();

                serverAddressTextbox.Enabled = false;
            }
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ReloadShares()
        {
            try
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
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private List<Share> LoadShares()
        {
            try
            {
                List<Share> shares = new List<Share>();
                foreach (LocalShare share in localShares)
                    shares.Add(share);
                client.ReportShares(shares.ToArray());
                List<Share> result = new List<Share>();
                foreach (Share share in client.ListShares())
                    result.Add(share);
                return result;
            }
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<Share>();
            }

        }

        public HashSet<Share> GetShares()
        {
            try
            {
                HashSet<Share> result = new HashSet<Share>();
                foreach (LocalShare share in localShares)
                {
                    result.Add(share);
                }
                return result;
            }
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new HashSet<Share>();
            }

        }

        private void downloadButton_Click(object sender, EventArgs ea)
        {
            try
            {
                Share share = shares[sharesListView.SelectedIndices[0]];
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
                    using (var file = System.IO.File.Create(shares[sharesListView.SelectedIndices[0]].Name))
                    {
                        byte[] bytes = ReadFully(input);
                        file.Write(bytes, 0, bytes.Length);
                    }
                }

                double d = share.Size / (DateTime.Now - time).Seconds / 1024.0;

                MessageBox.Show(String.Format("Download complete, speed: {0} Kb/s", d), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public static void ReadWholeArray(Stream stream, byte[] data)
        {
            try
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
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void sharesListView_SelectedIndexChanged(object sender, EventArgs ea)
        {
            try
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
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void refreshButton_Click(object sender, EventArgs ea)
        {
            try
            {
                downloadButton.Enabled = false;
                shares = LoadShares();
                ReloadShares();
            }
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
