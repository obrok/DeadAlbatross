using System.Collections.Generic;
using System.ServiceModel;
using DeadAlbatross.Commons;

namespace DeadAlbatross.Server
{
    [ServiceContract(Namespace = "http://DeadAlbatross.Server")]
    class Server
    {
        private static List<Share> _shares = new List<Share>();

        [OperationContract]
        public Share[] ListShares()
        {
            return _shares.ToArray();
        }

        [OperationContract]
        public void ReportShares(Share[] shares)
        {
            System.Console.WriteLine("got " + _shares.Count + " shares");
            _shares.AddRange(shares);
        }
    }
}
