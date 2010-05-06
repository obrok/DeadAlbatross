using System.Collections.Generic;
using System.ServiceModel;
using DeadAlbatross.Commons;

namespace DeadAlbatross.Server
{
    [ServiceContract(Namespace = "http://DeadAlbatross.Server")]
    class Server
    {
        private static HashSet<Share> _shares = new HashSet<Share>();

        [OperationContract]
        public Share[] ListShares()
        {
            List<Share> result = new List<Share>();
            foreach (var item in _shares)
            {
                result.Add(item);
            }
            return result.ToArray();
        }

        [OperationContract]
        public void ReportShares(Share[] shares)
        {
            foreach (var item in shares)
            {
                if (_shares.Add(item))
                {
                    System.Console.WriteLine(item.Name);
                }
            }
        }
    }
}
