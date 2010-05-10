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
            client = new ServerClient();
            
            InitializeComponent();

            InitShares();
        }

        private void InitShares()
        {
            shares = LoadShares();
            ReloadShares();
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolStripButton1.Checked)
            {
                shares = LoadShares();
            }
            else
            {
                shares.Clear();
            }

            ReloadShares();
        }

        private void ReloadShares()
        {
            sharesListView.Items.Clear();

            foreach (Share share in shares)
            {
                ListViewItem item = new ListViewItem(share.Name);
                item.SubItems.Add(share.Size.ToString());

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
            string hash = shares[sharesListView.SelectedIndices[0]].Hash;
            string[] addresses = client.RequestDownload(hash);

            System.ServiceModel.WSHttpBinding binding = new System.ServiceModel.WSHttpBinding();            
            Uri baseAddress = new Uri("http://127.0.0.1:1337/DeadAlbatross/Client/DeadAlbatrossClient");
            System.ServiceModel.EndpointAddress address = new System.ServiceModel.EndpointAddress(baseAddress);

            byte[] bytes = new ClientImplementationClient(binding, address).Download(hash);
            using (var file = System.IO.File.Create(shares[sharesListView.SelectedIndices[0]].Name))
            {
                file.Write(bytes, 0, bytes.Length);
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
    }
}
