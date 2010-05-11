using System.Collections.Generic;
using System.ServiceModel;
using DeadAlbatross.Commons;
using System.ServiceModel.Channels;
using System.IO;


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
        public byte[] Download(string hash, int bytesRead)
        {
            int chunkSize = 10000;

            FileStream fs = System.IO.File.OpenRead((files[hash]));
            fs.Seek(bytesRead, SeekOrigin.Begin);

            long left = new FileInfo(files[hash]).Length - (long)bytesRead;
            long toRead = chunkSize;
            if (left < chunkSize)
            {
                toRead = left;
            }

            byte[] buffer = new byte[toRead];
            fs.Read(buffer, 0, (int)toRead);
            fs.Close();
            return buffer;
        }
    }
}
