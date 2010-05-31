using System.Collections.Generic;
using System.ServiceModel;
using DeadAlbatross.Commons;
using System.ServiceModel.Channels;
using System.IO;


namespace DeadAlbatross.Client
{
    [ServiceBehavior(ConfigurationName="metadataSupport")]
    [ServiceContract(Namespace = "http://DeadAlbatross.Client")]
    public class ClientImplementation
    {
        private static Dictionary<string, string> files = new Dictionary<string, string>();

        public static void Register(string hash, string filepath)
        {
            files[hash] = filepath;
        }

        [OperationContract]
        public Stream Download(string hash)
        {
            FileStream stream = File.OpenRead(files[hash]);
            return stream;
        }
    }
}
