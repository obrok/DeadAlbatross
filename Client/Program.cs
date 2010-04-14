using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeadAlbatross.Commons;

namespace DeadAlbatross.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerClient client = new ServerClient();
            Share[] shares = new Share[1];
            shares[0] = new Share();
            shares[0].Name = "Hello";
            shares[0].Size = 123;

            client.ReportShares(shares);
            shares = client.ListShares();

            foreach(var share in shares)
                System.Console.WriteLine(share.Name + " " + share.Size);
        }
    }
}
