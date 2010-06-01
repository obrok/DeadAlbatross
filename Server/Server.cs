using System.Collections.Generic;
using System.ServiceModel;
using DeadAlbatross.Commons;
using System.ServiceModel.Channels;
using System.IO;
using System;
using log4net;

namespace DeadAlbatross.Server
{
    [ServiceBehavior(ConfigurationName = "metadataSupport")]
    [ServiceContract(Namespace = "http://DeadAlbatross.Server")]
    class Server
    {
        private static Dictionary<Share, HashSet<string>> _shares = new Dictionary<Share, HashSet<string>>();
        private static ILog _log = LogManager.GetLogger(typeof(Server).Name);

        [OperationContract]
        public Share[] ListShares()
        {
            try
            {
                Share[] result = new Share[_shares.Keys.Count];
                _shares.Keys.CopyTo(result, 0);
                return result;
            }
            catch (Exception e)
            {
                _log.ErrorFormat("An error occured: {0}", e.Message);
                throw e;
            }
        }

        [OperationContract]
        public void ReportShares(Share[] shares)
        {
            try
            {
                string address = (OperationContext.Current.
                        IncomingMessageProperties
                            [RemoteEndpointMessageProperty.Name]
                                as RemoteEndpointMessageProperty).Address;

                foreach (var item in shares)
                {
                    if (!_shares.ContainsKey(item))
                    {
                        _shares.Add(item, new HashSet<string>());
                        _log.InfoFormat("File submitted: {0}", item.Name);
                    }
                    _shares[item].Add(address);
                }
            }
            catch (Exception e)
            {
                _log.ErrorFormat("An error occured: {0}", e.Message);
                throw e;
            }
        }

        [OperationContract]
        public string[] RequestDownload(string hash)
        {
            try
            {
                Share index = new Share { Hash = hash };
                string[] result = new string[_shares[index].Count];
                _shares[index].CopyTo(result);

                _log.InfoFormat("Download requested: {0}, advising: {1}", hash, result[0]);

                return result;
            }
            catch (Exception e)
            {
                _log.ErrorFormat("An error occured: {0}", e.Message);
                throw e;
            }

        }
    }
}
