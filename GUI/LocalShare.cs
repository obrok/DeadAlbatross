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
        private string _hash=null;

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
        public string Hash
        {
            get
            {
                if (_hash==null)
                    _hash = GetSHA1Hash(FilePath);
                return _hash;
            }
        }

        private static string GetSHA1Hash(string pathName)
        {
            string strResult = "";
            string strHashData = "";

            byte[] arrbytHashValue;
            System.IO.FileStream oFileStream = null;

            System.Security.Cryptography.SHA1CryptoServiceProvider oSHA1Hasher =
                       new System.Security.Cryptography.SHA1CryptoServiceProvider();

            try
            {
                oFileStream = GetFileStream(pathName);
                arrbytHashValue = oSHA1Hasher.ComputeHash(oFileStream);
                oFileStream.Close();

                strHashData = System.BitConverter.ToString(arrbytHashValue);
                strHashData = strHashData.Replace("-", "");
                strResult = strHashData;
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error!",
                         System.Windows.Forms.MessageBoxButtons.OK,
                         System.Windows.Forms.MessageBoxIcon.Error,
                         System.Windows.Forms.MessageBoxDefaultButton.Button1);
            }

            return (strResult);
        }
        private static System.IO.FileStream GetFileStream(string pathName)
        {
            return (new System.IO.FileStream(pathName, System.IO.FileMode.Open,
                      System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite));
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
            return Hash.Equals(other.Hash);
        }

        #endregion

        public override int GetHashCode()
        {
            return FilePath.GetHashCode() + Name.GetHashCode();
        }

        public static implicit operator Share(LocalShare share)
        {
            return new Share { Name = share.Name, Size = share.Size, Hash = share.Hash };
        }
    }
}
