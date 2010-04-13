using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DeadAlbatross.Commons;

namespace DeadAlbatross.GUI
{
    class LocalShare : IEquatable<LocalShare>
    {
        public string FilePath;
        public string Name;
        public string StringSize
        {
            get
            {
                FileInfo info = new FileInfo(FilePath);
                long size = info.Length;

                if (size < 1024)
                    return size + " B";
                size /= 1024;
                if (size < 1024)
                    return size + " KB";
                size /= 1024;
                if (size < 1024)
                    return size + " MB";
                size /= 1024;
                return size + " GB";

                
            }
        }
        public long Size
        {
            get
            {
                return new FileInfo(FilePath).Length;
            }
        }

        public LocalShare(string filepath)
        {
            this.FilePath = filepath;
            this.Name = Path.GetFileName(filepath);
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object other)
        {
            if (other as LocalShare == null)
                return false;
            return Equals(other as LocalShare);
        }

        #region IEquatable<LocalShare> Members

        public bool Equals(LocalShare other)
        {
            return FilePath.Equals(other.FilePath);
        }

        #endregion

        public override int GetHashCode()
        {
            return FilePath.GetHashCode() + Name.GetHashCode();
        }

        public static implicit operator Share(LocalShare share)
        {
            return new Share { Name = share.Name, Size = share.Size };
        }
    }
}
