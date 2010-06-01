using System.Collections.Generic;
using System.ServiceModel;
using DeadAlbatross.Commons;
using System.ServiceModel.Channels;
using System.IO;
using log4net;
using System;

namespace DeadAlbatross.Client
{
    [ServiceBehavior(ConfigurationName = "metadataSupport")]
    [ServiceContract(Namespace = "http://DeadAlbatross.Client")]
    public class ClientImplementation
    {
        private static Dictionary<string, string> files = new Dictionary<string, string>();
        private static ILog _log = LogManager.GetLogger(typeof(ClientImplementation).Name);

        public static void Register(string hash, string filepath)
        {
            try
            {
                files[hash] = filepath;
            }
            catch (Exception e)
            {
                _log.ErrorFormat("An error occured: {0}", e.Message);
                throw e;
            }
        }

        [OperationContract]
        public Stream Download(string hash)
        {
            try
            {
                FileStream stream = File.OpenRead(files[hash]);
                return stream;
            }
            catch (Exception e)
            {
                _log.ErrorFormat("An error occured: {0}", e.Message);
                throw e;
            }
        }
    }
}
