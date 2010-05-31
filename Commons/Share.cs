using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DeadAlbatross.Commons
{
    [DataContract]
    public class Share
    {
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public long Size { get; set; }
        [DataMember]
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
