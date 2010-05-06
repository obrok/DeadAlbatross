using System.Collections.Generic;
using System.ServiceModel;
using DeadAlbatross.Commons;
using System.ServiceModel.Channels;


namespace DeadAlbatross.Client
{
    [ServiceContract(Namespace = "http://DeadAlbatross.Client")]
    public class ClientImplementation
    {
        private static Dictionary<string, string> files = new Dictionary<string, string>();

        public static void Register(string hash, string filepath)
        {
            files[hash] = filepath;
        }

        [OperationContract]
        public byte[] Download(string hash)
        {
            return System.IO.File.ReadAllBytes(files[hash]);
        }
    }
}
