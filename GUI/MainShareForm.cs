using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DeadAlbatross.GUI
{
    public partial class MainShareForm : Form
    {
        private HashSet<LocalShare> localShares;

        public MainShareForm()
        {
            localShares = new HashSet<LocalShare>();
            InitializeComponent();
        }

        private void Reload()
        {
            locallySharedListView.Items.Clear();

            foreach (LocalShare share in localShares)
            {
                ListViewItem item = new ListViewItem(share.Name);
                item.SubItems.Add(share.Size.ToString());
                item.SubItems.Add(share.FilePath);

                locallySharedListView.Items.Add(item);
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
                    localShares.Add(new LocalShare(item));
                }
                Reload();
            }
        }
    }
}
