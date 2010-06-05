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
    public partial class MainView : Form
    {
        private static ILog _log = LogManager.GetLogger(typeof(Program).Name);


        private MainController controller;

        public MainView()
        {
            try
            {
                controller = new MainController();

                InitializeComponent();
            }
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void ReloadLocalSharesList()
        {
            try
            {
                localSharesListView.Items.Clear();

                foreach (LocalShare share in controller.GetLocalShares())
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
                        controller.AddLocalShare(item);
                    }
                    ReloadLocalSharesList();
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
            if (connectButton.Checked)
            {
                try
                {
                    controller.Connect(serverAddressTextbox.Text);
                    connectButton.Text = "Rozłącz";
                    refreshButton.Enabled = true;
                    serverAddressTextbox.Enabled = false;
                }
                catch (Exception e)
                {
                    connectButton.Checked = false;

                    string errorMessage = String.Format("An error occured: {0}", e.Message);
                    _log.Error(errorMessage);
                    MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } 
            }
            else
            {
                try
                {
                    controller.Disconnect();
                    connectButton.Text = "Połącz";
                    refreshButton.Enabled = false;
                    serverAddressTextbox.Enabled = true; 
                }
                catch (Exception e)
                {
                    connectButton.Checked = true;

                    string errorMessage = String.Format("An error occured: {0}", e.Message);
                    _log.Error(errorMessage);
                    MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            ReloadRemoteSharesList();
        }


        private void ReloadRemoteSharesList()
        {
            try
            {
                sharesListView.Items.Clear();

                foreach (Share share in controller.GetRemoteShares())
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

        private void downloadButton_Click(object sender, EventArgs ea)
        {
            try
            {
                double d = controller.DownloadShare(sharesListView.SelectedIndices[0]);

                MessageBox.Show(String.Format("Download complete, speed: {0} Kb/s", d), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                string errorMessage = String.Format("An error occured: {0}", e.Message);
                _log.Error(errorMessage);
                MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                controller.LoadRemoteShares();

                ReloadRemoteSharesList();
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
