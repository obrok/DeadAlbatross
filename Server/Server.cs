using System.Collections.Generic;
using System.ServiceModel;
using DeadAlbatross.Commons;

namespace DeadAlbatross.Server
{
    [ServiceContract(Namespace = "http://DeadAlbatross.Server")]
    class Server
    {
        private static Dictionary<Share, HashSet<string>> _shares = new Dictionary<Share, HashSet<string>>();

        [OperationContract]
        public Share[] ListShares()
        {
            Share[] result = new Share[_shares.Keys.Count];
            _shares.Keys.CopyTo(result, 0);
            return result;
        }

        [OperationContract]
        public void ReportShares(Share[] shares)
        {
            foreach (var item in shares)
            {
                if (!_shares.ContainsKey(item))
                {
                    _shares.Add(item, new HashSet<string>());
                    System.Console.WriteLine(item.Name);
                }
                _shares[item].Add(item.ClientAddress);
            }
        }

        [OperationContract]
        public string[] RequestDownload(string hash)
        {
            Share index = new Share { Hash = hash };
            string[] result = new string[_shares[index].Count];
            _shares[index].CopyTo(result);
            return result;
        }
    }
}
