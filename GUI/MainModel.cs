using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadAlbatross.Commons;

namespace DeadAlbatross.GUI
{
    class MainModel
    {
        private HashSet<LocalShare> localShares;
        private List<Share> remoteShares;

        public MainModel()
        {
            localShares = new HashSet<LocalShare>();
            remoteShares = new List<Share>();
        }

        internal IEnumerable<LocalShare> GetLocalShares()
        {
            return localShares;
        }

        internal LocalShare AddLocalShare(string item)
        {
            LocalShare ls = new LocalShare(item);
            localShares.Add(ls);

            return ls;
        }

        internal void DeleteRemoteShares()
        {
            remoteShares.Clear();
        }

        internal void AddRemoteShare(Share share)
        {
            remoteShares.Add(share);
        }

        internal IEnumerable<Share> GetRemoteShares()
        {
            return remoteShares;
        }

        internal Share GetRemoteShares(int index)
        {
            return remoteShares[index];
        }
    }
}
