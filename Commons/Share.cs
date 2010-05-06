using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeadAlbatross.Commons
{
    public class Share
    {
        public String Name { get; set; }
        public long Size { get; set; }
        public String Hash { get; set; }

        public Share()
        {
            Hash = "";
        }

        public override bool Equals(object obj)
        {
            if (obj as Share == null)
            {
                return false;
            }
            return Hash == (obj as Share).Hash;
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }
    }
}
